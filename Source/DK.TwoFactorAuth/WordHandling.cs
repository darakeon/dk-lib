using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Keon.TwoFactorAuth
{
	internal static class WordHandling
	{
		public static IList<String> BreakByRegex(this String input, [RegexPattern] String pattern)
		{
			return new Regex(pattern)
				.Matches(input)
				.Cast<Match>()
				.Select(m => m.Groups[0].Value)
				.ToList();
		}

		public static String FixBlockSize(this String origin, Int32 blockSize, Char filler)
		{
			var difference = origin.Length % blockSize;
			if (difference == 0) return origin;

			var total = origin.Length + blockSize - difference;

			return origin.PadRight(total, filler);
		}
	}
}
