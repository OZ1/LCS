using System;

namespace LCS
{
	struct ReMapKey : IEquatable<ReMapKey>
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
