using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
	public class PooledObject : MonoBehaviour, IPooledObject
	{
		private bool _isActive = false;
		public bool isActive { get { return _isActive; } }

		public virtual void ToggleOn()
		{
			gameObject.SetActive(true);
			_isActive = true;
		}

		public virtual void ToggleOff()
		{
			StopAllCoroutines();
			gameObject.SetActive(false);
			_isActive = false;
		}

		public event System.Action<IPooledObject> isDisabled;

		public bool isUsedByObjectPool
		{
			get;
			set;
		}

		public void NotifyDestroy()
		{
			if (isDisabled != null)
				isDisabled(this);
		}
	}
}