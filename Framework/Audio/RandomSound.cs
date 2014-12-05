using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class RandomSound : MonoBehaviour
	{
		public AudioSource source;

		public float pauseIntervalMin;
		public float pauseIntervalMax;

		Timer _timer = null;

		void Awake()
		{
			if (source == null)
				source = GetComponent<AudioSource>();

			if (source != null)
			{
				source.loop = false;
			}

			StartNewTimer();
		}

		void Update()
		{
			if (source != null && !source.isPlaying)
			{
				if (_timer == null)
					StartNewTimer();
				else
					_timer.Update();

				if (_timer.hasEnded)
				{
					_timer = null;
					source.Play();
				}
			}
		}

		void StartNewTimer()
		{
			_timer = new Timer(Random.Range(pauseIntervalMin, pauseIntervalMax));
		}
	}
}