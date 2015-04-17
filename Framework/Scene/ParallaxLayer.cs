using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ParallaxLayer : MonoBehaviour
	{
		public enum ParallaxDirection
		{
			Horizontal,
			Vertical,
			Both
		}

		public ParallaxDirection direction = ParallaxDirection.Horizontal;

		// 0 -> exactly as camera
		// 1 -> staying exactly in the background
		public float scaleHorizontal;
		public float scaleVertical;

		private Transform _cameraTransform;
		private Transform _transform;

		float _cameraStartX;
		float _layerStartX;
		float _cameraStartY;
		float _layerStartY;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_cameraTransform = Camera.main.transform;
			_transform = transform;

			_cameraStartX = _cameraTransform.position.x;
			_cameraStartY = _cameraTransform.position.y;
			_layerStartX = _transform.position.x;
			_layerStartY = _transform.position.y;
		}

		void LateUpdate()
		{
			UpdatePosition();
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void UpdatePosition()
		{
			Vector3 newPos = _transform.position;

			if (direction == ParallaxDirection.Horizontal || direction == ParallaxDirection.Both)
			{
				float hDistance = _cameraTransform.position.x - _cameraStartX;
				newPos.x = _layerStartX + hDistance * scaleHorizontal;
			}

			if (direction == ParallaxDirection.Vertical || direction == ParallaxDirection.Both)
			{
				float vDistance = _cameraTransform.position.y - _cameraStartY;
				newPos.y = _layerStartY + vDistance * scaleVertical;
			}

			_transform.position = newPos;
		}
	}
}