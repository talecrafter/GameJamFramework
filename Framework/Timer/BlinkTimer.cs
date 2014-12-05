using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// Simple Timer Class, which has to be updated from outside,
	/// has even and odd intervals
	/// </summary>
	public class BlinkTimer
	{
		// ================================================================================
		//  public
		// --------------------------------------------------------------------------------

		public float duration = 3.0f;
		public float timeInterval = 0.4f;  // in seconds

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private float _elapsedTime = 0;

		// ================================================================================
		//  constructor
		// --------------------------------------------------------------------------------

		public BlinkTimer()
		{
			// not needed
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public bool Update()
		{
			_elapsedTime += Time.deltaTime;

			return IsBlinkInterval();
		}

		/// <summary>
		/// check if this is a blinking interval at the moment
		/// </summary>
		/// <returns></returns>
		public bool IsBlinkInterval()
		{
			if ((int)(_elapsedTime / timeInterval) % 2 == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		// check if time has elapsed
		public bool Finished()
		{
			return _elapsedTime >= duration;
		}
	}
}