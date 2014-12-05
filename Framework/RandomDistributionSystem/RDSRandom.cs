using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace rds
{
	/// <summary>
	/// RDS Randomizer implementation. This is a static class and should replace the use of .net's Random class.
	/// It is a bit slower than random due to the fact, that it uses RNGCryptoServiceProvider to produce more real random numbers,
	/// but the benefit of this is a far less predictable sequence of numbers.
	/// You may replace the randomizer used by calling the SetRandomizer method with any object derived from Random.
	/// Supply NULL to SetRandomizer to reset it to the default RNGCryptoServiceProvider.
	/// </summary>
	public static class RDSRandom
	{
		#region CRYPTOGRAPHIC IMPLEMENTATION (DISABLED CODE - FOR FUTURE USE - DID NOT WANT TO DELETE IT)
		//private RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
		//private byte[] mbuffer = new byte[4];

		///// <summary>
		///// Retrieves the next random value from the random number generator.
		///// The result is always between 0.0 and the given max-value (excluding).
		///// </summary>
		///// <param name="max">The maximum value.</param>
		///// <returns></returns>
		//public double GetValue(double max)
		//{
		//    rnd.GetBytes(mbuffer);
		//    UInt32 rand = BitConverter.ToUInt32(mbuffer, 0);
		//    double dbl = rand / (1.0 + UInt32.MaxValue);
		//    return dbl * max;
		//}
		#endregion

		#region TYPE INITIALIZER
		private static Random rnd = null;

		static RDSRandom()
		{
			SetRandomizer(null);
		}
		#endregion

		#region SETRANDOMIZER METHOD
		/// <summary>
		/// You may replace the randomizer used by calling the SetRandomizer method with any object derived from Random.
		/// Supply NULL to SetRandomizer to reset it to the default RNGCryptoServiceProvider.
		/// </summary>
		/// <param name="randomizer">The randomizer to use.</param>
		public static void SetRandomizer(Random randomizer)
		{
			if (randomizer == null)
				rnd = new Random();
			else
				rnd = randomizer;
		}
		#endregion

		#region DOUBLE VALUES
		/// <summary>
		/// Retrieves the next random value from the random number generator.
		/// The result is always between 0.0 and the given max-value (excluding).
		/// </summary>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random double value</returns>
		public static double GetDoubleValue(double max)
		{
			return rnd.NextDouble() * max;
		}

		/// <summary>
		/// Retrieves the next double random value from the random number generator.
		/// The result is always between the given min-value (including) and the given max-value (excluding).
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random double value</returns>
		public static double GetDoubleValue(double min, double max)
		{
			return min + rnd.NextDouble() * (max - min);
		}
		#endregion

		#region INTEGER VALUES
		/// <summary>
		/// Retrieves the next integer random value from the random number generator.
		/// The result is always between 0 (including) and the given max-value (excluding).
		/// </summary>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random integer value</returns>
		public static int GetIntValue(int max)
		{
			return rnd.Next(max);
		}

		/// <summary>
		/// Retrieves the next integer random value from the random number generator.
		/// The result is always between the given min-value (including) and the given max-value (excluding).
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random integer value</returns>
		public static int GetIntValue(int min, int max)
		{
			return rnd.Next(min, max);
		}
		#endregion

		#region ROLLDICE METHOD
		/// <summary>
		/// Simulates a dice roll with a given number of dice and a given number of sides per dice.
		/// The result is an IEnumberable of integers, where the first element (index 0) contains the sum
		/// of all dice rolled and all subsequent elements are the numbers rolled.
		/// </summary>
		/// <param name="dicecount">The dice count.</param>
		/// <param name="sidesperdice">The sides per dice.</param>
		/// <returns>An IEnumberable of integers, where the first element (index 0) contains the sum
		/// of all dice rolled and all subsequent elements are the numbers rolled.</returns>
		public static IEnumerable<int> RollDice(int dicecount, int sidesperdice)
		{
			List<int> rv = new List<int>();
			for (int i = 0; i < dicecount; i++)
				rv.Add(GetIntValue(1, sidesperdice + 1));
			rv.Insert(0, rv.Sum());
			return rv;
		}
		#endregion

		#region ISPERCENTHIT METHOD
		/// <summary>
		/// Determines whether a given percent chance is hit.
		/// Example: If you have a 3.5% chance of something happening, use this method
		/// as "if (IsPercentHit(0.035)) ...".
		/// </summary>
		/// <param name="percent">The percent. Value must be between 0.00 and 1.00. 
		/// Negative values will always result in a false return.</param>
		/// <returns>
		///   <c>true</c> if [is percent hit] [the specified percent]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPercentHit(double percent)
		{
			return (rnd.NextDouble() < percent);
		}
		#endregion
	}
}
