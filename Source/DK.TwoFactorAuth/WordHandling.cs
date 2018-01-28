using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace DK.TwoFactorAuth
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
	}
}
