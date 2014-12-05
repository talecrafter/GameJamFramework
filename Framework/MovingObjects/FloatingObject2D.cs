using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class FloatingObject2D : MonoBehaviour
	{
		public Vector2 movement;
		public Transform objectTransform;

		void Awake()
		{
			objectTransform = transform;
		}
	}
}