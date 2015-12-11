using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class LevelEntry : MonoBehaviour
	{
		public LevelConnection incomingConnection;

		public bool lookToTheRight = true;
		public bool lookDown = true;

		public Vector2 GetLookDirection()
		{
			if (lookToTheRight)
			{
				if (lookDown)
				{
					return new Vector2(1f, -1f);
				}
				else
				{
					return new Vector2(1f, 1f);
				}
			}
			else
			{
				if (lookDown)
				{
					return new Vector2(-1f, -1f);
				}
				else
				{
					return new Vector2(-1f, 1f);
				}
			}
		}
	}
}