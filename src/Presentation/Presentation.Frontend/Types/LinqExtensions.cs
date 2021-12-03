using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Linq
{
	/// Extends the Linq namespace.
	public static class LinqExtensions
	{
		/// Performs given action on all elements of given sequence.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var element in source)
			{
				action(element);
			}
		}
	}
}
