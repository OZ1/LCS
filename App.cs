using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using static System.Math;

namespace LCS
{
	class App
	{
		internal readonly byte[] Data;
		internal readonly String[] Strings;
		readonly DataSource DataSource;
		readonly StringTracker Track = new StringTracker();
		readonly LengthStartIndexComparer LengthStartIndexComparer;
		readonly StartLengthIndexComparer StartLengthIndexComparer;
		readonly int[] ByLength, ByStart;

		const string DefaultStateFileName = @"result";

		public App(byte[] data)
		{
			ByStart = new int[data.Length];
			for (var i = 0; i < ByStart.Length; i++) ByStart[i] = i;
			ByLength = (int[])ByStart.Clone();

			Data = data;
			Strings = new String[data.Length];
			LengthStartIndexComparer = new LengthStartIndexComparer(Strings);
			StartLengthIndexComparer = new StartLengthIndexComparer(Strings);
			DataSource = new DataSource(Strings, Data, ByStart);
		}

		void Initialize()
		{
			var bytes = new int[256];
			var initial = Track[0 & 1];
			for (var i = 0; i < Data.Length; i++)
			{
				var start = bytes[Data[i]] - 1;
				if (start < 0)
				{
					Strings[i].Start = i;
					bytes[Data[i]] = i + 1;
				}
				else
				{
					Strings[i].Start = start;
					initial.Add(i);
				}
			}
		}

		void Solve()
		{
			var remap = new ReMapDictionary();
			using var progress = new Progress(Track[0 & 1].Count);
			for (var length = 1; length < Data.Length; length++)
			{
				var tracking = Track[length & 1 ^ 1];
				var trackNew = Track[length & 1];
				if (tracking.Count == 0) break;

				remap.Clear();
				foreach (var i in tracking)
				{
					if (i + length >= Data.Length)
					{
						Strings[i].Length = length;
						trackNew.Free(i);
					}
					else if (Data[i + length] == Data[Strings[i].Start + length])
					{
						trackNew.Keep(i); // продолжаем отслѣживать
					}
					else if (remap.TryAdd(Strings[i].Start, Data[i + length], i, out var start))
					{
						Strings[i].Length = length; // предыдущий сегмент
						trackNew.Free(i);
					}
					else
					{
						Strings[i].Start = start; // перескок на другую вѣтвь
						trackNew.Keep(i);
					}
				}
				tracking.Clear();

				progress.Tracking = trackNew.Count;
				progress.Length = length;
			}
			Array.Sort(ByLength, LengthStartIndexComparer);
			Array.Sort(ByStart , StartLengthIndexComparer);
		}

		void Run()
		{
			Load();
			Initialize();
			Solve();
			Save();
		}

		public IEnumerable<StringSet> StringSets
		{
			get
			{
				if (Strings.Length == 0) yield break;
				yield return new StringSet(ByLength[0], DataSource);
				for (var i = 1; i < ByLength.Length; i++)
					if (Strings[ByLength[i - 1]] != Strings[ByLength[i]])
						yield return new StringSet(ByLength[i], DataSource);
			}
		}

		public IEnumerable<StringView> StringViews   => Enumerable.Range(0, Strings.Length).Select(i => new StringView(i, DataSource));
		public IEnumerable<StringView> ByLengthViews => ByLength                           .Select(i => new StringView(i, DataSource));
		public IEnumerable<StringView> ByStartViews  => ByStart                            .Select(i => new StringView(i, DataSource));

		void Load(string fileName = DefaultStateFileName)
		{
			try {
				using var f = new BinaryReader(File.OpenRead(fileName));
				for (var i = 0; i < Strings.Length; i++) Strings[i].Start  = f.ReadInt32();
				for (var i = 0; i < Strings.Length; i++) Strings[i].Length = f.ReadInt32();
			} catch (FileNotFoundException) { return; }
			Array.Sort(ByLength, LengthStartIndexComparer);
			Array.Sort(ByStart , StartLengthIndexComparer);
		}

		void Save(string fileName = DefaultStateFileName)
		{
			using var f = new BinaryWriter(File.OpenWrite(fileName));
			for (var i = 0; i < Strings.Length; i++) f.Write(Strings[i].Start);
			for (var i = 0; i < Strings.Length; i++) f.Write(Strings[i].Length);
		}

		public override string ToString() => $"Length = {Strings.Max(x => x.Length):x}";

		static void Main(string[] args) => new App(File.ReadAllBytes(args[0])).Run();
	}
}