using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ParallaxLayer : MonoBehaviour
	{
		// 0 -> exactly as camera
		// 1 -> staying exactly in the background
		public float scale;

		private Transform _cameraTransform;
		private Transform _transform;

		float _cameraStartX;
		float _layerStartX;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_cameraTransform = Camera.main.transform;
			_cameraStartX = _cameraTransform.position.x;
			_transform = transform;
			_layerStartX = _transform.position.x;
		}

		void Update()
		{
			UpdatePosition();
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void UpdatePosition()
		{
			float distance = _cameraTransform.position.x - _cameraStartX;

			float newHorizontalPos = _layerStartX + distance * scale;

			_transform.position = new Vector3(newHorizontalPos, _transform.position.y, _transform.position.z);
		}
	}
}