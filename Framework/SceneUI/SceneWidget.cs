using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class SceneWidget : PooledObject
	{
		[SerializeField]
		private Transform _target;
		private RectTransform _rectTransform;
		private RectTransform _canvasRect;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		protected virtual void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_canvasRect = transform.root.GetComponent<Canvas>().GetComponent<RectTransform>();
		}

		protected virtual void LateUpdate()
		{
			UpdatePosition();
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void UpdatePosition()
		{
			if (_target != null)
			{
				//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

				Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(_target.position);
				Vector2 WorldObject_ScreenPosition = new Vector2(
				((ViewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
				((ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)));

				// this would be for top left pivot
				//Vector2 WorldObject_ScreenPosition = new Vector2(
				//ViewportPosition.x * CanvasRect.sizeDelta.x,
				//ViewportPosition.y * CanvasRect.sizeDelta.y);

				_rectTransform.anchoredPosition = WorldObject_ScreenPosition;
			}
		}
	}
}