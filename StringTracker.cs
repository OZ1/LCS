using System.Collections.Generic;

namespace LCS
{
	class StringTracker
	{
		public List this[int i] => Lists[i];

		readonly List[] Lists = new List[2] { new List(), new List() };

		public class List : List<int>
		{
			public void Keep(int i) => Add(i);
			public void Free(int i) { }
		}
	}

	class StringTrackerHashSet : HashSet<int>
	{
		public StringTrackerHashSet this[int i] => this;

		readonly HashSet<int> Removed = new HashSet<int>();

		public void Keep(int i) { }
		public void Free(int i) => Removed.Add(i);
		public new void Clear()
		{
			ExceptWith(Removed);
			Removed.Clear();
		}
	}

	class StringTrackerSortedSet : SortedSet<int>
	{
		public StringTrackerSortedSet this[int i] => this;

		readonly HashSet<int> Removed = new HashSet<int>();

		public void Keep(int i) { }
		public void Free(int i) => Removed.Add(i);
		public new void Clear()
		{
			ExceptWith(Removed);
			Removed.Clear();
		}
	}

	static class StringTrackerExtensions
	{
		public static void Keep(this List<int> t, int i) => t.Add(i);
		public static void Free(this List<int> t, int i) { }
	}
}
