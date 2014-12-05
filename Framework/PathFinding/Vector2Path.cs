using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftingLegends.Framework
{
	public class Vector2Path
	{
		public List<Vector2> path = new List<Vector2>();

		public int current = 0;
		public int count = 0;
		public bool isFinished = false;

		public bool isValid
		{
			get
			{
				return count > 0;
			}
		}

		public Vector2Path(int allocationSize = 50)
		{
			for (int i = 0; i < allocationSize; i++)
			{
				path.Add(new Vector2(0, 0));
			}
		}

		public Vector2 this[int index]
		{
			get
			{
				return path[index];
			}
		}

		public void AddPosition(float x, float y)
		{
			if (count == path.Count - 1)
			{
				path.Add(new Vector2(x, y));
				count++;
				return;
			}

			path[count] = new Vector2(x, y);

			count++;

			isFinished = false;
		}

		public Vector2 CurrentPosition
		{
			get
			{
				return path[current];
			}
		}

		public void NextPosition()
		{
			current++;

			if (current >= count)
			{
				isFinished = true;
			}
		}

		public void Clear()
		{
			current = 0;
			count = 0;
			isFinished = true;
		}
	}
}