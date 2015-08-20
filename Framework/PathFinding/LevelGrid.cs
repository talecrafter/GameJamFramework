using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
    public class LevelGrid : MonoBehaviour, IPathField
	{
        // ================================================================================
        //  public
        // --------------------------------------------------------------------------------

        public bool drawDebug = false;

        // ================================================================================
        //  private
        // --------------------------------------------------------------------------------

        // cell size of the grid
        public float gridSize = 0.9f;
		public int maxCheckNodesWithoutProgress = 0;

		// bounds
		protected float _startX;
		protected float _startY;
		protected int _columnCount;
		protected int _rowCount;

		protected GridPathController _pathController = new GridPathController();

		// debug
		protected Vector3 _debugDrawSize;
		protected Vector2 _lastTarget;

		protected Vector2Path _testPath;

        // ================================================================================
        //  unity methods
        // --------------------------------------------------------------------------------

        protected virtual void Start()
        {
            _debugDrawSize = new Vector3(gridSize, gridSize, gridSize);

            CreateGrid(gridSize);

            if (Application.loadedLevelName == "pathfinding_test")
            {
                Test();
            }
        }

        // ================================================================================
        //  public methods
        // --------------------------------------------------------------------------------

        public virtual void GetPath(Vector2 startPos, Vector2 endPos, Vector2Path vectorPath)
        {
            _lastTarget = endPos;

            GridPosition startGridPosition = GetGridPositionFromLevelPosition(startPos);
            GridPosition endGridPosition = GetGridPositionFromLevelPosition(endPos);

            _pathController.FindPath(startGridPosition, endGridPosition, MovementPossibleCheck, true, maxCheckNodesWithoutProgress);

            vectorPath.Clear();
            if (_pathController.gridPath.count > 0)
            {
                for (int i = 0; i < _pathController.gridPath.count; i++)
                {
                    Vector2 p = GetLevelPositionFromGridPosition(_pathController.gridPath[i]);
                    vectorPath.AddPosition(p.x, p.y);
                }
            }

            _testPath = vectorPath;
        }

        public void UpdateField(Bounds bounds)
        {
            int fromColumn = GetColumnFromX(bounds.min.x);
            int toColumn = GetColumnFromX(bounds.max.x);

            int fromRow = GetRowFromY(bounds.min.y);
            int toRow = GetRowFromY(bounds.max.y);

            //Debug.Log("from " + fromColumn + "/" + fromRow + " to " + toColumn + "/" + toRow );

            int layerMask = Utilities.LayerMaskIncludingDefault("Building", "AreaEffects", "Obstacles");

            CheckColliders(_pathController.grid, layerMask, fromColumn: fromColumn, fromRow: fromRow, toColumn: toColumn, toRow: toRow);            
        }

        // ================================================================================
        //  private methods
        // --------------------------------------------------------------------------------

        protected bool MovementPossibleCheck(int fromColumn, int fromRow, int column, int row)
        {
            return _pathController.grid[row * _columnCount + column].walkable;
        }

        private void CreateGrid(float fieldSize)
        {
            // calculate grid bounds
            _startX = MainBase.Instance.levelBounds.x;
            _startY = MainBase.Instance.levelBounds.y;
            _columnCount = (int)Mathf.Ceil(MainBase.Instance.levelBounds.width / fieldSize);
            _rowCount = (int)Mathf.Ceil(MainBase.Instance.levelBounds.height / fieldSize);

            _pathController.AllowDiagonalMovement(true);
            _pathController.SetDimensions(rowCount: _rowCount, columnCount: _columnCount);
            
            // mark all possible collisions as true on the grid
            CheckColliders(_pathController.grid, Utilities.LayerMaskIncludingDefault("Building", "AreaEffects"));
            // hardcode directions
            _pathController.CalculatePossibleDirections();

            // check again and include dynamic objects
            CheckColliders(_pathController.grid, Utilities.LayerMaskIncludingDefault("Building", "AreaEffects", "Obstacles"));
        }

        private void CheckColliders(GridPathNode[] grid, int layerMask)
        {
            CheckColliders(grid, layerMask, 0, 0, _columnCount - 1, _rowCount - 1);
        }

        private void CheckColliders(GridPathNode[] grid, int layerMask, int fromColumn, int fromRow, int toColumn, int toRow)
        {
            // save raycastsHitTriggers setting
            bool beforeSetting = Physics2D.raycastsHitTriggers;
            Physics2D.raycastsHitTriggers = false;

            for (int row = fromRow; row <= toRow; row++)
            {
                for (int column = fromColumn; column <= toColumn; column++)
                {
                    // calculate position
                    float posX = _startX + column * gridSize + gridSize * 0.5f;
                    float posY = _startY + row * gridSize + gridSize * 0.5f;

                    // check for colliders
                    Collider2D coll = Physics2D.OverlapCircle(new Vector2(posX, posY), gridSize * 0.49f, layerMask);
                    if (coll == null)
                        grid[row * _columnCount + column].SetWalkable(true);
                    else
                        grid[row * _columnCount + column].SetWalkable(false);
                }
            }

            // restore raycastsHitTriggers setting
            Physics2D.raycastsHitTriggers = beforeSetting;
        }

		#region test methods
		// ================================================================================
		//  test methods
		// --------------------------------------------------------------------------------

		/// <summary>
		/// Debug Draw, show grid and last searched path
		/// </summary>
		protected void OnDrawGizmos()
        {
            if (!drawDebug || !Application.isPlaying)
                return;

            // draw all grid fields
            for (int row = 0; row < _rowCount; row++)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    if (_pathController.grid[row * _columnCount + column].walkable == false)
                    {
                        Gizmos.color = new Color(1.0f, 0, 0, 0.2f);
                        DrawMarker(row, column);
                    }

                    if (_pathController.grid[row * _columnCount + column].closed)
                    {
                        Gizmos.color = new Color(1.0f, 1.0f, 0, 0.3f);
                        DrawMarker(row, column);
                    }
                }
            }

            Gizmos.color = new Color(0, 1.0f, 0, 0.2f);

            if (_testPath != null)
            {
                for (int i = 0; i < _testPath.count; i++)
                {
                    Vector3 center = new Vector3(_testPath[i].x, _testPath[i].y, 0);
                    Gizmos.DrawCube(center, _debugDrawSize);
                }

                // draw 'last target' marker
                Gizmos.color = new Color(0, 0, 1.0f, 0.3f);
                Vector3 lastCenter = new Vector3(_lastTarget.x, _lastTarget.y, 0);
                Gizmos.DrawCube(lastCenter, _debugDrawSize);
            }
        }

        private void Test()
        {
            _testPath = new Vector2Path((_columnCount + _rowCount) * 5);

            // warmup, just in case
            Utilities.TestMethod(TestPathClosed, "ClosedPath", 50, false);

            float completeTime = 0;
            completeTime += Utilities.TestMethod(TestPathNear, "NearPath");
            completeTime += Utilities.TestMethod(TestPathOpen, "OpenPath");
            completeTime += Utilities.TestMethod(TestPathTypical, "TypicalPath");
            completeTime += Utilities.TestMethod(TestPathOpenReversed, "OpenPathReversed");
            completeTime += Utilities.TestMethod(TestPathMediumLength, "MediumLength");
            completeTime += Utilities.TestMethod(TestPathClosed, "ClosedPath");
            Debug.Log(completeTime.ToString("F5"));
        }

        private void TestPathOpen()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(18.0f, -9.0f), _testPath); // rechts unten
        }

        private void TestPathOpenReversed()
        {
            GetPath(new Vector2(18.0f, -9.0f), new Vector2(-18.0f, 8.0f), _testPath); // rechts unten
        }

        private void TestPathClosed()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(18.0f, -5.0f), _testPath); // rechts, Berge
        }

        private void TestPathTypical()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(18.0f, 8.0f), _testPath); // rechts oben
        }

        private void TestPathNear()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(-14.0f, 8.0f), _testPath); // sehr nah
        }

        private void TestPathMediumLength()
        {
            GetPath(new Vector2(0f, -7.0f), new Vector2(18.0f, -5.0f), _testPath); // rechts, Berge
        }

		#endregion

		#region helper methods
		// ================================================================================
		//  helper methods
		// --------------------------------------------------------------------------------

		protected GridPosition GetGridPositionFromLevelPosition(Vector2 levelPos)
        {
            return new GridPosition(GetColumnFromX(levelPos.x), GetRowFromY(levelPos.y));
        }

		protected Vector2 GetLevelPositionFromGridPosition(GridPosition pos)
        {
            return new Vector2(GetPositionFromColumn(pos.column), GetPositionFromRow(pos.row));
        }

		protected int GetColumnFromX(float posX)
        {
            return (int)((posX - _startX) / gridSize);
        }

		protected int GetRowFromY(float posY)
        {
            return (int)((posY - _startY) / gridSize);
        }

		protected float GetPositionFromColumn(int row)
        {
            return _startX + row * gridSize + gridSize * 0.5f;
        }

		protected float GetPositionFromRow(int row)
        {
            return _startY + row * gridSize + gridSize * 0.5f;
        }

		protected void SetGridSize(float size)
		{
			gridSize = size;
			_debugDrawSize = new Vector3(gridSize, gridSize, gridSize);
		}

        private void DrawMarker(int row, int column)
        {
            float posX = _startX + column * gridSize + gridSize * 0.5f;
            float posY = _startY + row * gridSize + gridSize * 0.5f;

            Vector3 center = new Vector3(posX, posY, 0);
            Gizmos.DrawCube(center, _debugDrawSize);
        }

        #endregion
    }
}