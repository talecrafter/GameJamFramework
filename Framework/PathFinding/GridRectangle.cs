using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CraftingLegends.Framework;

namespace CraftingLegends.Framework
{
	public class GridRectangle
	{
		private GridPosition min;
		private GridPosition max;

		private int width;
		private int height;

		public GridRectangle(GridPosition posOne, GridPosition posTwo)
		{
			int minCol = Mathf.Min(posOne.column, posTwo.column);
			int minRow = Mathf.Min(posOne.row, posTwo.row);

			int maxCol = Mathf.Max(posOne.column, posTwo.column);
			int maxRow = Mathf.Max(posOne.row, posTwo.row);

			min = new GridPosition(minCol, minRow);
			max = new GridPosition(maxCol, maxRow);
		}

		public bool Contains(GridPosition pos)
		{
			return pos.column >= min.column
				&& pos.column <= max.column
				&& pos.row >= min.row
				&& pos.row <= max.row;
		}
	}
}