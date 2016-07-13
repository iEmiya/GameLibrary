using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace GameLibrary.Tests.MultiRun
{
	[TestFixture]
	public class TestFastQueue
	{
		[Test, Category("Queue"), Category("Performance")]
		public void LoopNativePerformance([Values(100000, 1000000, 10000000)] int step)
		{
			var it = Environment.TickCount;
			var tmp = new Queue<int>();

			var sw = Stopwatch.StartNew();
			var queue = new Queue<int>();
			for (int i = 0; i < step; i++)
			{
				queue.Enqueue(it);
			}
			sw.Stop();
			Console.WriteLine("Enqueue...");
			Console.WriteLine($"total: {sw.Elapsed.TotalMilliseconds:#,##0.00000000} ms");
			Console.WriteLine($"  avg: {sw.Elapsed.TotalMilliseconds / step:#,##0.00000000} ms");
			GC.Collect();

			sw.Restart();
			while (queue.Count != 0)
			{
				var d = queue.Dequeue();
			}
			sw.Stop();
			Console.WriteLine("Dequeue...");
			Console.WriteLine($"total: {sw.Elapsed.TotalMilliseconds:#,##0.00000000} ms");
			Console.WriteLine($"  avg: {sw.Elapsed.TotalMilliseconds / step:#,##0.00000000} ms");
			GC.Collect();
		}

		[Test, Category("Queue"), Category("DotMemoryUnit"), DotMemoryUnit(CollectAllocations = true)]
		public void LoopNativeDotMemoryUnit([Values(100000, 1000000, 10000000)] int step)
		{
			var it = Environment.TickCount;
			var tmp = new Queue<int>();

			var memoryPoint1 = dotMemory.Check();
			var queue = new Queue<int>();
			for (int i = 0; i < step; i++)
			{
				queue.Enqueue(it);
			}
			var memoryPoint2 = dotMemory.Check(memory =>
			{
				var t = memory.GetTrafficFrom(memoryPoint1);

				Console.WriteLine("Enqueue...");
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
			GC.Collect();

			memoryPoint1 = dotMemory.Check();
			while (queue.Count != 0)
			{
				var d = queue.Dequeue();
			}
			memoryPoint2 = dotMemory.Check(memory =>
			{
				var t = memory.GetTrafficFrom(memoryPoint2);

				Console.WriteLine("Dequeue...");
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
			GC.Collect();
		}

		[Test, Category("Queue"), Category("Performance")]
		public void LoopFastPerformance([Values(100000, 1000000, 10000000)] int step)
		{
			var it = Environment.TickCount;
			var tmp = new FastQueue<int>();

			var sw = Stopwatch.StartNew();
			var queue = new FastQueue<int>();
			for (int i = 0; i < step; i++)
			{
				queue.Enqueue(it);
			}
			sw.Stop();
			Console.WriteLine("Enqueue...");
			Console.WriteLine($"total: {sw.Elapsed.TotalMilliseconds:#,##0.00000000} ms");
			Console.WriteLine($"  avg: {sw.Elapsed.TotalMilliseconds / step:#,##0.00000000} ms");
			GC.Collect();

			sw.Restart();
			while (!queue.IsEmpty)
			{
				var d = queue.Dequeue();
			}
			sw.Stop();
			Console.WriteLine("Dequeue...");
			Console.WriteLine($"total: {sw.Elapsed.TotalMilliseconds:#,##0.00000000} ms");
			Console.WriteLine($"  avg: {sw.Elapsed.TotalMilliseconds / step:#,##0.00000000} ms");
			GC.Collect();
		}

		[Test, Category("Queue"), Category("DotMemoryUnit"), DotMemoryUnit(CollectAllocations = true)]
		public void LoopFastDotMemoryUnit([Values(100000, 1000000, 10000000)] int step)
		{
			var it = Environment.TickCount;
			var tmp = new FastQueue<int>();

			var memoryPoint1 = dotMemory.Check();
			var queue = new FastQueue<int>();
			for (int i = 0; i < step; i++)
			{
				queue.Enqueue(it);
			}
			var memoryPoint2 = dotMemory.Check(memory =>
			{
				var t = memory.GetTrafficFrom(memoryPoint1);

				Console.WriteLine("Enqueue...");
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
			GC.Collect();

			memoryPoint1 = dotMemory.Check();
			while (!queue.IsEmpty)
			{
				var d = queue.Dequeue();
			}
			memoryPoint2 = dotMemory.Check(memory =>
			{
				var t = memory.GetTrafficFrom(memoryPoint2);

				Console.WriteLine("Dequeue...");
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
			GC.Collect();
		}
	}
}