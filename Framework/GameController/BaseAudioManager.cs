using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	[RequireComponent(typeof(AudioSource))]
	public class BaseAudioManager : MonoBehaviour
	{
		const float PROGRESS_UNTIL_NEXT_PLAY = 0.7f;

		[SerializeField]
		private AudioClip _buttonSound;

		private AudioSource _source;

		private Dictionary<string, Timer> _playedList = new Dictionary<string, Timer>();

		void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		void Update()
		{
			CleanPlayedList();
		}

		public void PlayButtonSound()
		{
			Play(_buttonSound);
		}

		// plays a random command sound
		public void PlayRandomSound(AudioClip[] clips)
		{
			AudioClip commandSound = clips[Random.Range(0, clips.Length)];
			Play(commandSound);
		}

		/// <summary>
		/// plays an Audioclip if it was not played in PROGRESS_UNTIL_NEXT_PLAY of duration
		/// </summary>
		/// <param name="clip"></param>
		public void PlayOnce(AudioClip clip, float volume = 1.0f)
		{
			if (clip == null)
				return;

			string type = clip.name;

			if (!_playedList.ContainsKey(type))
			{
				_playedList[type] = new Timer(clip.length * PROGRESS_UNTIL_NEXT_PLAY);
			}
			else
			{
				if (_playedList[type].hasEnded)
				{
					_playedList[type].Reset();
				}
				else
				{
					return;
				}
			}

			PlayWithVariation(clip, volume);
		}

		public void Play(AudioClip clip, float volume = 1.0f)
		{
			if (clip != null)
			{
				_source.PlayOneShot(clip, _source.volume * volume);
			}
		}

		public void PlayWithVariation(AudioClip clip, float volume = 1.0f)
		{
			_source.pitch = Random.Range(.97f, 1.0f);
			float volumeVariation = Random.Range(.85f, 1f);
			_source.PlayOneShot(clip, _source.volume * volume * volumeVariation);
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		// removes elapsed entries in the playedList
		private void CleanPlayedList()
		{
			List<string> removables = new List<string>();

			// check entries and mark for removal
			foreach (var item in _playedList)
			{
				if (item.Value != null)
				{
					item.Value.Update();
				}

				if (item.Value.hasEnded)
				{
					removables.Add(item.Key);
				}
			}

			// remove items
			foreach (var item in removables)
			{
				_playedList.Remove(item);
			}
		}
	}
}