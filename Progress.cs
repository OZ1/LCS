using System;
using System.Threading;

namespace LCS
{
	class Progress : IDisposable
	{
		public readonly ManualResetEvent Exit = new ManualResetEvent(false);
		readonly Thread Thread;
		public int Tracking;
		public int Length;
		int Prev;
		int Time;

		public Progress(int total)
		{
			Length = 1;
			Prev = total;
			Thread = new Thread(ThreadProc) { IsBackground = true };
			Thread.Start();
		}

		void ThreadProc()
		{
			while (!Exit.WaitOne(1000))
			{
				Time++;
				if (Tracking == 0) break;
				Console.WriteLine(ToString());
				Prev = Tracking;
			}
		}

		public void Dispose()
		{
			if (Exit.Set())
				Thread.Join();
		}

		public override string ToString() => $"{Length}: {Tracking} - {Prev - Tracking} за {Time}";
	}
}
