using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CraftingLegends.Core
{
	public static class Utilities
	{
		public static Texture2D CreateBlankTexture(Color color, TextureFormat format = TextureFormat.RGB24, int size = 2)
		{
			// create empty texture
			Texture2D texture = new Texture2D(size, size, format, false, true);

			// get all pixels as an array
			var cols = texture.GetPixels();
			for (int i = 0; i < cols.Length; i++)
			{
				cols[i] = color;
			}

			// important steps to save changed pixel values
			texture.SetPixels(cols);
			texture.Apply();

			texture.hideFlags = HideFlags.HideAndDontSave;

			return texture;
		}

		public static Vector2 MouseToScreenCoords()
		{
			return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		}

		public static Vector3 GetNormalizedDirection(Vector3 fromPos, Vector3 toPos)
		{
			Vector3 direction = toPos - fromPos;
			return direction.normalized;
		}

		public static Vector3 GetPositionByObjectName(string objectName)
		{
			GameObject obj = GameObject.Find(objectName);
			if (obj != null)
				return obj.transform.position;
			else
			{
				Debug.LogWarning("Position from Object " + objectName + " not found");
				return Vector3.zero;
			}
		}

		#region LayerMasks

		// calculates a layermask by combining all named layers
		public static int LayerMask(params string[] layerNames)
		{
			int mask = 0;  // no layers selected

			for (int i = 0; i < layerNames.Length; i++)
			{
				int newMask = 1 << UnityEngine.LayerMask.NameToLayer(layerNames[i]);
				mask = mask | newMask;
			}

			return mask;
		}

		// calculates a layermask by adding layerNames to default layer mask
		public static int LayerMaskIncludingDefault(params string[] layerNames)
		{
			int mask = 1;  // LayerMask for 'Default' Layer

			for (int i = 0; i < layerNames.Length; i++)
			{
				int newMask = 1 << UnityEngine.LayerMask.NameToLayer(layerNames[i]);
				mask = mask | newMask;
			}

			return mask;
		}

		// calculates a layermask by subtracting all named layers
		public static int LayerMaskAllExcluding(params string[] layerNames)
		{
			int mask = int.MaxValue; // all layers selected

			for (int i = 0; i < layerNames.Length; i++)
			{
				int newMask = 1 << UnityEngine.LayerMask.NameToLayer(layerNames[i]);

				// we 'subtract' the bits that are in both masks
				mask = mask ^ newMask;   // 0xff00 ^ 0x0ff0 => 0xf0f0;
			}

			return mask;
		}

		#endregion

		public static float TestMethod(Action actionToBeTested, string testName, int numberOfExecutions = 50, bool printDebug = true)
		{
			float startTime = Time.realtimeSinceStartup;

			for (int i = 0; i < numberOfExecutions; i++)
			{
				actionToBeTested();
			}

			float averageTime = (Time.realtimeSinceStartup - startTime) / (float)numberOfExecutions;

			if (printDebug)
				Debug.Log("Testing " + testName + ": " + averageTime.ToString("F5") + " ms in average");

			return averageTime;
		}


		public static Rect screenRect
		{
			get
			{
				return new Rect(0, 0, Screen.width, Screen.height);
			}
		}

		public static Color ColorWithAlpha(Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}

		public static Color ColorFromInt(int red, int green, int blue)
		{
			return new Color(red / 255f, green / 255f, blue / 255f);
		}
	}
}