using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CraftingLegends.Core
{
	public static class TransformExtensions
	{
		public static List<Transform> GetChildren(this Transform transform)
		{
			List<Transform> children = new List<Transform>();
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				children.Add(transform.GetChild(i));
			}
			return children;
		}

		public static void DestroyChildren<T>(this Transform root) where T : MonoBehaviour
		{
			var objects = root.GetComponentsInChildren<T>();
			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i] == null)
					continue;

				if (objects[i].transform == root)
					continue;

				if (Application.isPlaying)
				{
					GameObject.Destroy(objects[i].gameObject);
				}
				else
				{
					GameObject.DestroyImmediate(objects[i].gameObject);
				}
			}
		}

		public static void DestroyChildren(this Transform root)
		{
			if (Application.isPlaying)
			{
				int childCount = root.childCount;
				for (int i = 0; i < childCount; i++)
				{
					GameObject.Destroy(root.GetChild(i).gameObject);
				}
			}
			else
			{
				root.DestroyChildrenDuringEditTime();
			}
		}

		public static void DestroyChildrenDuringEditTime(this Transform root)
		{
			int childCount = root.childCount;
			List<Transform> children = new List<Transform>();

			for (int i = 0; i < childCount; i++)
			{
				children.Add(root.GetChild(i));
			}

			foreach (var item in children)
			{
				GameObject.DestroyImmediate(item.gameObject);
			}
		}

		public static void AddComponentToChildren<T>(this Transform transform) where T : Component
		{
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.AddComponent<T>();
			}
		}

		public static void EnableRendererInChildren<T>(this Transform transform, bool enabled) where T : Renderer
		{
			T[] componentsInChildren = transform.GetComponentsInChildren<T>();
			foreach (var component in componentsInChildren)
			{
				component.enabled = enabled;
			}
		}

		public static void EnableComponentsInChildren<T>(this Transform transform, bool enabled) where T : Behaviour
		{
			T[] componentsInChildren = transform.GetComponentsInChildren<T>();
			foreach (var component in componentsInChildren)
			{
				component.enabled = enabled;
			}
		}

		public static void EnableColliderInChildren<T>(this Transform transform, bool enabled) where T : Collider
		{
			T[] componentsInChildren = transform.GetComponentsInChildren<T>();
			foreach (var component in componentsInChildren)
			{
				component.enabled = enabled;
			}
		}

		#region positions

		public static Vector3 GetCenterWeighted(this List<Transform> transforms)
		{
			Vector3 sum = Vector3.zero;
			int count = 0;

			if (transforms == null || transforms.Count == 0)
			{
				return sum;
			}

			for (int i = 0; i < transforms.Count; i++)
			{
				if (transforms[i] != null)
				{
					sum += transforms[i].position;
					count++;
				}
			}

			return sum / count;
		}

		public static Vector3 GetCenter(this List<Transform> transforms)
		{
			if (transforms == null || transforms.Count == 0 || transforms[0] == null)
			{
				return Vector3.zero;
            }

			Vector3 min = transforms[0].position;
			Vector3 max = transforms[0].position;

			for (int i = 1; i < transforms.Count; i++)
			{
				if (transforms[i] != null)
				{
					Vector3 pos = transforms[i].position;

					if (pos.x < min.x)
						min.x = pos.x;
					if (pos.x > max.x)
						max.x = pos.x;

					if (pos.y < min.y)
						min.y = pos.y;
					if (pos.y > max.y)
						max.y = pos.y;

					if (pos.z < min.z)
						min.z = pos.z;
					if (pos.z > max.z)
						max.z = pos.z;
				}
			}

			return Vector3.Lerp(min, max, 0.5f);
		}

		#endregion

		#region Instantiate Objects

		public static T InstantiateChild<T>(this Transform transform, T prefab, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null) where T : Component
		{
			Vector3 pos = position ?? transform.position;
			Quaternion rot = rotation ?? Quaternion.identity;

			T newObject = GameObject.Instantiate(prefab, pos, rot) as T;
			if (newObject != null)
			{
				newObject.name = prefab.name;

				if (scale.HasValue)
					newObject.transform.localScale = scale.Value;

				newObject.transform.SetParent(transform);
			}

			return newObject;
		}

		public static GameObject InstantiateChildGameObject(this Transform transform, GameObject prefab, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
		{
			Vector3 pos = position ?? transform.position;
			Quaternion rot = rotation ?? Quaternion.identity;

			GameObject newObject = GameObject.Instantiate(prefab, pos, rot) as GameObject;
			if (newObject != null)
			{
				newObject.name = prefab.name;

				if (scale.HasValue)
					newObject.transform.localScale = scale.Value;

				newObject.transform.parent = transform;
			}

			return newObject;
		}

		#endregion

		#region GetInterfaces

		/*
		 * Multiple Versions of GetInterface<T>
		 * Could be used for other types than Interface but we cannot use 'isInterface' or 'getInterfaces' on the type
		 * because Windows Store does not support those
		 */

		public static T[] GetInterfaces<T>(this Transform transform)
		{
			var listOfComponents = transform.GetComponents<MonoBehaviour>();

			return (from component in listOfComponents where component is T select (T)(object)component).ToArray();
		}

		public static T GetInterface<T>(this Transform transform)
		{
			return transform.GetInterfaces<T>().FirstOrDefault();
		}

		public static T[] GetInterfacesInChildren<T>(this Transform transform)
		{
			var listOfComponents = transform.GetComponentsInChildren<MonoBehaviour>();

			return (from component in listOfComponents where component is T select (T)(object)component).ToArray();
		}

		public static T GetInterfaceInChildren<T>(this Transform transform)
		{
			return transform.GetInterfacesInChildren<T>().FirstOrDefault();
		}

		public static T[] GetInterfacesInParentAndChildren<T>(this Transform transform)
		{
			if (transform.parent != null)
				return transform.parent.GetInterfacesInChildren<T>();

			return transform.GetInterfacesInChildren<T>();
		}

		public static T GetInterfaceInParentAndChildren<T>(this Transform transform)
		{
			return transform.GetInterfacesInParentAndChildren<T>().FirstOrDefault();
		}

		public static T[] GetInterfacesInRootAndChildren<T>(this Transform transform)
		{
			return transform.root.GetInterfacesInChildren<T>();
		}

		public static T GetInterfaceInRootAndChildren<T>(this Transform transform)
		{
			return transform.GetInterfacesInRootAndChildren<T>().FirstOrDefault();
		}

		#endregion
	}

}