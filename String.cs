using System;

namespace LCS
{
	struct String : IEquatable<String>, IComparable<String>
	{
		public int Start;
		public int Length;
		public int End
		{
			get => Start + Length;
			set => Length = value - Start;
		}

		public String(int start, int length)
		{
			Start = start;
			Length = length;
		}

		public readonly int CompareTo(String other)
		{
			int c;
			if ((c = other.Length - Length) != 0) return c;
			return Start - other.Start;
		}

		public static bool operator==(String x, String y) =>  x.Equals(y);
		public static bool operator!=(String x, String y) => !x.Equals(y);

		public readonly bool Equals(String other) => Start == other.Start && Length == other.Length;
		public readonly override bool Equals(object obj) => obj is String s && Equals(s);
		public readonly override int GetHashCode() => Start ^ Length << 16 ^ Length >> 16;
		public readonly override string ToString() => Utility.FormatRange(Start, Length);
	}
}
