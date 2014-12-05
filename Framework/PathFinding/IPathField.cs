using UnityEngine;

namespace CraftingLegends.Framework
{
	public interface IPathField
	{
		void GetPath(Vector2 startPos, Vector2 endPos, Vector2Path vectorPath);
		void UpdateField(Bounds bounds);
	}
}