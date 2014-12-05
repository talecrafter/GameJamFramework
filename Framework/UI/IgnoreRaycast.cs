using UnityEngine;

namespace CraftingLegends.Framework
{
	public class IgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
	{
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}
}
