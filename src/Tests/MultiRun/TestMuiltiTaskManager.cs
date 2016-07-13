using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace GameLibrary.Tests.MultiRun
{
	[TestFixture]
	public class TestMuiltiTaskManager
	{
		[Test, Category("MuiltiTask"), Category("Unit"), Timeout(5000)]
		public void LoopMuiltiTask([Values(10, 100, 1000, 10000, 100000)] int step)
		{
			var tasks = new MyRunTask[step];
			for (int i = 0; i < step; i++)
			{
				var it = tasks[i] = new MyRunTask();
				it.Activate();
				Assert.IsTrue(it.Activated);
			}
			while (tasks.Any(x => x.Activated)) Thread.Sleep(10);

			for (int i = 0; i < step; i++)
			{
				var it = tasks[i];
				Assert.IsFalse(it.Activated);
				Assert.IsTrue(it.Done);
			}
		}

		[Test, Category("MuiltiTask"), Category("Performance")]
		public void LoopNativePerformance([Values(10, 100, 1000, 10000, 100000)] int step)
		{
			var tasks = new MyRunTask[step];

			var sw = Stopwatch.StartNew();
			for (int i = 0; i < step; i++)
			{
				var it = tasks[i] = new MyRunTask();
				it.CallBack();
			}
			while (tasks.Any(x => x.Activated)) Thread.Sleep(0);
			sw.Stop();

			for (int i = 0; i < step; i++)
			{
				var it = tasks[i];
				Assert.IsFalse(it.Activated);
				Assert.IsTrue(it.Done);
			}

			Console.WriteLine("Wait...");
			Console.WriteLine($"total: {sw.Elapsed.TotalMilliseconds:#,##0.00000000} ms");
			Console.WriteLine($"  avg: {sw.Elapsed.TotalMilliseconds / step:#,##0.00000000} ms");
			GC.Collect();
		}

		[Test, Category("MuiltiTask"), Category("Performance")]
		public void LoopMuiltiTaskPerformance([Values(10, 100, 1000, 10000, 100000)] int step)
		{
			var tasks = new MyRunTask[step];

			var sw = Stopwatch.StartNew();
			for (int i = 0; i < step; i++)
			{
				var it = tasks[i] = new MyRunTask();
				it.Activate();
			}
			while (tasks.Any(x => x.Activated)) Thread.Sleep(0);
			sw.Stop();

			for (int i = 0; i < step; i++)
			{
				var it = tasks[i];
				Assert.IsFalse(it.Activated);
				Assert.IsTrue(it.Done);
			}

			Console.WriteLine("Wait...");
			Console.WriteLine($"total: {sw.Elapsed.TotalMilliseconds:#,##0.00000000} ms");
			Console.WriteLine($"  avg: {sw.Elapsed.TotalMilliseconds / step:#,##0.00000000} ms");
			GC.Collect();
		}

		[Test, Category("MuiltiTask"), Category("DotMemoryUnit"), DotMemoryUnit(CollectAllocations = true)]
		public void LoopNativeDotMemoryUnite([Values(10, 100, 1000, 10000, 100000)] int step)
		{
			var tasks = new MyRunTask[step];

			var memoryPoint1 = dotMemory.Check();
			for (int i = 0; i < step; i++)
			{
				var it = tasks[i] = new MyRunTask();
				it.CallBack();
			}
			while (tasks.Any(x => x.Activated)) Thread.Sleep(0);
			var memoryPoint2 = dotMemory.Check(memory =>
			{
				var t = memory.GetTrafficFrom(memoryPoint1);

				Console.WriteLine("Wait...");
				Console.WriteLine("Total:");
				Console.WriteLine($" AllocatedMemory: ObjectsCount={t.AllocatedMemory.ObjectsCount:#,##0.00}; SizeInBytes={t.AllocatedMemory.SizeInBytes:#,##0.00}");
				Console.WriteLine($" CollectedMemory: ObjectsCount={t.CollectedMemory.ObjectsCount:#,##0.00}; SizeInBytes={t.CollectedMemory.SizeInBytes:#,##0.00}");
				Console.WriteLine($"DifferenceMemory: ObjectsCount={t.AllocatedMemory.ObjectsCount - t.CollectedMemory.ObjectsCount:#,##0.00}; SizeInBytes={t.AllocatedMemory.SizeInBytes - t.CollectedMemory.SizeInBytes:#,##0.00}");
				Console.WriteLine();

				Console.WriteLine("Avg:");
				Console.WriteLine($" AllocatedMemory: ObjectsCount~{1d * t.AllocatedMemory.ObjectsCount / step:#,##0.00}; SizeInBytes~{1d * t.AllocatedMemory.SizeInBytes / step:#,##0.00}");
				Console.WriteLine($" CollectedMemory: ObjectsCount~{1d * t.CollectedMemory.ObjectsCount / step:#,##0.00}; SizeInBytes~{1d * t.CollectedMemory.SizeInBytes / step:#,##0.00}");
				Console.WriteLine($"DifferenceMemory: ObjectsCount~{(1d * t.AllocatedMemory.ObjectsCount - t.CollectedMemory.ObjectsCount) / step:#,##0.00}; SizeInBytes~{(1d * t.AllocatedMemory.SizeInBytes - t.CollectedMemory.SizeInBytes) / step:#,##0.00}");
				Console.WriteLine();
			});

			for (int i = 0; i < step; i++)
			{
				var it = tasks[i];
				Assert.IsFalse(it.Activated);
				Assert.IsTrue(it.Done);
			}
			GC.Collect();
		}

		[Test, Category("MuiltiTask"), Category("DotMemoryUnit"), DotMemoryUnit(CollectAllocations = true)]
		public void LoopMuiltiTaskDotMemoryUnit([Values(10, 100, 1000, 10000, 100000)] int step)
		{
			var tasks = new MyRunTask[step];

			var memoryPoint1 = dotMemory.Check();
			for (int i = 0; i < step; i++)
			{
				var it = tasks[i] = new MyRunTask();
				it.Activate();
			}
			while (tasks.Any(x => x.Activated)) Thread.Sleep(0);
			var memoryPoint2 = dotMemory.Check(memory =>
			{
				var t = memory.GetTrafficFrom(memoryPoint1);

				Console.WriteLine("Wait...");
				Console.WriteLine("Total:");
				Console.WriteLine($" AllocatedMemory: ObjectsCount={t.AllocatedMemory.ObjectsCount:#,##0.00}; SizeInBytes={t.AllocatedMemory.SizeInBytes:#,##0.00}");
				Console.WriteLine($" CollectedMemory: ObjectsCount={t.CollectedMemory.ObjectsCount:#,##0.00}; SizeInBytes={t.CollectedMemory.SizeInBytes:#,##0.00}");
				Console.WriteLine($"DifferenceMemory: ObjectsCount={t.AllocatedMemory.ObjectsCount - t.CollectedMemory.ObjectsCount:#,##0.00}; SizeInBytes={t.AllocatedMemory.SizeInBytes - t.CollectedMemory.SizeInBytes:#,##0.00}");
				Console.WriteLine();

				Console.WriteLine("Avg:");
				Console.WriteLine($" AllocatedMemory: ObjectsCount~{1d * t.AllocatedMemory.ObjectsCount / step:#,##0.00}; SizeInBytes~{1d * t.AllocatedMemory.SizeInBytes / step:#,##0.00}");
				Console.WriteLine($" CollectedMemory: ObjectsCount~{1d * t.CollectedMemory.ObjectsCount / step:#,##0.00}; SizeInBytes~{1d * t.CollectedMemory.SizeInBytes / step:#,##0.00}");
				Console.WriteLine($"DifferenceMemory: ObjectsCount~{(1d * t.AllocatedMemory.ObjectsCount - t.CollectedMemory.ObjectsCount) / step:#,##0.00}; SizeInBytes~{(1d * t.AllocatedMemory.SizeInBytes - t.CollectedMemory.SizeInBytes) / step:#,##0.00}");
				Console.WriteLine();
			});

			for (int i = 0; i < step; i++)
			{
				var it = tasks[i];
				Assert.IsFalse(it.Activated);
				Assert.IsTrue(it.Done);
			}

			GC.Collect();
		}

		class MyRunTask : MultiRunTask
		{
			private double? _p;

			public MyRunTask() : base(0, 50) { }

			public bool Done => _p.HasValue;

			public override void CallBack()
			{
				_p = Math.Pow(Math.PI, Math.E);
				this.Deactivate();
			}
		}
	}
}