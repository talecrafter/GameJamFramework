using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Core
{
	public static class GizmosUtilities
	{
		public static void DrawRect(Rect rect, Color color)
		{
			Gizmos.color = color;

			// top line
			Vector3 firstPos = new Vector3(rect.xMin, rect.yMin, 0f);
			Vector3 secondPos = new Vector3(rect.xMax, rect.yMin, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);

			// bottom line
			firstPos = new Vector3(rect.xMin, rect.yMax, 0f);
			secondPos = new Vector3(rect.xMax, rect.yMax, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);

			// bottom line
			firstPos = new Vector3(rect.xMin, rect.yMin, 0f);
			secondPos = new Vector3(rect.xMin, rect.yMax, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);

			// bottom line
			firstPos = new Vector3(rect.xMax, rect.yMin, 0f);
			secondPos = new Vector3(rect.xMax, rect.yMax, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);
		}
	}
}