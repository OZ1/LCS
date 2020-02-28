using System;
using System.Collections.Generic;
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

		const string DefaultStateFileName = @"result";

		public App(byte[] data)
		{
			Data = data;
			Strings = new String[data.Length];
			DataSource = new DataSource(Strings, Data);
		}

		int Initialize()
		{
			var bytes = new int[256];
			var trackNew = Track[0 & 1];
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
					trackNew.Add(i);
				}
			}
			return trackNew.Count;
		}

		void Solve()
		{
			var total = Initialize();

			using var progress = new Progress(total);

			var remap = new ReMapDictionary();
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
		}

		void Run()
		{
			Load();
			Solve();
			Save();
		}

		IEnumerable<int> GetIndices() => Enumerable.Range(0, Strings.Length);

		int[] GetSortedIndices()
		{
			var indices = new int[Strings.Length];
			for (var i = 0; i < indices.Length; i++) indices[i] = i;
			Array.Sort(Strings, indices);
			return indices;
		}

		public IEnumerable<StringSet> StringSets
		{
			get
			{
				if (Strings.Length == 0) yield break;
				var indices = GetSortedIndices();
				var source = Strings[indices[0]];
				var starts = new List<int>() { source.Start };
				for (var i = 1; i < indices.Length; i++)
				{
					if (source != Strings[indices[i]])
					{
						yield return new StringSet(source, starts);
						source = Strings[indices[i]];
						starts.Clear();
						continue;
					}
					else starts.Add(indices[i]);
				}
				if (starts.Count != 0)
					yield return new StringSet(source, starts);
			}
		}

		public IEnumerable<StringSetView> StringSetViews => StringSets.Select(x => new StringSetView(x, DataSource));

		public IEnumerable<StringView> StringViews       => GetIndices()      .Select(i => new StringView(i, DataSource));
		public IEnumerable<StringView> SortedStringViews => GetSortedIndices().Select(i => new StringView(i, DataSource));

		void Load(string fileName = DefaultStateFileName)
		{
			try
			{
				using var f = new BinaryReader(File.OpenRead(fileName));
				for (var i = 0; i < Strings.Length; i++) Strings[i].Start = f.ReadInt32();
				for (var i = 0; i < Strings.Length; i++) Strings[i].Length = f.ReadInt32();
			}
			catch (FileNotFoundException) { return; }
		}

		void Save(string fileName = DefaultStateFileName)
		{
			using var f = new BinaryWriter(File.OpenWrite(fileName));
			for (var i = 0; i < Strings.Length; i++) f.Write(Strings[i].Start);
			for (var i = 0; i < Strings.Length; i++) f.Write(Strings[i].Length);
		}

		public override string ToString() => $"Lenth = {Strings.Max(x => x.Length):x}";

		static void Main(string[] args) => new App(File.ReadAllBytes(args[0])).Run();
	}
}