using System;
using System.Collections.Generic;
using System.Text;

namespace LCS
{
	readonly struct StringView
	{
		readonly public int Index;
		readonly public int Length;
		readonly DataSource DS;

		readonly String     String => DS.Strings[Index];
		readonly public int Start  => DS.Strings[Index].Start;
		readonly public int End    => Index + Length;

		public StringView(int i, DataSource ds)
		{
			Index = i;
			Length = ds.Strings[Index].Length;
			DS = ds ?? throw new ArgumentNullException(nameof(ds));
		}

		public StringView(int i, int length, DataSource ds)
		{
			Index = i;
			Length = length;
			DS = ds ?? throw new ArgumentNullException(nameof(ds));
		}

		public readonly StringView Next => new StringView(Start, DS);

		public readonly IEnumerable<StringView> Segments
		{
			get
			{
				if (Index == Start) yield break;
				var p = Next;
				do {
					yield return p;
					p = p.Next;
				} while (p.Index != p.Start);
			}
		}

		public readonly IEnumerable<StringView> Others
		{
			get
			{
				for (var i = 0; i < DS.Strings.Length; i++)
					if (DS.Strings[i] == String)
						yield return new StringView(i, DS);
			}
		}

		public readonly IEnumerable<StringView> Prefixes
		{
			get
			{
				for (var i = 0; i < DS.Strings.Length; i++)
					if (DS.Strings[i].Start == Start)
						yield return new StringView(i, DS);
			}
		}

		public override readonly string ToString()
		{
			var s = new StringBuilder();
			s.Append(Utility.FormatRange(Index, Length));
			s.Append($" > {Start:x}");
			if (DS.Data.Length != 0)
			{
				s.Append(' ');
				s.Append('"');
				s.Append(Utility.FormatData(DS.Data, Start, Length));
				s.Append('"');
			}
			return s.ToString();
		}
	}
}
