using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
    public class GameObjectPool
    {
        private GameObject _prefab;

        private Stack<IPooledObject> _pool = new Stack<IPooledObject>();
        private List<IPooledObject> _activeObjects = new List<IPooledObject>();

        // ================================================================================
        //  constructor
        // --------------------------------------------------------------------------------

        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
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

                newObject.isInactiveInObjectPool = true;
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

            newObject.isDisabled += TogglableObjectIsDisabled;

            return newObject;
        }

        // return an Object to the pool
        public void Push(IPooledObject item)
        {
            item.ToggleOff();

            //if (item is MonoBehaviour)
            //    (item as MonoBehaviour).transform.position = new Vector3(-1000.0f, -1000.0f, -1000.0f);

            item.isDisabled -= TogglableObjectIsDisabled;

            if (_activeObjects.Contains(item))
                _activeObjects.Remove(item);

            _pool.Push(item);
        }

        // ================================================================================
        //  private methods
        // --------------------------------------------------------------------------------

        private IPooledObject CreateNewObject(Vector3? pos = null)
        {
            GameObject newObject = GameObjectFactory.GameObject(_prefab, position:pos);
            IPooledObject instance = newObject.transform.GetInterface<IPooledObject>();

            if (instance == null)
            {
                Debug.LogError("Object " + _prefab + " has no Type " + typeof(IPooledObject));
            }

            return instance;
        }

        private void TogglableObjectIsDisabled(IPooledObject obj)
        {
            Push(obj);
        }
    }

}