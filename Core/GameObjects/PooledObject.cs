using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
	public class PooledObject : MonoBehaviour, IPooledObject
	{
		private bool _isActive = true;
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

		private bool _isUseByObjectPool = false;
		public bool isUsedByObjectPool
		{
			get
            {
				return _isUseByObjectPool;
			}
			set
			{
				_isUseByObjectPool = value;
			}
		}

		public void NotifyDisabled()
		{
			if (isUsedByObjectPool)
			{
				if (isDisabled != null)
					isDisabled(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}