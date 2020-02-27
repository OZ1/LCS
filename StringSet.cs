using System.Collections.Generic;

namespace LCS
{
	class StringSet
	{
		public String Source;
		public List<int> Starts = new List<int>();
		public int Length => Source.Length;

		public StringSet(String source)
		{
			Source = source;
		}

		public override string ToString() => $"{Source} {Starts}";
	}
}
