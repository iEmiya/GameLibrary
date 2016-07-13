using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace GameLibrary
{
	public sealed class MuiltiTaskManager : IDisposable
	{
		private const int Limitation2AddTasks = 1000;

		public static MuiltiTaskManager Default { get; } = new MuiltiTaskManager();

		private static readonly Stopwatch m_watch;
		private static long m_lastTicks;

		static MuiltiTaskManager()
		{
			m_watch = Stopwatch.StartNew();
		}

		private static long GetElapsedTicks()
		{
			long seed = m_watch.ElapsedTicks;
			if (seed == m_lastTicks)
			{
				seed = m_lastTicks++;
			}
			m_lastTicks = seed;
			return seed;
		}

		private bool _isRun;

		private readonly SortedDictionary<long, MultiRunTask> _tasksCollection;

		private readonly ConcurrentQueue<MultiRunTask> _addedTasks;
		private readonly ConcurrentQueue<MultiRunTask> _removedTasks;

		private readonly SemaphoreSlim _lock;
		private readonly Thread _mainThread;
		private volatile bool _activated;

		public MuiltiTaskManager()
		{
			_tasksCollection = new SortedDictionary<long, MultiRunTask>();
			_addedTasks = new ConcurrentQueue<MultiRunTask>();
			_removedTasks = new ConcurrentQueue<MultiRunTask>();

			_lock = new SemaphoreSlim(0, int.MaxValue);
			_mainThread = new Thread(new ThreadStart(Execute));
			_isRun = true;
			if (!_mainThread.IsAlive)
			{
				_mainThread.Start();
			}
		}

		public bool IsCompleted => !_activated;

		public void Add(MultiRunTask node)
		{
			Contract.Assert(node.manager == null);
			Contract.Assert(node.priority == 0);
			node.manager = this;
			_addedTasks.Enqueue(node);
			if (!_activated)
			{
				_lock.Release();
			}
		}

		public void Remove(MultiRunTask node)
		{
			Contract.Assert(this == node.manager);
			Contract.Assert(node.priority != 0);
			node.manager = null;
			_removedTasks.Enqueue(node);
			if (!_activated)
			{
				_lock.Release();
			}
		}

		public void Dispose()
		{
			_isRun = false;
		}

		private void Execute()
		{
			MultiRunTask t;
			while (_isRun)
			{
				if (_activated || !_lock.Wait(5))
				{
					if (_addedTasks.IsEmpty && _tasksCollection.Count == 0 && _removedTasks.IsEmpty)
					{
						_activated = false;
						continue;
					}
				}
				_activated = true;

				var elapsedTicks = GetElapsedTicks();
				long limitation = Limitation2AddTasks;

				while (_addedTasks.TryDequeue(out t))
				{
					t.priority = ++elapsedTicks + t.dueTime;
					_tasksCollection.Add(t.priority, t);
					if (0 == --limitation) break;
				}

				foreach (var task in _tasksCollection)
				{
					if (task.Key > ++elapsedTicks) break;
					t = task.Value;

					t.CallBack();

					if (!t.Activated) continue;

					_removedTasks.Enqueue(t);
					if (t.period <= 0) continue;

					t.dueTime = t.period;
					_addedTasks.Enqueue(t);
				}

				while (_removedTasks.TryDequeue(out t))
				{
					_tasksCollection.Remove(t.priority);
				}
			}
		}
	}

	public class MultiRunTask
	{
		public MultiRunTask()
		{
		}

		public MultiRunTask(int dueTime, int period)
		{
			this.dueTime = dueTime * TimeSpan.TicksPerMillisecond;
			this.period = period * TimeSpan.TicksPerMillisecond;
		}

		public long dueTime;
		public long period;
		public Action Func;

		internal MuiltiTaskManager manager;
		internal long priority;

		public virtual void CallBack()
		{
			Func?.Invoke();
		}

		public bool Activated => manager != null;

		public void Activate(MuiltiTaskManager manager = null)
		{
			(manager ?? MuiltiTaskManager.Default).Add(this);
		}

		public virtual void Deactivate()
		{
			this.manager?.Remove(this);
		}
	}
}