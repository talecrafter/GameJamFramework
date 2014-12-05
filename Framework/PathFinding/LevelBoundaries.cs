using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
    /// <summary>
    /// max boundaries for camera and player movement
    /// </summary>
    public class LevelBoundaries : MonoBehaviour
    {
        // ================================================================================
        //  public
        // --------------------------------------------------------------------------------

        public Rect bounds = new Rect(0, 0, 10, 10);

        private Rect _levelBounds;
        public Rect levelBounds
        {
            get
            {
                CalculateBounds();
                return _levelBounds;
            }
        }

        // display in editor window
        public bool drawRectangle = true;
        public Color lineColor = Color.blue;

        private Transform _transform;

        // ================================================================================
        //  unity methods
        // --------------------------------------------------------------------------------

        void Awake()
        {
            _transform = transform;

            BaseGameController.Instance.levelBounds = levelBounds;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = lineColor;

            CalculateBounds();

            // horizontal lines
            if (drawRectangle)
            {
                // top line
                Vector3 firstPos = new Vector3(_levelBounds.xMin, _levelBounds.yMin, 0f);
                Vector3 secondPos = new Vector3(_levelBounds.xMax, _levelBounds.yMin, -1.0f);
                Gizmos.DrawLine(firstPos, secondPos);

                // bottom line
                firstPos = new Vector3(_levelBounds.xMin, _levelBounds.yMax, 0f);
                secondPos = new Vector3(_levelBounds.xMax, _levelBounds.yMax, -1.0f);
                Gizmos.DrawLine(firstPos, secondPos);

                // bottom line
                firstPos = new Vector3(_levelBounds.xMin, _levelBounds.yMin, 0f);
                secondPos = new Vector3(_levelBounds.xMin, _levelBounds.yMax, -1.0f);
                Gizmos.DrawLine(firstPos, secondPos);

                // bottom line
                firstPos = new Vector3(_levelBounds.xMax, _levelBounds.yMin, 0f);
                secondPos = new Vector3(_levelBounds.xMax, _levelBounds.yMax, -1.0f);
                Gizmos.DrawLine(firstPos, secondPos);
            }
        }

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public Vector2 GetRelativePosition(Vector3 pos)
		{
			CalculateBounds();

			float x = (pos.x - _levelBounds.x) / _levelBounds.width;
			float y = (pos.y - _levelBounds.y) / _levelBounds.height;

			return new Vector2(x, y);
		}

        // ================================================================================
        //  private methods
        // --------------------------------------------------------------------------------

        /// <summary>
        /// calculates level bounds from position of this object + bounds
        /// </summary>
        private void CalculateBounds()
        {
            _transform = transform;

            _levelBounds.xMin = _transform.position.x + bounds.xMin;
            _levelBounds.yMin = _transform.position.y + bounds.yMin;
            _levelBounds.width = bounds.width;
            _levelBounds.height = bounds.height;
        }
    }

}