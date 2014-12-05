using System;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// data class that represents a path through a grid defined by rows and columns
	/// positions are saved internally in reversed order because they are added in that order without known length
	/// used by GridPathController to save the last calculated path without memory allocation
	/// </summary>
	public class GridPath
	{
		private List<GridPosition> _path = new List<GridPosition>();
		public int count = 0;

		public void SetDimensions(int maxColumn, int maxRow)
		{
			int maxSize = (maxColumn + maxRow) * 5;

			for (int i = 0; i < maxSize; i++)
			{
				_path.Add(new GridPosition(0, 0));
			}
		}

		public GridPosition this[int index]
		{
			get
			{
				//if (index < 0 || index >= count)
				//    return null;

				return _path[count - 1 - index];
			}
		}

		public void AddPosition(int column, int row)
		{
			if (count == _path.Count - 1)
			{
				_path.Add(new GridPosition(column, row));
				count++;
				return;
			}

			//// for class
			//GridPosition pos = _path[count];
			//pos.row = row;
			//pos.column = column;

			// for struct
			_path[count] = new GridPosition(column, row);

			count++;
		}

		public void Clear()
		{
			count = 0;
		}
	}
}