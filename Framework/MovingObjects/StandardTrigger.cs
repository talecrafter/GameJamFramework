using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Framework
{
	public class StandardTrigger<T> : MonoBehaviour
	{
		public event System.Action<T> triggerWasEntered;

		public void OnTriggerEnter2D(Collider2D coll)
		{
			T hit = coll.transform.GetComponent<T>();

			if (hit != null && triggerWasEntered != null)
			{
				triggerWasEntered(hit);
			}
		}
	}
}