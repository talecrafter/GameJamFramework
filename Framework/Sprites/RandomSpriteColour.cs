using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Framework
{
	public class RandomSpriteColour : MonoBehaviour
	{
		public Color fromColor;
		public Color toColor;

		public void Awake()
		{
			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
			spriteRenderer.color = Color.Lerp(fromColor, toColor, Random.Range(0, 1f));
		}
	}
}