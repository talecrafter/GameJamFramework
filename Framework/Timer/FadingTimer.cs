using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CraftingLegends.Framework
{
	public class FadingTimer
	{

		// ================================================================================
		//  public
		// --------------------------------------------------------------------------------

		public float progress;

		// ================================================================================
		//  properties
		// --------------------------------------------------------------------------------

		private float _elapsedTime;

		private bool _holdAfterFadeIn = false;
		public bool holdAfterFadeIn
		{
			get { return _holdAfterFadeIn; }
			set { _holdAfterFadeIn = holdAfterFadeIn; }
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private float _fullDuration;
		private float _fadeInTime;
		private float _fadeOutTime;

		private bool _timeHasEnded = false;

		// ================================================================================
		//  constructor
		// --------------------------------------------------------------------------------

		public FadingTimer(float fadeInTime, float fullDuration, float fadeOutTime = 0)
		{
			_fadeInTime = fadeInTime;
			_fadeOutTime = fadeOutTime;
			_fullDuration = fullDuration;

			updateTime();
		}

		// simplified with fade in only
		public FadingTimer(float fadeInTime) : this(fadeInTime, fadeInTime) { }

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public bool hasEnded
		{
			get
			{
				if (_timeHasEnded && !_holdAfterFadeIn)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public void Update()
		{
			_elapsedTime += Time.deltaTime;
			updateTime();
		}

		public void Reset()
		{
			_timeHasEnded = false;
			_elapsedTime = 0;
			progress = 0;
		}

		public void Stop()
		{
			progress = 1.0f;
			_timeHasEnded = true;
		}

		// ======================================================================
		//	private methods
		// ----------------------------------------------------------------------

		private void updateTime()
		{
			if (_elapsedTime > _fullDuration && !_holdAfterFadeIn)
			{
				_timeHasEnded = true;
				return;
			}

			// calculate percent
			progress = 1.0f;
			if (_elapsedTime < _fadeInTime)
			{
				progress = _elapsedTime / _fadeInTime;
			}
			if (!_holdAfterFadeIn && _elapsedTime > _fullDuration - _fadeOutTime)
			{
				progress = 1 - (_elapsedTime - (_fullDuration - _fadeOutTime)) / _fadeOutTime;
			}
			if (progress < 0)
			{
				progress = 0;
			}
			if (progress > 1.0f)
			{
				progress = 1.0f;
			}
		}
	}
}