using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LCS
{
	class ReMapDictionary : ConcurrentDictionary<ReMapKey, int>
	{
		public bool TryAdd(int source, byte ending, int index, out int start)
		{
			var key = new ReMapKey(source, ending);
			start = GetOrAdd(key, index);
			return start == index;
		}
	}

	class ReMapDictionaryST : Dictionary<ReMapKey, int>
	{
		public bool TryAdd(int source, byte ending, int index, out int start)
		{
			var key = new ReMapKey(source, ending);
			if (TryGetValue(key, out start)) return false;
			else Add(key, start = index);
			return true;
		}
	}
}
