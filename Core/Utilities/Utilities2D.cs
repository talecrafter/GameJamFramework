using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
	public static class Utilities2D
	{
		public static Rect CameraBounds2D()
		{
			Rect rect = new Rect();

			float verticalDistance = Camera.main.orthographicSize;
			float horizontalDistance = Camera.main.orthographicSize * Camera.main.aspect;

			Vector3 pos = Camera.main.transform.position;
			rect.xMin = pos.x - horizontalDistance;
			rect.xMax = pos.x + horizontalDistance;
			rect.yMin = pos.y - verticalDistance;
			rect.yMax = pos.y + verticalDistance;

			return rect;
		}

		public static Rect CalculateBounds(Vector2[] vertices)
		{
			if (vertices.Length == 0)
				return default(Rect);

			Vector2 min = vertices[0];
			Vector2 max = vertices[0];

			for (int i = 1; i < vertices.Length; i++)
			{
				Vector2 current = vertices[i];

				if (current.x < min.x)
					min.x = current.x;
				if (current.y < min.y)
					min.y = current.y;

				if (current.x > max.x)
					max.x = current.x;
				if (current.y > max.y)
					max.y = current.y;
			}

			return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
		}

		public static Rect GetRectFromTwoPoints(Vector2 start, Vector2 end)
		{
			float xMin = Mathf.Min(start.x, end.x);
			float yMin = Mathf.Min(start.y, end.y);
			float xMax = Mathf.Max(start.x, end.x);
			float yMax = Mathf.Max(start.y, end.y);

			Rect rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
			return rect;
		}

		// gets hit from mouse pointer on given layers
		public static Transform GetHitFromPointerOnLayers(params string[] layerNames)
		{
			int mask = Utilities.LayerMask(layerNames);
			return GetHitFromPointer(mask);
		}

		public static Transform GetHitFromPointer(int layerMask = int.MaxValue)
		{
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0, layerMask);
			return hit.transform;
		}

		public static Vector2 GetNormalizedDirection(Vector2 fromPos, Vector2 toPos)
		{
			Vector2 direction = toPos - fromPos;
			return direction.normalized;
		}

		public static float Vector2SqrDistance(Vector2 fromPos, Vector2 toPos)
		{
			return (fromPos - toPos).sqrMagnitude;
		}

		public static Quaternion GetRotationFromVectorToVector(Vector2 fromVector, Vector2 toVector)
		{
			Vector3 r = new Vector3(0, 0, Mathf.Atan2((toVector.y - fromVector.y), (toVector.x - fromVector.x)) * Mathf.Rad2Deg);
			return Quaternion.Euler(r);
		}
	}
}