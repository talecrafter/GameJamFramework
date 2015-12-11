using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftingLegends.Core
{
    /// <summary>
    /// collection of helper methods for instantiating new Objects
    /// </summary>
    public static class GameObjectFactory
    {
		#region Instantiate Prefabs

		// instantiate a component and its gameoject structure
		public static T Instantiate<T>(T prefab, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, Transform parent = null) where T : Component
		{
			if (prefab == null)
			{
				Debug.LogWarning("Prefab is null");
				return null;
			}

            Vector3 pos = position ?? Vector3.zero;
            Quaternion rot = rotation ?? Quaternion.identity;

			T newObject = UnityEngine.GameObject.Instantiate(prefab, pos, rot) as T;
            if (newObject != null)
            {
                newObject.name = prefab.name;

                if (scale.HasValue)
                    newObject.transform.localScale = scale.Value;
            }

			if (parent != null)
				newObject.transform.SetParent(parent);

            return newObject;
        }

		public static T Clone<T>(T existingObject, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null) where T : Component
		{
			if (existingObject == null)
			{
				Debug.LogWarning("Prefab is null");
				return null;
			}

			return Instantiate<T>(existingObject, position, rotation, scale, existingObject.transform.parent);			
		}

		// instantiate a gameobject
		public static GameObject GameObject(GameObject prefab, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
		{
			Vector3 pos = position ?? Vector3.zero;
			Quaternion rot = rotation ?? Quaternion.identity;

			GameObject newObject = UnityEngine.GameObject.Instantiate(prefab, pos, rot) as GameObject;
			if (newObject != null)
			{
				newObject.name = prefab.name;

				if (scale.HasValue)
					newObject.transform.localScale = scale.Value;
			}
			return newObject;
		}

		public static T InstantiateWithScale<T>(T prefab, Vector3 position, float scale) where T : Component
		{
            return Instantiate<T>(prefab, position: position, scale: new Vector3(scale, scale, scale));
        }

        #endregion
    }
}
