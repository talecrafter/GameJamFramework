using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// Simpler Timer, which has to be updated from outside
	/// </summary>
	public class Timer
	{

		// ================================================================================
		//  public
		// --------------------------------------------------------------------------------

		public bool hasEnded
		{
			get
			{
				return _elapsedTime >= _duration;
			}
		}

		public float progress
		{
			get
			{
				float p = _elapsedTime / _duration;

				if (p < 0)
					p = 0;
				if (p > 1.0f)
					p = 1.0f;

				return p;
			}
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private float _duration = 3.0f;
		private float _elapsedTime = 0;

		// ================================================================================
		//  constructor
		// --------------------------------------------------------------------------------

		public Timer(float duration)
		{
			_duration = duration;
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public void Update()
		{
			_elapsedTime += Time.deltaTime;
		}

		public void Reset()
		{
			_elapsedTime = 0;
		}

		public void SetRandomStart(float progress = 1.0f)
		{
			_elapsedTime = Random.Range(0, _duration * progress);
		}

		/// <summary>
		/// builds a string of the remaining time like "2:03"
		/// </summary>
		/// <returns></returns>
		public string ToCountdownString()
		{
			int remaining = Mathf.CeilToInt(_duration - _elapsedTime);

			if (remaining < 0)
			{
				return "0:00";
			}

			int minutes = remaining / 60;
			int seconds = remaining % 60;

			string timeString = minutes.ToString() + ":";
			if (seconds < 10)
				timeString += "0";
			timeString += seconds.ToString();

			return timeString;
		}
	}
}