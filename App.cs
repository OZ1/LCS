using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Math;

class App
{
	byte[] Data;
	String[] Strings;
	readonly List<int>[] Track = new List<int>[] { new List<int>(), new List<int>() };
	int Length;

	const string DefaultStateFileName = @"result.dat";

	public App(byte[] data)
	{
		Data = data;
		Strings = new String[data.Length];
		Load();
	}

	void Initialize()
	{
		var bytes = new int?[256];
		var trackNew = Track[1 & 1];

		Length = 1;
		trackNew.Clear();
		for (var i = 0; i < Data.Length; i++)
		{
			if (bytes[Data[i]] is int start)
			{
				Debug.Assert(start < i);
				Strings[i] = new String { App = this, Index = i, Start = start };
				Strings[start].Count++;
				trackNew.Add(i);
			}
			else
			{
				Strings[i] = new String { App = this, Index = i, Start = i };
				bytes[Data[i]] = i;
			}
		}
	}

	void Run()
	{
		Initialize();
		var abandoned = new HashSet<int>();
		for (Length = 2; Length <= Data.Length - Length; Length++)
		{
			var tracking = Track[Length & 1 ^ 1];
			var trackNew = Track[Length & 1];
			if (tracking.Count == 0) break;

			trackNew.Clear();
			abandoned.Clear();
			foreach (var i in tracking)
			{
				var prev = Strings[i].Start;
				ref var current = ref Strings[i];
				ref var initial = ref Strings[prev];

				if (i + Length - 1 >= Data.Length)
				{
					if (Length == 0x15483) Debugger.Break();
					current.Length = Length - 1;
					current.Another = prev;
					Debug.Assert(initial.Count > 0);
					initial.Count--;
					if (initial.Count == 0)
					{
						initial.Length = Length - 1;
						initial.Another = i;
					}
					continue;
				}

				Debug.Assert(i > prev);
				Debug.Assert(current.Count == 0);
				Debug.Assert(current.Ends is null);

				var ending = Data[i + Length - 1];
				if (ending == Data[prev + Length - 1])
				{
					trackNew.Add(i);
					continue;
				}

				current.Length = Length - 1;
				current.Another = prev;
				Debug.Assert(initial.Count > 0);
				initial.Count--;
				if (initial.Count == 0)
				{
					initial.Length = Length - 1;
					initial.Another = i;
				}

				if (abandoned.Add(prev))
				{
					Debug.Assert(initial.Ends is null);
					initial.Ends = new Dictionary<byte, int>();
					initial.Ends.Add(ending, i);
					current.Start = i;
				}
				else if (initial.Ends.TryGetValue(ending, out var start))
				{
					Strings[start].Count++;
					current.Start = start;
					trackNew.Add(i);
					continue;
				}
				else
				{
					initial.Ends.Add(ending, i);
					current.Start = i;
				}
			}

			foreach (var item in abandoned)
				Strings[item].Ends = null;

			var added = tracking.Count - trackNew.Count;
		//	Console.WriteLine($"{Length}: {trackNew.Count} - {added}");
		}
		Save();
		var sorted = new String[Strings.Length]; Strings.CopyTo(sorted, 0); // 0x00015484
		Array.Sort(sorted, (x, y) =>
		{
			int c;
			if ((c = y.Length - x.Length) != 0) return c;
			if ((c = x.Another - y.Another) != 0) return c;
			return x.Index - y.Index;
		});
		var result = new List<String>();
		for (var i = 0; i < sorted.Length; i++)
			if (!result.Any(x => x.Start <= sorted[i].Index && sorted[i].Index <= x.Start + x.Length))
				result.Add(sorted[i]);
	}

	void Load(string fileName = DefaultStateFileName)
	{
		try
		{
			using (var f = new BinaryReader(File.OpenRead(fileName)))
			{
				var strings = new String[f.BaseStream.Length / 16];
				for (var i = 0; i < strings.Length; i++)
				{
					Strings[i] = new String
					{
						Start   = f.ReadInt32(),
						Count   = f.ReadInt32(),
						Length  = f.ReadInt32(),
						Another = f.ReadInt32(),
						Index = i,
						App = this,
					};
				}
			}
		}
		catch (FileNotFoundException)
		{
			return;
		}
	}

	void Save(string fileName = DefaultStateFileName)
	{
		using (var f = new BinaryWriter(File.OpenWrite(fileName)))
		{
			for (var i = 0; i < Strings.Length; i++)
			{
				f.Write(Strings[i].Start);
				f.Write(Strings[i].Count);
				f.Write(Strings[i].Length);
				f.Write(Strings[i].Another);
			}
		}
	}

	struct String
	{
		public int Start;
		public int Count;
		public int Length;
		public int Another;
		public int Index;
		public App App;
		public Dictionary<byte, int> Ends;

		public override string ToString() => $"{Index:x}: {Start:x} × {Count} \"" + string.Join(" ", App.Data.Skip(Start).Take(Min(8, Min(App.Length, App.Data.Length - Start))).Select(x => x.ToString("X2"))) + "\"";
	}

	public override string ToString() => $"{Length}, Track={Max(Track[0].Count, Track[1].Count)} d={Abs(Track[0].Count - Track[1].Count)}";

	static void Main(string[] args) => new App(File.ReadAllBytes(args[0])).Run();
}
