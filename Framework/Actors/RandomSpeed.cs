using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Framework
{
	public class RandomSpeed : MonoBehaviour
	{
		[Range(1f, 10f)]
		public float multiplier = 1f;

		void Awake()
		{
			Actor2D actor = GetComponent<Actor2D>();
			actor.movementSpeed += Random.Range(1f, multiplier);
		}
	}
}