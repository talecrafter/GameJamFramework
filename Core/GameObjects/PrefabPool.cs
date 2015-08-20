using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

namespace CraftingLegends.Core
{
	public class PrefabPool
	{
		private Dictionary<MonoBehaviour, GameObjectPool> _prefabPool = new Dictionary<MonoBehaviour, GameObjectPool>();

		// get an Object from the pool
		public IPooledObject Pop(MonoBehaviour prefab, Vector3? pos = null, Transform parent = null)
		{
			if (!_prefabPool.ContainsKey(prefab))
			{
				_prefabPool[prefab] = new GameObjectPool(prefab, parent);
			}

			return _prefabPool[prefab].Pop(pos);
		}

		public void Reset()
		{
			_prefabPool.Clear();
		}
	}
}