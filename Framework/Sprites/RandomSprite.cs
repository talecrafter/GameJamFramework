using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class RandomSprite : MonoBehaviour
	{
		public List<Sprite> images;

		void Awake()
		{
			GetComponent<SpriteRenderer>().sprite = images.PickRandom();
		}
	}
}