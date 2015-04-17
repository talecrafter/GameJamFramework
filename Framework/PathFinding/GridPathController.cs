using System.Collections.Generic;
using UnityEngine;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// A* pathfinding in square based fields
	/// </summary>
	public class GridPathController
	{

		// ======================================================================
		//  events and delegates
		// ----------------------------------------------------------------------

		public delegate bool MovementPossibleCheck(int fromColumn, int fromRow, int column, int row);

		// ======================================================================
		//  public
		// ----------------------------------------------------------------------

		public GridPathReversed gridPath = new GridPathReversed();   // last path calculated

		// ======================================================================
		//  private
		// ----------------------------------------------------------------------

		private int _rowCount = 1;
		private int _columnCount = 1;
		private int _fieldCount = 1;

		private GridPathNode _nearestPoint;
		private int _nearestDistance = 0;
		private int _lastTimeNearestPointPushedForward;

		private GridPathNode[] _grid;
		public GridPathNode[] grid
		{
			get
			{
				return _grid;
			}
		}

		private List<GridPathNode> _openList = new List<GridPathNode>();

		// check values for different directions; standard is four directions
		private int[] _columnCheck = { 0, -1, 1, 0 };
		private int[] _rowCheck = { -1, 0, 0, 1 };
		private int[] _costOfMovement = { 10, 10, 10, 10 };
		private int _checkLength = 4;
		private int _heuristicValue = 1;

		// ======================================================================
		//  constructor
		// ----------------------------------------------------------------------

		public GridPathController()
		{
			// not needed
		}

		// ======================================================================
		//  public methods
		// ----------------------------------------------------------------------

		public void SetDimensions(int columnCount, int rowCount)
		{
			_columnCount = columnCount;
			_rowCount = rowCount;

			// prepare closed list
			_grid = new GridPathNode[rowCount * columnCount];
			//_closedList.Clear();
			for (int row = 0; row < _rowCount; row++)
			{
				//_closedList.Add(new List<GridPathNode>());
				for (int column = 0; column < _columnCount; column++)
				{
					int index = row * _columnCount + column;
					_grid[index] = new GridPathNode(column, row, index);
				}
			}

			_fieldCount = _rowCount * _columnCount;

			gridPath.SetDimensions(_columnCount, _rowCount);
		}

		public void AllowDiagonalMovement(bool allowed)
		{
			if (allowed)
			{
				_columnCheck = new int[] { -1, 1, 1, -1, 0, -1, 1, 0 };
				_rowCheck = new int[] { 1, 1, -1, -1, -1, 0, 0, 1 };
				_costOfMovement = new int[] { 16, 16, 16, 16, 10, 10, 10, 10 };
				_checkLength = 8;
			}
			else
			{
				_columnCheck = new int[] { 0, -1, 1, 0 };
				_rowCheck = new int[] { -1, 0, 0, 1 };
				_costOfMovement = new int[] { 10, 10, 10, 10 };
				_checkLength = 4;
			}
		}

		/// <summary>
		/// will hardcode possible directions of all nodes into the nodes itself
		/// </summary>
		/// <param name="movementPossible"></param>
		public void CalculatePossibleDirections(MovementPossibleCheck movementPossible = null)
		{
			for (int row = 0; row < _rowCount; row++)
			{
				//_closedList.Add(new List<GridPathNode>());
				for (int column = 0; column < _columnCount; column++)
				{
					GridPathNode currentNode = _grid[row * _columnCount + column];

					for (int i = 0; i < _checkLength; i++)
					{
						int checkRow = currentNode.row + _rowCheck[i];
						int checkColumn = currentNode.column + _columnCheck[i];
						int checkField = checkRow * _columnCount + checkColumn;

						// if new position out of bounds, ignore it
						if (checkRow < 0 || checkColumn < 0 || checkRow >= _rowCount || checkColumn >= _columnCount)
							continue;

						GridPathNode checkNode = _grid[checkField];

						// movement not possible => ignore
						if (movementPossible == null)
						{
							if (checkNode.walkable == false)
								continue;
						}
						else
						{
							if (movementPossible(currentNode.column, currentNode.row, checkColumn, checkRow) == false)
								continue;
						}

						currentNode.AddDirection(checkNode, _costOfMovement[i]);
					}
				}
			}
		}

		// calculates path from startPos to endPos
		public bool FindPath(GridPosition startPos, GridPosition endPos, MovementPossibleCheck movementPossible = null, bool saveNearestPath = true, int maxNodeChecksWithoutProgress = 0)
		{
			// assume no path found
			gridPath.Clear();

			if (startPos.row < 0 || startPos.column < 0 || endPos.row < 0 || endPos.column < 0
				|| startPos.column >= _columnCount || startPos.row >= _rowCount || endPos.column >= _columnCount || endPos.row >= _rowCount)
			{
				return false;
			}

			// reset open and closed list
			_openList.Clear();
			for (int field = 0; field < _fieldCount; field++)
			{
				GridPathNode node = _grid[field];
				if (node.closed)
					node.closed = false;
			}

			// set target values
			int targetNodeIndex = endPos.row * _columnCount + endPos.column;
			GridPathNode targetNode = _grid[targetNodeIndex];

			// check if end point is walkable and then decide if using brute force or sophisticated
			// brute force works better when no path will be found and maxNodeChecksWithoutProgress is not set
			bool bruteForce = false;
			bool movementFromStartToEndIsPossible = false;

			if (movementPossible != null)
				movementFromStartToEndIsPossible = movementPossible(startPos.column, startPos.row, endPos.column, endPos.row);
			else
				movementFromStartToEndIsPossible = targetNode.walkable;

			// check if sophisticated or brute force approach
			if (!movementFromStartToEndIsPossible && maxNodeChecksWithoutProgress <= 0)
			{
				// brute force
				_heuristicValue = 1;
				bruteForce = true;
			}
			else
			{
				// sophisticated
				_heuristicValue = 10;
			}

			// set nearest point
			_nearestDistance = int.MaxValue;
			_lastTimeNearestPointPushedForward = 0;

			// add the starting point to the open list
			int startNodeIndex = startPos.row * _columnCount + startPos.column;
			GridPathNode startNode = _grid[startNodeIndex];
			_openList.Add(startNode);
			startNode.h = GetHeuristic(startPos.row, startPos.column, endPos.row, endPos.column);
			startNode.opened = true;
			_nearestPoint = startNode;   // starting point is also nearest point at the moment

			// while not found and still nodes to check
			while (targetNode.closed == false && _openList.Count > 0
			// and while not the maximum of checks without real progress
			&& (maxNodeChecksWithoutProgress <= 0 || _lastTimeNearestPointPushedForward < maxNodeChecksWithoutProgress))
			{
				// first node in the openList will become the current node
				GridPathNode currentNode = _openList[0];

				// save nearest field
				if (currentNode.h < _nearestDistance)
				{
					_nearestDistance = currentNode.h;
					_nearestPoint = currentNode;
					_lastTimeNearestPointPushedForward = 0;
				}
				else
				{
					_lastTimeNearestPointPushedForward++;
				}

				// mark current node as closed
				currentNode.closed = true;
				currentNode.opened = false;
				_openList.RemoveAt(0);

				for (int i = 0; i < currentNode.directionCount; i++)
				{
					GridPathNode.GridMovement gridMovement = currentNode.directions[i];
					GridPathNode checkNode = gridMovement.node;

					// if node is already closed, ignore it
					if (checkNode.closed)
						continue;

					// movement not possible => ignore
					if (movementPossible == null)
					{
						if (checkNode.walkable == false)
							continue;
					}
					else
					{
						if (movementPossible(currentNode.column, currentNode.row, checkNode.column, checkNode.row) == false)
							continue;
					}

					// possible movement cost including path to currentNode
					int g = currentNode.g + gridMovement.movementCost;

					if (checkNode.opened == false)
					{
						checkNode.parentIndex = currentNode.index;

						checkNode.h = GetHeuristic(checkNode.row, checkNode.column, endPos.row, endPos.column);
						checkNode.g = g;
						checkNode.f = checkNode.g + checkNode.h;

						checkNode.opened = true;

						if (bruteForce)
							_openList.Add(checkNode);
						else
							AddToOpenList(checkNode);
					}
					else
					{
						// checkNode is on open list
						// if path to this checkNode is better from the current Node, update 
						if (checkNode.g > g)
						{
							// update parent to the current Node
							checkNode.parentIndex = currentNode.index;

							// update path cost, checkNode.h already calculated
							checkNode.g = g;
							checkNode.f = checkNode.g + checkNode.h;

							// do not bother to move node to correct spot in open list,
							// this way it's faster
						}
					}
				}
			}

			// clear opened values for next search
			int openListCount = _openList.Count;
			for (int i = 0; i < openListCount; i++)
			{
				GridPathNode node = _openList[i];
				node.opened = false;
			}

			// if target found
			if (targetNode.closed)
			{
				CreatePath(startNodeIndex, targetNodeIndex);
				return true;
			}
			// save nearest path if wished
			else if (saveNearestPath)
			{
				CreatePath(startNodeIndex, _nearestPoint.row * _columnCount + _nearestPoint.column);
				return false;
			}
			// no path and no nearest point
			else
			{
				return false;
			}
		}

		private void CreatePath(int startField, int targetField)
		{
			while (startField != targetField && targetField > 0)
			{
				GridPathNode node = _grid[targetField];

				gridPath.AddPosition(node.column, node.row);

				// get next position
				targetField = node.parentIndex;
			}
		}

		private void AddToOpenList(GridPathNode newNode)
		{
			int count = _openList.Count;
			for (int i = 0; i < count; i++)
			{
				if (newNode.f < _openList[i].f)
				{
					_openList.Insert(i, newNode);
					return;
				}
			}

			_openList.Add(newNode);
		}

		// gathers a list of fields that could be reached with the given movementPoints
		//public List<GridTargetField> FindPossibleFields(GridPosition startPos, int movementPoints, MovementPossibleCheck movementPossible)
		//{
		//    List<GridTargetField> targetList = new List<GridTargetField>();

		//    if (startPos.row < 0 || startPos.column < 0  || startPos.column >= _columnCount || startPos.row >= _rowCount)
		//    {
		//        return targetList;
		//    }

		//    List<GridPathNode> openList = new List<GridPathNode>();

		//    List<List<GridPathNode>> closedList = new List<List<GridPathNode>>();
		//    for (int j = 0; j < this._rowCount; j++)
		//    {
		//        closedList.Add(new List<GridPathNode>());
		//        for (int k = 0; k < this._columnCount; k++)
		//        {
		//            closedList[j].Add(null);
		//        }
		//    }

		//    // add the starting point to the open list
		//    openList.Add(new GridPathNode(startPos.column, startPos.row));

		//    int checkRow;
		//    int checkColumn;

		//    while (openList.Count > 0)
		//    {
		//        // Switch it to the closed list
		//        closedList[openList[0].row][openList[0].column] = this.ClonePathNode(openList[0]);
		//        GridPathNode currentNode = closedList[openList[0].row][openList[0].column];

		//        if (currentNode.row != startPos.row || currentNode.column != startPos.column)
		//        {
		//            if (currentNode.g <= movementPoints)
		//            {
		//                targetList.Add(new GridTargetField(new GridPosition(currentNode.column, currentNode.row), currentNode.g));
		//            }
		//        }

		//        // remove from open list
		//        openList.RemoveAt(0);

		//        List<GridPosition> checkList = new List<GridPosition>();
		//        checkList.Add(new GridPosition(0, -1));
		//        checkList.Add(new GridPosition(-1, 0));
		//        checkList.Add(new GridPosition(1, 0));
		//        checkList.Add(new GridPosition(0, 1));

		//        for (int i = 0; i < checkList.Count; i++)
		//        {
		//            checkRow = currentNode.row + checkList[i].row;
		//            checkColumn = currentNode.column + checkList[i].column;

		//            // If it is not walkable or if it is on the closed list, ignore it
		//            if (checkRow < 0 || checkColumn < 0 || checkRow >= _rowCount || checkColumn >= _columnCount)
		//                continue;
		//            if (movementPossible(currentNode.column, currentNode.row, checkColumn, checkRow) == false)
		//                continue;
		//            if (closedList[checkRow][checkColumn] != null)
		//                continue;

		//            GridPathNode onOpenList = null;
		//            int onOpenListPos = 0;
		//            for (int onListCounter = 0; onListCounter < openList.Count; onListCounter++)
		//            {
		//                if (openList[onListCounter].row == checkRow && openList[onListCounter].column == checkColumn)
		//                {
		//                    onOpenList = openList[onListCounter];
		//                    onOpenListPos = onListCounter;
		//                }
		//            }
		//            GridPathNode newNode = new GridPathNode(checkColumn, checkRow);

		//            newNode.parentRow = currentNode.row;
		//            newNode.parentColumn = currentNode.column;

		//            int cost = 1;
		//            newNode.g = currentNode.g + cost;
		//            if (onOpenList == null)
		//            {
		//                openList.Add(newNode);
		//            }
		//            else
		//            {
		//                if (onOpenList.g > newNode.g)
		//                {
		//                    openList.RemoveAt(onOpenListPos);
		//                    openList.Add(newNode);
		//                }
		//            }
		//        }
		//    }

		//    return targetList;
		//}

		// ======================================================================
		//  private methods
		// ----------------------------------------------------------------------

		// calculates heuristic by Manhattan distance
		private int GetHeuristic(int startRow, int startColumn, int targetRow, int targetColumn)
		{
			//// alternative: distance by pythagoras
			//float lengthPowered = Mathf.Pow(UnityEngine.Mathf.Abs(startRow - targetRow), 2) + Mathf.Pow(UnityEngine.Mathf.Abs(startColumn - targetColumn), 2);
			//return (int)Mathf.Sqrt(lengthPowered) * _heuristicValue;

			return (UnityEngine.Mathf.Abs(startRow - targetRow) + UnityEngine.Mathf.Abs(startColumn - targetColumn)) * _heuristicValue;
		}
	}
}