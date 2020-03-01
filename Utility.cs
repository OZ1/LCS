using System.Text;

namespace LCS
{
	static class Utility
	{
		public static string FormatRange(int Start, int Length) => Length >  1 ? $"[{Start:x};{Start + Length:x}) x {Length:x}" :
		                                                           Length == 1 ? $"[{Start:x}]" :
		                                                                         $"[{Start:x})";

		public static string FormatData(byte[] data, int start, int length)
		{
			const int Side = 4;

			var to = length;
			if (to == 0) to = Side * 2;
			else
			if (to > Side * 2) to = Side;
			if (to > data.Length) to = data.Length;

			var s  = new StringBuilder();
			for (var i = 0; i < to; i++)
			{
				s.Append(data[start + i].ToString("X2"));
				if (i + 1 < to) s.Append(' ');
			}

			if (to < data.Length)
			{
				if (length > Side * 2 || length == 0)
					s.Append(" ...");

				if (length > Side * 2)
				{
					var end = start + length;
					for (var i = end - Side - 1; i < end; i++)
					{
						s.Append(' ');
						s.Append(data[i].ToString("X2"));
					}
				}
			}

			return s.ToString();
		}
	}
}
