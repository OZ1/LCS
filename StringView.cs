using System;
using System.Collections.Generic;
using System.Text;

namespace LCS
{
	readonly struct StringView
	{
		readonly public int Index;
		readonly DataSource DS;

		public int Start  => DS.Strings[Index].Start;
		public int Length => DS.Strings[Index].Length;
		public int End    => DS.Strings[Index].End;
		String     String => DS.Strings[Index];

		public StringView(int i, DataSource ds)
		{
			Index = i;
			DS = ds ?? throw new ArgumentNullException(nameof(ds));
		}

		public StringView Next => new StringView(Start, DS);

		public IEnumerable<StringView> Segments
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

		public IEnumerable<StringView> Others
		{
			get
			{
				for (var i = 0; i < DS.Strings.Length; i++)
					if (DS.Strings[i] == String)
						yield return new StringView(i, DS);
			}
		}

		public IEnumerable<StringView> Prefixes
		{
			get
			{
				for (var i = 0; i < DS.Strings.Length; i++)
					if (DS.Strings[i].Start == Start)
						yield return new StringView(i, DS);
			}
		}

		public override string ToString()
		{
			const int Side = 4;
			var s = new StringBuilder($"[{Index:x};{End:x}) x {Length:x} > {Start:x}");
			if (DS.Data.Length != 0)
			{
				s.Append(" \"");
				var to = Length;
				if (to == 0) to = Side * 2; else
				if (to > Side * 2) to = Side;
				if (to > DS.Data.Length) to = DS.Data.Length;
				for (var i = 0; i < to; i++)
				{
					s.Append(DS.Data[Start + i].ToString("X2"));
					if (i + 1 < to) s.Append(' ');
				}
				if (to < DS.Data.Length)
				{
					if (Length > Side * 2 || Length == 0)
						s.Append(" ...");
					if (Length > Side * 2)
					{
						for (var i = End - Side - 1; i < End; i++)
						{
							s.Append(' ');
							s.Append(DS.Data[i].ToString("X2"));
						}
					}
				}
				s.Append('"');
			}
			return s.ToString();
		}
	}
}
