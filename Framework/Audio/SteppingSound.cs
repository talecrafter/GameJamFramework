using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class SteppingSound : MonoBehaviour
	{
		public AudioSource source;

		private Actor _actor;

		void Awake()
		{
			if (source == null)
				source = GetComponent<AudioSource>();

			_actor = GetComponent<Actor>();

			source.loop = true;
			source.Stop();
		}

		void Update()
		{
			if (_actor.isMoving)
			{
				if (!source.isPlaying)
				{
					source.Play();
				}
			}
			else
			{
				if (source.isPlaying)
				{
					source.Stop();
				}
			}
		}
	}
}