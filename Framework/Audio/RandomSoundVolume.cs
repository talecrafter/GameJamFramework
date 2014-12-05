using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class RandomSoundVolume : MonoBehaviour
	{
		public AudioSource source;

		public float intervalMin = 5.0f;
		public float intervalMax = 10.0f;

		public bool fadeInFadeOut = true;

		bool _active = true;
		FadingTimer _timer;

		void Awake()
		{
			if (source == null)
				source = GetComponent<AudioSource>();

			source.loop = true;
			if (!source.isPlaying)
				source.Play();

			StartNewTimer();
		}

		void Update()
		{
			if (_timer.hasEnded)
			{
				_active = !_active;
				source.mute = !_active;
				StartNewTimer();
			}

			if (_active)
			{
				if (fadeInFadeOut)
					source.volume = _timer.progress;
				else
					source.volume = 1.0f;
			}

			_timer.Update();
		}

		void StartNewTimer()
		{
			_timer = new FadingTimer(intervalMin * 0.2f, Random.Range(intervalMin, intervalMax), intervalMin * 0.2f);
		}
	}
}