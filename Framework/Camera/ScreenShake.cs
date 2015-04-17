using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ScreenShake : MonoBehaviour
	{
		public float ShakeAmount = 0.25f;
		public float decreaseFactor = 1.0f;

		private Camera _camera;
		private Vector3 originalPos;
		private float shake = 0.0f;

		void Awake()
		{
			_camera = GetComponent<Camera>();
		}

		void Update()
		{
			if (BaseGameController.isRunning)
			{
				if (shake > 0.0f)
				{
					Vector2 shakePos = Random.insideUnitCircle * ShakeAmount * shake;
					_camera.transform.localPosition = new Vector3(shakePos.x, shakePos.y, originalPos.z);

					shake -= Time.deltaTime * decreaseFactor;

					if (shake <= 0.0f)
					{
						shake = 0.0f;
						_camera.transform.localPosition = originalPos;
					}
				}
			}
		}

		public void Shake(float amount)
		{
			if (shake <= 0.0f)
			{
				originalPos = _camera.transform.localPosition;
			}

			shake = amount;
		}
	}
}