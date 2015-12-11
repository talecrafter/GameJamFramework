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

		public float timeElapsedProgress
		{
			get
			{
				if (_elapsedTime > _fullDuration)
					return 1f;

				return _elapsedTime / _fullDuration;
			}
		}

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

			UpdateTime();
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

		public bool IsInFadeIn
		{
			get
			{
				return !hasEnded && _elapsedTime < _fadeInTime;
            }
		}

		public void Update()
		{
			_elapsedTime += Time.deltaTime;
			UpdateTime();
		}

		public void Reset()
		{
			_timeHasEnded = false;
			_elapsedTime = 0;

			UpdateTime();
		}

		public void Stop()
		{
			_timeHasEnded = true;

			if (_fadeOutTime > 0)
			{
				progress = 0;
			}
			else
			{
				progress = 1f;
			}
		}

		public void SetToFadedInPoint()
		{
			_timeHasEnded = false;
			_elapsedTime = _fadeInTime;
			UpdateTime();
		}

		public void SetDuration(float duration)
		{
			_fullDuration = duration;
        }

		// ======================================================================
		//	private methods
		// ----------------------------------------------------------------------

		private void UpdateTime()
		{
			if (_elapsedTime > _fullDuration && !_holdAfterFadeIn)
			{
				_timeHasEnded = true;
				return;
			}

			// calculate percent
			progress = 1f;
			if (_elapsedTime < _fadeInTime)
			{
				progress = _elapsedTime / _fadeInTime;
			}
			if (!_holdAfterFadeIn && _elapsedTime > _fullDuration - _fadeOutTime)
			{
				progress = 1f - (_elapsedTime - (_fullDuration - _fadeOutTime)) / _fadeOutTime;
			}

			// clamp values
			if (progress < 0)
			{
				progress = 0;
			}
			if (progress > 1f)
			{
				progress = 1f;
			}
		}
	}
}