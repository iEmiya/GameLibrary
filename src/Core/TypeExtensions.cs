using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection.Emit;

namespace GameLibrary
{
	// https://codingsolution.wordpress.com/2013/07/12/activator-createinstance-is-slow/
	public static class TypeExtensions
	{
		#region CreateInstance
		public static object CreateInstance(this Type type)
		{
			Func<object> constructor;
			if (!_constructors.TryGetValue(type, out constructor))
			{
				constructor = type.GetConstructorDelegate();
				_constructors.TryAdd(type, constructor);
			}
			return constructor();
		}

		public static bool TryRemoveConstructor(this Type type)
		{
			Func<object> constructor;
			return _constructors.TryRemove(type, out constructor);
		}

		private static readonly ConcurrentDictionary<Type, Func<object>> _constructors = new ConcurrentDictionary<Type, Func<object>>();
		#endregion
		#region  CreateInstance[TArg1]
		public static object CreateInstance<TArg1>(this Type type, TArg1 arg1)
		{
			return PrivateActivator<TArg1>.CreateInstance(type, arg1);
		}

		public static bool TryRemoveConstructor<TArg1>(this Type type)
		{
			Func<TArg1, object> constructor;
			return PrivateActivator<TArg1>.TryRemoveConstructor(type, out constructor);
		}

		private static class PrivateActivator<TArg1>
		{
			private static readonly ConcurrentDictionary<Type, Func<TArg1, object>> _constructors = new ConcurrentDictionary<Type, Func<TArg1, object>>();
			internal static object CreateInstance(Type type, TArg1 arg1)
			{
				Func<TArg1, object> constructor;
				if (!_constructors.TryGetValue(type, out constructor))
				{
					constructor = (Func<TArg1, object>)type.GetConstructorDelegate(typeof(Func<TArg1, object>));
					_constructors.TryAdd(type, constructor);
				}
				return constructor(arg1);
			}
			internal static bool TryRemoveConstructor(Type type, out Func<TArg1, object> constructor)
			{
				return _constructors.TryRemove(type, out constructor);
			}
		}
		#endregion
		#region  CreateInstance[TArg1,TArg2]
		public static object CreateInstance<TArg1, TArg2>(this Type type, TArg1 arg1, TArg2 arg2)
		{
			return PrivateActivator<TArg1, TArg2>.CreateInstance(type, arg1, arg2);
		}

		public static bool TryRemoveConstructor<TArg1, TArg2>(this Type type)
		{
			Func<TArg1, TArg2, object> constructor;
			return PrivateActivator<TArg1, TArg2>.TryRemoveConstructor(type, out constructor);
		}

		private static class PrivateActivator<TArg1, TArg2>
		{
			private static readonly ConcurrentDictionary<Type, Func<TArg1, TArg2, object>> _constructors = new ConcurrentDictionary<Type, Func<TArg1, TArg2, object>>();
			internal static object CreateInstance(Type type, TArg1 arg1, TArg2 arg2)
			{
				Func<TArg1, TArg2, object> constructor;
				if (!_constructors.TryGetValue(type, out constructor))
				{
					constructor = (Func<TArg1, TArg2, object>)type.GetConstructorDelegate(typeof(Func<TArg1, TArg2, object>));
					_constructors.TryAdd(type, constructor);
				}
				return constructor(arg1, arg2);
			}
			internal static bool TryRemoveConstructor(Type type, out Func<TArg1, TArg2, object> constructor)
			{
				return _constructors.TryRemove(type, out constructor);
			}
		}
		#endregion
		#region  CreateInstance[TArg1,TArg2,TArg3]
		public static object CreateInstance<TArg1, TArg2, TArg3>(this Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			return PrivateActivator<TArg1, TArg2, TArg3>.CreateInstance(type, arg1, arg2, arg3);
		}

		public static bool TryRemoveConstructor<TArg1, TArg2, TArg3>(this Type type)
		{
			Func<TArg1, TArg2, TArg3, object> constructor;
			return PrivateActivator<TArg1, TArg2, TArg3>.TryRemoveConstructor(type, out constructor);
		}

		private static class PrivateActivator<TArg1, TArg2, TArg3>
		{
			private static readonly ConcurrentDictionary<Type, Func<TArg1, TArg2, TArg3, object>> _constructors = new ConcurrentDictionary<Type, Func<TArg1, TArg2, TArg3, object>>();
			internal static object CreateInstance(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
			{
				Func<TArg1, TArg2, TArg3, object> constructor;
				if (!_constructors.TryGetValue(type, out constructor))
				{
					constructor = (Func<TArg1, TArg2, TArg3, object>)type.GetConstructorDelegate(typeof(Func<TArg1, TArg2, TArg3, object>));
					_constructors.TryAdd(type, constructor);
				}
				return constructor(arg1, arg2, arg3);
			}
			internal static bool TryRemoveConstructor(Type type, out Func<TArg1, TArg2, TArg3, object> constructor)
			{
				return _constructors.TryRemove(type, out constructor);
			}
		}
		#endregion
		#region  CreateInstance[TArg1,TArg2,TArg3,TArg4]
		public static object CreateInstance<TArg1, TArg2, TArg3, TArg4>(this Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
		{
			return PrivateActivator<TArg1, TArg2, TArg3, TArg4>.CreateInstance(type, arg1, arg2, arg3, arg4);
		}

		public static bool TryRemoveConstructor<TArg1, TArg2, TArg3, TArg4>(this Type type)
		{
			Func<TArg1, TArg2, TArg3, TArg4, object> constructor;
			return PrivateActivator<TArg1, TArg2, TArg3, TArg4>.TryRemoveConstructor(type, out constructor);
		}

		private static class PrivateActivator<TArg1, TArg2, TArg3, TArg4>
		{
			private static readonly ConcurrentDictionary<Type, Func<TArg1, TArg2, TArg3, TArg4, object>> _constructors = new ConcurrentDictionary<Type, Func<TArg1, TArg2, TArg3, TArg4, object>>();
			internal static object CreateInstance(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
			{
				Func<TArg1, TArg2, TArg3, TArg4, object> constructor;
				if (!_constructors.TryGetValue(type, out constructor))
				{
					constructor = (Func<TArg1, TArg2, TArg3, TArg4, object>)type.GetConstructorDelegate(typeof(Func<TArg1, TArg2, TArg3, TArg4, object>));
					_constructors.TryAdd(type, constructor);
				}
				return constructor(arg1, arg2, arg3, arg4);
			}
			internal static bool TryRemoveConstructor(Type type, out Func<TArg1, TArg2, TArg3, TArg4, object> constructor)
			{
				return _constructors.TryRemove(type, out constructor);
			}
		}
		#endregion
		#region GetConstructorDelegate
		public static Func<object> GetConstructorDelegate(this Type type)
		{
			return (Func<object>)GetConstructorDelegate(type, typeof(Func<object>));
		}

		public static Delegate GetConstructorDelegate(this Type type, Type delegateType)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (delegateType == null)
				throw new ArgumentNullException(nameof(delegateType));

			var genericArguments = delegateType.GetGenericArguments();
			var argTypes = genericArguments.Length > 1 ? genericArguments.Take(genericArguments.Length - 1).ToArray() : Type.EmptyTypes;

			var constructor = type.GetConstructor(argTypes);
			if (constructor == null)
			{
				if (argTypes.Length == 0)
				{
					throw new InvalidProgramException($"Type '{type.Name}' doesn't have a parameterless constructor.");
				}
				throw new InvalidProgramException($"Type '{type.Name}' doesn't have the requested constructor.");
			}

			var dynamicMethod = new DynamicMethod("DM$_" + type.Name, type, argTypes, type);
			var ilGen = dynamicMethod.GetILGenerator();
			for (var i = 0; i < argTypes.Length; i++)
			{
				ilGen.Emit(OpCodes.Ldarg, i);
			}
			ilGen.Emit(OpCodes.Newobj, constructor);
			ilGen.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(delegateType);
		}
		#endregion
	}
}
