using System;
using System.Collections.Generic;
using System.Linq;

namespace LCS
{
	readonly struct StringSet
	{
		public readonly int Index;
		readonly DataSource DS;

		public readonly String Source => DS.Strings[Index];
		public readonly int    Length => DS.Strings[Index].Length;
		public readonly int    End    => Index + Length;

		public readonly IEnumerable<int> Starts
		{
			get
			{
				var source = DS.Strings[Index];
				yield return source.Start;

				var i = Array.BinarySearch(DS.ByStart, Index, DS.StartLengthIndexComparer); // byStart[i] == index

				for (var j = i - 1; j > 0; j--)
				{
					var another = DS.ByStart[j];
					if (DS.Strings[another].Start != source.Start) break;
					yield return another;
				}

				yield return DS.ByStart[i];

				for (var j = i + 1; j < DS.ByStart.Length; j++)
				{
					var another = DS.ByStart[j];
					if (DS.Strings[another].Start != source.Start) break;
					if (DS.Strings[another].Length < source.Length) break;
					yield return another;
				}
			}
		}

		public readonly IEnumerable<StringView> Strings
		{
			get
			{
				var ds = DS;
				var length = Length;
				return Starts.Select(i => new StringView(i, length, ds));
			}
		}

		public StringSet(int index, DataSource ds)
		{
			Index = index;
			DS = ds ?? throw new ArgumentNullException(nameof(ds));
		}

		public override readonly string ToString() => Strings.First().ToString();
	}
}
