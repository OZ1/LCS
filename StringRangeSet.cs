using System.Collections.Generic;

namespace LCS
{
	class StringRangeSet : SortedSet<String>
	{
		public StringRangeSet() : base(new OverlapComparer())
		{
		}

		public bool TryAddNotOverlapping(String s)
		{
			var overlap = GetViewBetween(s, s);
			if (overlap.Count != 0) return false;
			Add(s);
			return true;
		}

		struct OverlapComparer : IComparer<String>
		{
			public int Compare(String x, String y)
			{
				if (x.End <= y.Start) return -1;
				if (y.End <= x.Start) return +1;
				return 0;
			}
		}
	}
}
