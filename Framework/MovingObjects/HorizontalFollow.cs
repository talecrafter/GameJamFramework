using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class HorizontalFollow : MonoBehaviour
	{
		public Transform target;

		private Transform _transform;

		void Awake()
		{
			_transform = transform;
		}

		void Update()
		{
			if (target != null)
				_transform.position = new Vector3(target.position.x, _transform.position.y, _transform.position.z);
		}

	}
}