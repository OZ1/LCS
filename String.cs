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

		public int CompareTo(String other)
		{
			int c;
			if ((c = other.Length - Length) != 0) return c;
			return Start - other.Start;
		}

		public static bool operator==(String x, String y) =>  x.Equals(y);
		public static bool operator!=(String x, String y) => !x.Equals(y);

		public bool Equals(String other) => Start == other.Start && Length == other.Length;
		public override bool Equals(object obj) => obj is String s && Equals(s);
		public override int GetHashCode() => Start ^ Length << 16 ^ Length >> 16;
		public override string ToString() => Length >  1 ? $"[{Start:x};{End:x}) x {Length:x}" :
		                                     Length == 1 ? $"[{Start:x}]" :
		                                                   $"[{Start:x})";
	}
}
