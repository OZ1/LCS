namespace LCS
{
	static class Utility
	{
		public static string FormatRange(int Start, int Length) => Length >  1 ? $"[{Start:x};{Start + Length:x}) x {Length:x}" :
		                                                           Length == 1 ? $"[{Start:x}]" :
		                                                                         $"[{Start:x})";
	}
}
