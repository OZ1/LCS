using System;

namespace LCS
{
	class DataSource
	{
		public readonly String[] Strings;
		public readonly byte[] Data;

		public DataSource(String[] strings, byte[] data)
		{
			Strings = strings ?? throw new ArgumentNullException(nameof(strings));
			Data = data ?? throw new ArgumentNullException(nameof(data));
		}
	}
}
