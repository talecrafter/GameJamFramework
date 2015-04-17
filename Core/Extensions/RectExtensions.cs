using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{

	public static class RectExtensions
	{
		public static Vector2 BottomLeft(this Rect rect)
		{
			return new Vector2(rect.xMin, rect.yMin);
		}

		public static Vector2 TopRight(this Rect rect)
		{
			return new Vector2(rect.xMax, rect.yMax);
		}

		public static Rect Expand(this Rect rect, float delta)
		{
			Rect result = rect;
			result.x -= delta;
			result.y -= delta;
			result.width += delta * 2.0f;
			result.height += delta * 2.0f;
			return result;
		}

		public static Vector2 GetRelativePositionFromCoords(this Rect rect, Vector2 relativePosition)
		{
			float x = (relativePosition.x - rect.x) / rect.width;
			float y = (relativePosition.y - rect.y) / rect.height;
			return new Vector2(x, y);
		}

		public static Vector2 GetRelativePositionFromCoords(this Rect rect, Vector3 relativePosition, bool flipY = false)
		{
			float x = (relativePosition.x - rect.x) / rect.width;
			float y = (relativePosition.y - rect.y) / rect.height;

			if (flipY)
				y = 1.0f - y;

			return new Vector2(x, y);
		}

		public static Vector2 GetPositionFromRelativeCoords(this Rect rect, Vector2 relativePosition)
		{
			float x = rect.x + relativePosition.x * rect.width;
			float y = rect.y + relativePosition.y * rect.height;
			return new Vector2(x, y);
		}

		public static Vector2 GetPositionFromRelativeCoords(this Rect rect, Vector3 relativePosition)
		{
			float x = rect.x + relativePosition.x * rect.width;
			float y = rect.y + relativePosition.y * rect.height;
			return new Vector2(x, y);
		}

		/// <summary>
		/// get a rect from 0,0 to 1,1 comparing this rect to the parent
		/// boundaries get cut by the parent rect boundaries
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static Rect ProjectOntoOther(this Rect rect, Rect parent)
		{
			Rect result = new Rect(0, 0, 1.0f, 1.0f);

			if (rect.x < parent.x)
				result.x = 0;
			else if (rect.x > parent.xMax)
				result.x = 1.0f;
			else
				result.x = (rect.x - parent.x) / parent.width;

			if (rect.xMax < parent.x)
				result.xMax = 0;
			else if (rect.xMax > parent.xMax)
				result.xMax = 1.0f;
			else
				result.xMax = (rect.xMax - parent.x) / parent.width;

			if (rect.y < parent.y)
				result.y = 0;
			else if (rect.y > parent.yMax)
				result.y = 1.0f;
			else
				result.y = (rect.y - parent.y) / parent.height;

			if (rect.yMax < parent.y)
				result.yMax = 0;
			else if (rect.yMax > parent.yMax)
				result.yMax = 1.0f;
			else
				result.yMax = (rect.yMax - parent.y) / parent.height;

			return result;
		}

		public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}

		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}

		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}

		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}

		public static Rect Translate(this Rect rect, Vector2 delta)
		{
			Rect result = rect;
			result.x += delta.x;
			result.y += delta.y;
			return result;
		}

		public static Vector2 RandomPoint(this Rect rect)
		{
			float x = Random.Range(rect.xMin, rect.xMax);
			float y = Random.Range(rect.yMin, rect.yMax);
			return new Vector2(x, y);
		}

		public static Vector2 Delta(this Rect rect, Rect otherRect)
		{
			return new Vector2(otherRect.x - rect.x, otherRect.y - rect.y);
		}

		public static bool Intersects(this Rect firstRect, Rect secondRect)
		{
			bool c1 = firstRect.x < secondRect.xMax;
			bool c2 = firstRect.xMax > secondRect.x;
			bool c3 = firstRect.y < secondRect.yMax;
			bool c4 = firstRect.yMax > secondRect.y;

			return c1 && c2 && c3 && c4;
		}
	}

}