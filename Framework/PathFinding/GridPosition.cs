using UnityEngine;
namespace CraftingLegends.Framework
{
	/// <summary>
	/// Position in a square-based map with rows and columns
	/// </summary>
	public struct GridPosition
	{
		/*
		 
		 ^ row
		 |
		 o--> column
		 
		 */

		// ======================================================================
		//  public
		// ----------------------------------------------------------------------

		public int row;
		public int column;

		// ======================================================================
		//  constructor
		// ----------------------------------------------------------------------

		public GridPosition(int column, int row)
		{
			this.row = row;
			this.column = column;
		}

		// ======================================================================
		//  public methods
		// ----------------------------------------------------------------------

		// calculate Manhattan Distance to other position
		public int GetDistance(GridPosition position)
		{
			return UnityEngine.Mathf.Abs(this.column - position.column) + UnityEngine.Mathf.Abs(this.row - position.row);
		}

		// new position from offset row and offset column
		public GridPosition NewPositionFromOffset(int columnOffset, int rowOffset)
		{
			return new GridPosition(column + columnOffset, row + rowOffset);
		}

		// calculate direction from this position to other position
		public GridDirection GetDirection(GridPosition position)
		{
			// top
			if (position.row < this.row)
			{
				return GridDirection.North;
			}
			// right
			if (position.column > this.column)
			{
				return GridDirection.East;
			}
			// bottom
			if (position.row > this.row)
			{
				return GridDirection.South;
			}
			// left
			return GridDirection.West;
		}

		public GridDirection GetBestDirection(GridPosition position)
		{
			GridPosition offset = position - this;
			if (Mathf.Abs(offset.column) > Mathf.Abs(offset.row))
			{
				if (offset.column > 0)
				{
					return GridDirection.East;
				}
				else
				{
					return GridDirection.West;
				}
			}
			else
			{
				if (offset.row > 0)
				{
					return GridDirection.North;
				}
				else
				{
					return GridDirection.South;
				}
			}
		}

		public GridPosition north { get { return GetAdjacentPosition(GridDirection.North); } }
		public GridPosition east { get { return GetAdjacentPosition(GridDirection.East); } }
		public GridPosition west { get { return GetAdjacentPosition(GridDirection.West); } }
		public GridPosition south { get { return GetAdjacentPosition(GridDirection.South); } }

		// get position next to this one
		public GridPosition GetAdjacentPosition(GridDirection direction)
		{
			GridPosition position = new GridPosition(column, row);

			// top
			if (direction == GridDirection.North)
			{
				position.row++;
			}
			// right
			if (direction == GridDirection.East)
			{
				position.column++;
			}
			// bottom
			if (direction == GridDirection.South)
			{
				position.row--;
			}
			// left
			if (direction == GridDirection.West)
			{
				position.column--;
			}

			return position;
		}

		public static GridPosition operator +(GridPosition posOne, GridPosition posTwo)
		{
			return posOne.NewPositionFromOffset(posTwo.column, posTwo.row);
		}

		public static GridPosition operator -(GridPosition posOne, GridPosition posTwo)
		{
			return posOne.NewPositionFromOffset(-posTwo.column, -posTwo.row);
		}

		public static GridPosition operator +(GridPosition pos, GridDirection direction)
		{
			return pos.GetAdjacentPosition(direction);
		}

		public static bool operator ==(GridPosition pos, GridPosition otherPos)
		{
			if (pos.row == otherPos.row && pos.column == otherPos.column)
				return true;
			else
				return false;
		}

		public static bool operator !=(GridPosition pos, GridPosition otherPos)
		{
			if (pos.row != otherPos.row || pos.column != otherPos.column)
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + column.GetHashCode();
			hash = (hash * 7) + row.GetHashCode();
			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is GridPosition)
				return this == (GridPosition)obj;
			return false;
		}

		public override string ToString()
		{
			return "Position: x " + column + " y " + row;
		}

		public bool IsAdjacentOrDiagonal(GridPosition checkPoint)
		{
			int distanceX = UnityEngine.Mathf.Abs(this.column - checkPoint.column);
			int distanceY = UnityEngine.Mathf.Abs(this.row - checkPoint.row);
			if (distanceX <= 1 && distanceY <= 1)
				return true;
			else
				return false;
		}

		public bool IsAdjacent(GridPosition checkPoint)
		{
			int distanceX = UnityEngine.Mathf.Abs(this.column - checkPoint.column);
			int distanceY = UnityEngine.Mathf.Abs(this.row - checkPoint.row);
			return distanceX + distanceY == 1;
		}
	}
}