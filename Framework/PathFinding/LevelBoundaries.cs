using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

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

		[SerializeField]
        protected Rect bounds = new Rect(0, 0, 10, 10);

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

            MainBase.Instance.levelBounds = levelBounds;
        }

        void OnDrawGizmos()
        {
            CalculateBounds();

			GizmosUtilities.DrawRect(_levelBounds, lineColor);
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

		public void SetBounds(Rect bounds)
		{
			this.bounds = bounds;
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