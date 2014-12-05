using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	[System.Serializable]
	public class Weapon
	{
		public enum WeaponType
		{
			Melee,
			Artillery,
			Healing
		}

		[SerializeField]
		public AudioClip attackSound;

		[SerializeField]
		public WeaponType type = WeaponType.Melee;

		[SerializeField]
		public float amount;

		[SerializeField]
		public float attackDuration;

		[SerializeField]
		public float range;

		[SerializeField]
		public float precision = 1.0f;

		[SerializeField]
		public float rangeDelay;

		[SerializeField]
		public GameObject shellPrefab;
	}
}