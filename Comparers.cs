using System;
using System.Collections.Generic;

namespace LCS
{
	readonly struct LengthStartIndexComparer : IComparer<int>
	{
		readonly String[] Strings;

		public LengthStartIndexComparer(String[] strings)
		{
			Strings = strings ?? throw new ArgumentNullException(nameof(strings));
		}

		public readonly int Compare(int i, int j)
		{
			int c;
			if ((c = Strings[j].Length - Strings[i].Length) != 0) return c;
			if ((c = Strings[i].Start  - Strings[j].Start ) != 0) return c;
			return i - j;
		}
	}

	readonly struct StartLengthIndexComparer : IComparer<int>
	{
		public readonly String[] Strings;

		public StartLengthIndexComparer(String[] strings)
		{
			Strings = strings ?? throw new ArgumentNullException(nameof(strings));
		}

		public readonly int Compare(int i, int j)
		{
			int c;
			if ((c = Strings[i].Start  - Strings[j].Start ) != 0) return c;
			if ((c = Strings[j].Length - Strings[i].Length) != 0) return c;
			return i - j;
		}
	}
}
