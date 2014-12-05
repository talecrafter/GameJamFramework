using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class SoundCrossFading : MonoBehaviour
	{
		const float firstMarker = 0.15f;
		const float lastMarker = 1.0f - firstMarker;

		public List<AudioClip> clips;

		public AudioSource firstSource;
		public AudioSource secondSource;

		private AudioClip _current;
		private AudioClip _next;

		private AudioSource _currentSource;
		private AudioSource _nextSource;

		bool _currentHasReachedDeclining = false;
		bool _choosenNextClip = false;

		void Start()
		{
			firstSource.loop = false;
			secondSource.loop = false;

			_currentSource = firstSource;
			_nextSource = secondSource;

			_currentSource.clip = GetNextClip(null);
			_currentSource.Play();

			_nextSource.Stop();
			_nextSource.clip = null;
		}

		public void Update()
		{
			if (_currentSource.isPlaying)
				_currentSource.volume = VolumeFromProgress(_currentSource);
			if (_nextSource.isPlaying)
				_nextSource.volume = VolumeFromProgress(_nextSource);

			if (!_nextSource.isPlaying && !_choosenNextClip)
			{
				_nextSource.clip = GetNextClip(_currentSource.clip);
				_choosenNextClip = true;
			}

			if (!_currentSource.isPlaying)
				SwapSources();

			if (GetProgress(_currentSource) > lastMarker && !_currentHasReachedDeclining)
			{
				_currentHasReachedDeclining = true;
				_nextSource.Play();
			}
		}

		private void PlayNext()
		{
			_nextSource.Play();
		}

		private void SwapSources()
		{
			AudioSource temp = _currentSource;
			_currentSource = _nextSource;
			_nextSource = temp;

			_choosenNextClip = false;
			_currentHasReachedDeclining = false;
		}

		private float VolumeFromProgress(AudioSource source)
		{
			float progress = GetProgress(source);

			if (progress < firstMarker)
			{
				return progress / firstMarker;
			}
			else if (progress > lastMarker)
			{
				return (1.0f - progress) / firstMarker;
			}
			else
			{
				return 1.0f;
			}
		}

		private float GetProgress(AudioSource source)
		{
			if (source != null && source.clip != null)
				return (float)source.timeSamples / (float)source.clip.samples;
			else
				return 1.0f;
		}

		private AudioClip GetNextClip(AudioClip current)
		{
			if (clips.Count <= 1)
				return null;

			AudioClip clip = clips.PickRandom();
			while (clip == current && clip != null)
			{
				clip = clips.PickRandom();
			}

			return clip;
		}
	}
}