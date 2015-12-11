using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
    public class GameObjectPool
    {
        private MonoBehaviour _prefab;

		private Transform _parent = null;

        private Stack<IPooledObject> _pool = new Stack<IPooledObject>();
        private List<IPooledObject> _activeObjects = new List<IPooledObject>();

        // ================================================================================
        //  constructor
        // --------------------------------------------------------------------------------

		public GameObjectPool(MonoBehaviour prefab, Transform parent = null)
		{
			_prefab = prefab;
			_parent = parent;
		}

        // ================================================================================
        //  public methods
        // --------------------------------------------------------------------------------

        // get an Object from the pool
        public IPooledObject Pop(Vector3? pos = null)
        {
            IPooledObject newObject;

            if (_pool.Count == 0)
            {
                newObject = CreateNewObject(pos);

                if (newObject == null)
                    return default(IPooledObject);

                newObject.isUsedByObjectPool = true;
            }
            else
            {
                newObject = _pool.Pop();
                newObject.ToggleOn();

                // set position
                if (newObject is MonoBehaviour && pos != null)
                    (newObject as MonoBehaviour).transform.position = pos.Value;
            }

            _activeObjects.Add(newObject);

            newObject.getsDisabled += ObjectGetsDisabled;

            return newObject;
        }

        // return an Object to the pool
        public void Push(IPooledObject item)
        {
            item.ToggleOff();

            //if (item is MonoBehaviour)
            //    (item as MonoBehaviour).transform.position = new Vector3(-1000.0f, -1000.0f, -1000.0f);

            item.getsDisabled -= ObjectGetsDisabled;

            if (_activeObjects.Contains(item))
                _activeObjects.Remove(item);

            _pool.Push(item);
        }

		public void Clear()
		{
			_activeObjects.Clear();
			_pool.Clear();
		}

        // ================================================================================
        //  private methods
        // --------------------------------------------------------------------------------

        private IPooledObject CreateNewObject(Vector3? pos = null)
        {
			if (_prefab == null)
			{
				Debug.LogError("ObjectPool has no prefab");
				return null;
			}

			MonoBehaviour newObject = GameObjectFactory.Instantiate(_prefab, position: pos);

			if (_parent != null)
				newObject.transform.SetParent(_parent);

			IPooledObject instance = newObject as IPooledObject;

            if (instance == null)
            {
                Debug.LogError("Object " + _prefab + " is not of type " + typeof(IPooledObject));
            }

            return instance;
        }

        private void ObjectGetsDisabled(IPooledObject obj)
        {
            Push(obj);
        }
    }

}