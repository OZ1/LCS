using System;
using System.Collections.Generic;

namespace LCS
{
	class RangeSet : SortedSet<RangeSet.Range>
	{
		public bool TryAddNotOverlapping(int start, int length)
		{
			var r = new Range(start, length);
			var overlap = GetViewBetween(r, r);
			if (overlap.Count != 0) return false;
			Add(r);
			return true;
		}

		public struct Range : IComparable<Range>
		{
			public int Start;
			public int End;
			public int Length
			{
				get => End - Start;
				set => End = Start + value;
			}

			public Range(int start, int length) 
			{
				Start = start;
				End = start + length;
			}

			public int CompareTo(Range other)
			{
				if (End <= other.Start) return -1;
				if (other.End <= Start) return +1;
				return 0;
			}

			public static implicit operator Range(int i) => new Range { Start = i, End = i };
		}
	}
}
