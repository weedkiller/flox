using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace S3.CoreServices.System
{
	public static class EnumerableExtensions
	{
		public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerable<T> src)
		{
			return new ConcurrentBag<T>(src);
		}
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src)
		{
			return new HashSet<T>(src);
		}

		public static int IndexOf<T>(this IEnumerable<T> src,T match)
		{
			return src.Select((x, i) => new {Item = x, index = i})
				.FirstOrDefault(x =>(x.Item==null && match==null)|| x.Item!=null&& x.Item.Equals(match))?.index ?? -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> src,Func<T,bool> match)
		{
			return src.Select((x, i) => new {Item = x, index = i})
				.FirstOrDefault(x=>match(x.Item))?.index ?? -1;
		}
	}
}