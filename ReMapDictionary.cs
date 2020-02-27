using System;
using System.Collections.Generic;

namespace LCS
{
	class ReMapDictionary : Dictionary<ReMapDictionary.ReMapKey, int>
	{
		public bool TryAdd(int source, byte ending, int index, out int start)
		{
			var key = new ReMapKey(source, ending);
			if (TryGetValue(key, out start)) return false;
			else Add(key, start = index);
			return true;
		}

		public struct ReMapKey : IEquatable<ReMapKey>
		{
			public int Source;
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
	}
}
