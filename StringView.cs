using System.Collections.Generic;
using System.Text;

namespace LCS
{
	class StringView
	{
		const int DisplayBytes = 4;

		public int Index;
		public int Start  => Strings[Index].Start;
		public int Length => Strings[Index].Length;
		public int End    => Strings[Index].End;
		String     String => Strings[Index];

		readonly String[] Strings;
		readonly byte[] Data;

		public StringView Next => Index != Start ? new StringView(Start, Strings, Data) : null;

		public IEnumerable<StringView> Segments
		{
			get
			{
				for (var p = Next; p != null; p = p.Next)
					yield return p;
			}
		}

		public IEnumerable<StringView> Others
		{
			get
			{
				for (var i = 0; i < Strings.Length; i++)
					if (Strings[i] == String)
						yield return new StringView(i, Strings, Data);
			}
		}

		public IEnumerable<StringView> Prefixes
		{
			get
			{
				for (var i = 0; i < Strings.Length; i++)
					if (Strings[i].Start == Start)
						yield return new StringView(i, Strings, Data);
			}
		}

		public StringView(int i, String[] strings, byte[] data)
		{
			Index = i;
			Strings = strings;
			Data = data;
		}

		public override string ToString()
		{
			var s = new StringBuilder();
			s.Append($"{Index:x}: ");
			s.Append(String.ToString());

			if (Length > 0)
				s.Append(" \"");

			var i = Start;
			if (Length >= DisplayBytes)
			{
				for (; i < Start + DisplayBytes; i++)
				{
					s.Append(Data[i].ToString("X2"));
					s.Append(' ');
				}
				if (Length > DisplayBytes * 2)
				{
					s.Append("... ");
					i = End - DisplayBytes - 1;
				}
			}
			for (; i < End; i++)
			{
				s.Append(Data[i].ToString("X2"));
				if (i + 1 < End) s.Append(' ');
			}
			if (Length > 0)
				s.Append('"');
			return s.ToString();
		}
	}
}
