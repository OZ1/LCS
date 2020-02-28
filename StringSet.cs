using System.Collections.Generic;
using System.Linq;

namespace LCS
{
	readonly struct StringSet
	{
		public readonly String Source;
		public readonly int[] Starts;
		public int Length => Source.Length;

		public StringSet(String source, IEnumerable<int> starts)
		{
			Source = source;
			Starts = starts.ToArray();
		}

		public override string ToString() => $"{Source}, Count = {Starts.Length}";
	}
}
