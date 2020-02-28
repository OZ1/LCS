using System;

namespace LCS
{
	readonly struct ReMapKey : IEquatable<ReMapKey>
	{
		public readonly  int Source;
		public readonly byte Ending;

		public ReMapKey(int source, byte ending)
		{
			Source = source;
			Ending = ending;
		}

		public readonly bool Equals(ReMapKey other) => Source == other.Source && Ending == other.Ending;
		public readonly override bool Equals(object obj) => obj is ReMapKey ending && Equals(ending);
		public readonly override int GetHashCode() => Source << 8 | Ending;
		public readonly override string ToString() => $"{Source:x} [{Ending:X2}]";
	}
}
