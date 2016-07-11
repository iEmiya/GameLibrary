using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace GameLibrary
{
	// You need install nuget package Microsoft.CodeAnalysis.CSharp.Scripting 1.3.2 inside your project
	public sealed class HotswapCSharpScript : IDisposable
	{
		public class Globals
		{
			public object arg1;
			public object arg2;
			public object arg3;
			public object arg4;
		}

		sealed class HotswapClass
		{
			internal HotswapClass(HotswapClassAttribute attribute, Type realObjectType)
			{
				InterfaceType = attribute.InterfaceType;
				RealObjectType = realObjectType;
				ClassFileSource = attribute.ClassFileSource;
			}

			internal readonly Type InterfaceType;
			internal readonly Type RealObjectType;
			internal readonly string ClassFileSource;
			internal bool? OverwriteAfterLaunch;
			internal string OverwriteClassFileSource;
		}
		
		private bool _disposed;
		private readonly List<Assembly> _references;

		// InterfaceType <- RealObjectType = HotSwapClassAttribute
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, HotswapClass>> _subtypes;
		private readonly ConcurrentDictionary<HotswapClass, ScriptRunner<object>> _runners;

		private readonly ReaderWriterLockSlim _locker;
		private readonly ConcurrentDictionary<string, FileSystemWatcher> _directoryies;
		private readonly ConcurrentDictionary<FileSystemWatcher, List<HotswapClass>> _watchers;

		public HotswapCSharpScript()
		{
			_disposed = false;
			_references = new List<Assembly>();

			_subtypes = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, HotswapClass>>();
			_runners = new ConcurrentDictionary<HotswapClass, ScriptRunner<object>>();

			_locker = new ReaderWriterLockSlim();
			_directoryies = new ConcurrentDictionary<string, FileSystemWatcher>();
			_watchers = new ConcurrentDictionary<FileSystemWatcher, List<HotswapClass>>();
		}

		public void Dispose()
		{
			_disposed = true;

			foreach (var d in _watchers)
			{
				var watcher = d.Key;
				watcher.Dispose();
			}
		}

		#region Load

		public HotswapCSharpScript Load<T>()
		{
			if (_disposed) throw new ObjectDisposedException(nameof(HotswapCSharpScript));

			var assembly = typeof(T).Assembly;
			var types =
				from t in assembly.GetTypes()
				let attributes = t.GetCustomAttributes(typeof(HotswapClassAttribute), true)
				where attributes != null && attributes.Length > 0
				select new { RealObjectType = t, Attributes = attributes.Cast<HotswapClassAttribute>() };
			foreach (var type in types)
			{
				foreach (var attr in type.Attributes)
				{
					var it = attr.InterfaceType;
					var t = type.RealObjectType;

					if (!it.IsInterface) throw new ApplicationException($"Type '{it}' must be interface");
					if (!it.IsAssignableFrom(t)) throw new ApplicationException($"Type '{t}' don't implement interface '{it}'");
					if (!File.Exists(attr.ClassFileSource)) throw new ApplicationException($"Not found file source '{attr.ClassFileSource}' type '{t}' is '{it}'");

					var hotswapClass = new HotswapClass(attr, t);

					var realObjectTypes = _subtypes.GetOrAdd(it, new ConcurrentDictionary<Type, HotswapClass>());
					if (!realObjectTypes.TryAdd(t, hotswapClass)) continue;

					if (attr.OverwriteWhenBoot) ReloadClassFileSource(hotswapClass);

					var directoryName = Path.GetDirectoryName(attr.ClassFileSource);

					_locker.EnterReadLock();
					try
					{
						FileSystemWatcher watcher;
						if (_directoryies.TryGetValue(directoryName, out watcher))
						{
							_watchers[watcher].Add(hotswapClass);
							continue;
						}
					}
					finally
					{
						_locker.ExitReadLock();
					}

					_locker.EnterWriteLock();
					try
					{
						var watcher = new FileSystemWatcher(directoryName, "*.cs");
						if (_directoryies.TryAdd(directoryName, watcher))
						{
							watcher.NotifyFilter = NotifyFilters.LastWrite;
							watcher.Changed += new FileSystemEventHandler(OnChangedFileSystem);

							_watchers.TryAdd(watcher, new List<HotswapClass>() { hotswapClass });
							watcher.EnableRaisingEvents = true;
						}
						else
						{
							watcher.EnableRaisingEvents = false;
							watcher.Dispose();
						}
					}
					finally
					{
						_locker.ExitWriteLock();
					}
				}
			}
			return this;
		}

		private void OnChangedFileSystem(object source, FileSystemEventArgs e)
		{
			var watcher = (FileSystemWatcher)source;
			if (e.ChangeType != WatcherChangeTypes.Changed) return;
			List<HotswapClass> attributes;
			if (!_watchers.TryGetValue(watcher, out attributes)) return;
			foreach (var attr in attributes)
			{
				if (!e.FullPath.Equals(attr.ClassFileSource)) continue;
				ReloadClassFileSource(attr);
			}
		}

		private void ReloadClassFileSource(HotswapClass attr)
		{
			string classFileSource = null;
			while (classFileSource == null)
			{
				try
				{
					using (var file = new FileStream(attr.ClassFileSource, FileMode.Open, FileAccess.Read, FileShare.None))
					using (var reader = new StreamReader(file))
					{
						classFileSource = reader.ReadToEnd();
					}
				}
				catch (IOException)
				{
					Thread.SpinWait(200);
				}
			}

			attr.OverwriteClassFileSource = classFileSource;
			attr.OverwriteAfterLaunch = true;

			ScriptRunner<object> runner;
			_runners.TryRemove(attr, out runner);
		}

		#endregion
		#region AddReferences

		public HotswapCSharpScript AddReferences(IEnumerable<Assembly> references)
		{
			_references.AddRange(references);
			return this;
		}

		#endregion
		#region Contain

		public bool Contain<TSub>()
		{
			var interfaceType = typeof(TSub);
			return Contain(interfaceType);
		}

		public bool Contain(Type interfaceType)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(HotswapCSharpScript));
			ConcurrentDictionary<Type, HotswapClass> types;
			return _subtypes.TryGetValue(interfaceType, out types);
		}

		public bool Contain<TSub, T>()
		{
			var interfaceType = typeof(TSub);
			var realObjectType = typeof(T);
			return Contain(interfaceType, realObjectType);
		}

		public bool Contain(Type interfaceType, Type realObjectType)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(HotswapCSharpScript));
			ConcurrentDictionary<Type, HotswapClass> types;
			if (!_subtypes.TryGetValue(interfaceType, out types)) return false;
			HotswapClass attr;
			return types.TryGetValue(realObjectType, out attr);
		}

		#endregion
		#region CreateInstance

		public TSub CreateInstance<TSub>(params object[] arguments)
			where TSub : class
		{
			if (_disposed) throw new ObjectDisposedException(nameof(HotswapCSharpScript));

			var interfaceType = typeof(TSub);
			ConcurrentDictionary<Type, HotswapClass> types;
			if (!_subtypes.TryGetValue(interfaceType, out types))
				throw new ArgumentOutOfRangeException(nameof(TSub), $"Not found types for subtype '{interfaceType}' inside {nameof(HotswapCSharpScript)}");

			var d = types.FirstOrDefault();
			if (d.Equals(default(KeyValuePair<Type, HotswapClass>)))
				throw new ArgumentOutOfRangeException($"Not found any type for subtype '{interfaceType}' inside {nameof(HotswapCSharpScript)}");

			var attr = d.Value;
			return (TSub)CreateInstanceInternal(attr, arguments);
		}

		public TSub CreateInstance<TSub, T>(params object[] arguments)
			where T : class, TSub
			where TSub : class
		{
			var interfaceType = typeof(TSub);
			var realObjectType = typeof(T);
			return (TSub)CreateInstance(interfaceType, realObjectType, arguments);
		}

		public object CreateInstance(Type interfaceType, Type realObjectType, params object[] arguments)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(HotswapCSharpScript));

			ConcurrentDictionary<Type, HotswapClass> types;
			if (!_subtypes.TryGetValue(interfaceType, out types))
				throw new ArgumentOutOfRangeException(nameof(interfaceType), $"Not found types for subtype '{interfaceType}' inside {nameof(HotswapCSharpScript)}");

			HotswapClass attr;
			if (!types.TryGetValue(realObjectType, out attr))
				throw new ArgumentOutOfRangeException(nameof(realObjectType), $"Not found type '{realObjectType}' for subtype '{interfaceType}' inside {nameof(HotswapCSharpScript)}");

			return CreateInstanceInternal(attr, arguments);
		}

		private object CreateInstanceInternal(HotswapClass attr, params object[] arguments)
		{
			if (arguments.Length <= 4)
			{
				var it = attr.InterfaceType;
				var t = attr.RealObjectType;

				if (!attr.OverwriteAfterLaunch.HasValue)
				{
					if (arguments.Length == 0) return t.CreateInstance();
					if (arguments.Length == 1) return t.CreateInstance(arguments[0]);
					if (arguments.Length == 2) return t.CreateInstance(arguments[0], arguments[1]);
					if (arguments.Length == 3) return t.CreateInstance(arguments[0], arguments[1], arguments[2]);
					if (arguments.Length == 4) return t.CreateInstance(arguments[0], arguments[1], arguments[2], arguments[3]);
				}
				else
				{
					ScriptRunner<object> runner = null;
					while (runner == null)
					{
						var classFileSource = attr.OverwriteClassFileSource;
						var overwrite = attr.OverwriteAfterLaunch.Value;
						if (!overwrite)
						{
							_runners.TryGetValue(attr, out runner);
							continue;
						}
						if (classFileSource == null) continue;

						string args = string.Empty;
						if (arguments.Length == 1) args = "arg1";
						else if (arguments.Length == 2) args = "arg1, arg2";
						else if (arguments.Length == 3) args = "arg1, arg2, arg3";
						else if (arguments.Length == 4) args = "arg1, arg2, arg3, arg4";

						var csCode = attr.OverwriteClassFileSource + $"\r\nreturn new {t.Name}({args});";
						var options = ScriptOptions.Default
							.AddReferences(typeof(HotswapCSharpScript).Assembly, it.Assembly, t.Assembly)
							.AddReferences(_references);

						var script = CSharpScript.Create<object>(csCode, options, typeof(Globals));
						var scriptDelegate = script.CreateDelegate();

						_runners.TryAdd(attr, scriptDelegate);
						attr.OverwriteClassFileSource = null;
						attr.OverwriteAfterLaunch = false;

						if (arguments.Length == 0) t.TryRemoveConstructor();
						else if (arguments.Length == 1) t.TryRemoveConstructor<object>();
						else if (arguments.Length == 2) t.TryRemoveConstructor<object, object>();
						else if (arguments.Length == 3) t.TryRemoveConstructor<object, object, object>();
						else if (arguments.Length == 4) t.TryRemoveConstructor<object, object, object, object>();
					}

					var globals = new Globals();
					if (arguments.Length >= 1) globals.arg1 = arguments[0];
					if (arguments.Length >= 2) globals.arg2 = arguments[1];
					if (arguments.Length >= 3) globals.arg3 = arguments[2];
					if (arguments.Length >= 4) globals.arg4 = arguments[3];

					return runner.Invoke(globals).Result;
				}
			}

			throw new NotSupportedException($"Not supported with {arguments.Length} arguments");
		}

		#endregion
	}
}
