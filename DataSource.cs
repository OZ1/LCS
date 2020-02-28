using System;

namespace LCS
{
	class DataSource
	{
		public readonly String[] Strings;
		public readonly    int[] ByStart;
		public readonly   byte[] Data;
		public readonly StartLengthIndexComparer StartLengthIndexComparer;

		public DataSource(String[] strings, byte[] data, int[] byStart)
		{
			Strings = strings ?? throw new ArgumentNullException(nameof(strings));
			ByStart = byStart ?? throw new ArgumentNullException(nameof(byStart));
			Data    = data    ?? throw new ArgumentNullException(nameof(data));
			StartLengthIndexComparer = new StartLengthIndexComparer(strings);
		}
	}
}
