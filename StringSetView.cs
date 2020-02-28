using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCS
{
	readonly struct StringSetView
	{
		public readonly StringSet StringSet;
		public readonly DataSource DS;

		public StringSetView(StringSet stringSet, DataSource ds)
		{
			StringSet = stringSet;
			DS = ds ?? throw new ArgumentNullException(nameof(ds));
		}

		public IEnumerable<StringView> Strings
		{
			get
			{
				var ds = DS;
				return StringSet.Starts.Select(x => new StringView(x, ds));
			}
		}

		public override string ToString()
		{
			return new StringView(StringSet.Source.Start, DS).ToString();
		}
	}
}
