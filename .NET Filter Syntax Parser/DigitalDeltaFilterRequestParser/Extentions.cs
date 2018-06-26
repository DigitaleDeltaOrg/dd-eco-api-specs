using System.Collections.Generic;

namespace DigitalDeltaFilterRequestParser
{
	public static class Extentions
	{
		// Checks if a value lies between min and max, inclusive. 
		public static bool IsBetween<T>(this T item, T min, T max)
		{
			return IsBetween(item, min, max, true);
		}

		// Checks if a value lies between min and max, inclusive. 
		public static bool IsBetween<T>(this T item, T min, T max, bool inclusive)
		{
			return inclusive
				? Comparer<T>.Default.Compare(item, min) >= 0 && Comparer<T>.Default.Compare(item, max) <= 0
				: Comparer<T>.Default.Compare(item, min) > 0 && Comparer<T>.Default.Compare(item, max) < 0;
		}

	}
}