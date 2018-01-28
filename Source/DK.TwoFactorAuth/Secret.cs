using System;
using System.Collections.Generic;
using System.Text;

namespace DK.TwoFactorAuth
{
    public class Secret
    {
	    public static String Generate()
		{
			var number = getRandomNumber();

			var seconds = getMillisecondParts();

			return cypher(number.Item1)
				+ cypher(seconds.Item1)
				+ cypher(number.Item2)
				+ cypher(seconds.Item2)
			    + cypher(number.Item3);
		}

		private static Tuple<Int32, Int32, Int32> getRandomNumber()
		{
			var random = new Random();

			return Tuple.Create(
				random.Next(0, Int32.MaxValue),
				random.Next(0, Int32.MaxValue),
				random.Next(0, Int32.MaxValue)
			);
		}

		private static Tuple<Int32, Int32> getMillisecondParts()
		{
			var milliseconds = (
				DateTime.Now - new DateTime(1986, 2, 21)
			).TotalMilliseconds;

			var factor = Math.Pow(10, 7);
			var part1 = (Int32)(milliseconds / factor);
			var part2 = (Int32)(milliseconds % factor);

			return Tuple.Create(part1, part2);
		}

		private const String alphabet =
			"48CD3F6H1JKLMN09QR57UVWXY2#oizeasgtbp";

		private static String cypher(int number)
	    {
		    if (number == 0) return "";
		    var size = alphabet.Length;
			var character = alphabet[number % size];
		    return cypher(number / size) + character;
	    }
    }
}
