using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using static System.Math;

class App
{
	readonly byte[] Data;
	readonly int[] Starts;
	readonly int[] Lengths;
	readonly List<int>[] Track = new List<int>[] { new List<int>(), new List<int>() };
	int Length;

	const string DefaultStateFileName = @"result.dat";

	public App(byte[] data)
	{
		Data = data;
		Starts = new int[data.Length];
		Lengths = new int[data.Length];
	}

	void Initialize()
	{
		var bytes = new int[256];
		var trackNew = Track[0 & 1];
		for (var i = 0; i < Data.Length; i++)
		{
			var start = bytes[Data[i]] - 1;
			if (start < 0)
			{
				Starts[i] = i;
				bytes[Data[i]] = i + 1;
			}
			else
			{
				Starts[i] = start;
				trackNew.Add(i);
			}
		}
		Length = 1;
	}

	void Solve()
	{
		var remap = new ReMapDictionary();
		for (Length = 1; Length < Data.Length; Length++)
		{
			var tracking = Track[Length & 1 ^ 1];
			var trackNew = Track[Length & 1];
			if (tracking.Count == 0) break;

			remap.Clear();
			trackNew.Clear();
			for (var j = 0; j < tracking.Count; j++)
			{
				var i = tracking[j];
				if (i + Length >= Data.Length)
				{
					Lengths[i] = Length;
				}
				else if (Data[i + Length] == Data[Starts[i] + Length])
				{
					trackNew.Add(i); // продолжаем отслѣживать
				}
				else if (remap.TryAdd(Starts[i], Data[i + Length], i, out var start))
				{
					Lengths[i] = Length; // предыдущий сегмент
				}
				else
				{
					Starts[i] = start; // перескок на другую вѣтвь
					trackNew.Add(i);
				}
			}
		}
	}

	void Run()
	{
		Load();

		Initialize();

		var thread = new Thread(ProgressProc) { IsBackground = true };
		thread.Start();

		Solve();

		thread.Abort();

		Save();

		var results = new List<String>();
		var strings = Strings.ToList();
		var set = new LCS.RangeSet();
		foreach (var str in strings)
			foreach (var start in str)
				if (set.TryAddNotOverlapping(start, str.Length))
					if (Data.Skip(start).Take(str.Length).Any(x => x == 0 || x == 0xFF))
						results.Add(new String(start, str.Length));
	}

	void Load(string fileName = DefaultStateFileName)
	{
		try
		{
			using var f = new BinaryReader(File.OpenRead(fileName));
			for (var i = 0; i < Starts.Length; i++) Starts[i] = f.ReadInt32();
			for (var i = 0; i < Lengths.Length; i++) Lengths[i] = f.ReadInt32();
		}
		catch (FileNotFoundException)
		{
			return;
		}
	}

	void Save(string fileName = DefaultStateFileName)
	{
		using var f = new BinaryWriter(File.OpenWrite(fileName));
		for (var i = 0; i < Starts.Length; i++) f.Write(Starts[i]);
		for (var i = 0; i < Lengths.Length; i++) f.Write(Lengths[i]);
	}

	IEnumerable<StringSet> Strings
	{
		get
		{
			var indexes = new int[Data.Length];
			for (var i = 0; i < indexes.Length; i++) indexes[i] = i;
			Array.Sort(indexes, (x, y) =>
			{
				int c;
				if ((c = Lengths[x] - Lengths[y]) != 0) return c;
				if ((c =  Starts[y] -  Starts[x]) != 0) return c;
				return y - x;
			});

			for (int i = indexes.Length - 1, j = i - 1; i >= 0; i = j)
			{
				var s = new StringSet(Lengths[indexes[i]], Starts[indexes[i]]) { Starts[indexes[i]], indexes[i] };
				for (; j >= 0; j--)
					if (Lengths[indexes[j]] == s.Length && Starts[indexes[j]] == s.Source)
						s.Add(indexes[j]);
					else break;
				yield return s;
			}
		}
	}

	struct String
	{
		public int Start;
		public int Length;

		public String(int start, int length)
		{
			Start = start;
			Length = length;
		}
	}

	class StringSet : List<int>
	{
		public int Length;
		public int Source;

		public StringSet(int length, int source)
		{
			Length = length;
			Source = source;
		}
	}

	class ReMapDictionary : Dictionary<ReMapKey, int>
	{
		public bool TryAdd(int source, byte ending, int index, out int start)
		{
			var key = new ReMapKey(source, ending);
			if (TryGetValue(key, out start)) return false;
			else Add(key, start = index);
			return true;
		}
	}

	struct ReMapKey : IEquatable<ReMapKey>
	{
		public int  Source;
		public byte Ending;

		public ReMapKey(int source, byte ending)
		{
			Source = source;
			Ending = ending;
		}

		public bool Equals(ReMapKey other) => Source == other.Source && Ending == other.Ending;
		public override bool Equals(object obj) => obj is ReMapKey ending && Equals(ending);
		public override int GetHashCode() => Source << 8 | Ending;
		public override string ToString() => $"{Source:x} [{Ending:X2}]";
	}

	void ProgressProc()
	{
		var t0 = DateTime.Now;
		for (var n1 = Max(Track[0].Count, Track[1].Count); n1 != 0;)
		{
			var n2 = Max(Track[0].Count, Track[1].Count);
			Console.WriteLine($"{Length}: {n2} - {n1 - n2}" + " за " + (DateTime.Now - t0).TotalSeconds);
			Thread.Sleep(200);
			n1 = n2;
		}
	}

	public override string ToString() => $"{Length}, Track={Max(Track[0].Count, Track[1].Count)}";

	static void Main(string[] args) => new App(File.ReadAllBytes(args[0])).Run();
}
