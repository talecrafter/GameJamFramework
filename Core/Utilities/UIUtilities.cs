using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Core
{
	public static class UIUtilities
	{
		/// <summary>
		/// changes a left bound ui container to right bound; assumes that the rect width of the RectTransform contains all content
		/// </summary>
		/// <param name="component"></param>
		public static void MoveAlignmentFromLeftToRight(MonoBehaviour component)
		{
			RectTransform rectTransform = component.GetComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(1f, 1f);
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x - rectTransform.rect.width, rectTransform.anchoredPosition.y);
		}

		public static void MoveAlignmentFromRightToLeft(MonoBehaviour component)
		{
			RectTransform rectTransform = component.GetComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
			rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x + rectTransform.rect.width, rectTransform.anchoredPosition.y);
		}

		public static void MoveAnchorFromLeftToRight(MonoBehaviour component)
		{
			RectTransform rectTransform = component.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
		}

		public static void MoveAnchorFromRightToLeft(MonoBehaviour component)
		{
			RectTransform rectTransform = component.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
		}

		public static void MoveAnchorFromLeftToRight(Transform transform)
		{
			RectTransform rectTransform = transform.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
		}

		public static void MoveAnchorFromRightToLeft(Transform transform)
		{
			RectTransform rectTransform = transform.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
		}

		/// <summary>
		/// just sets the x scale to -x
		/// </summary>
		/// <param name="component"></param>
		public static void FlipXScale(MonoBehaviour component)
		{
			RectTransform rectTransform = component.GetComponent<RectTransform>();
			Vector3 scale = rectTransform.localScale;
			scale.x *= -1f;
			rectTransform.localScale = scale;
		}
	}
}