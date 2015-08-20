using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class CameraShake : MonoBehaviour
	{
		[SerializeField]
		private float _shakeAmount = 0.25f;
		[SerializeField]
		private float _decreaseFactor = 1.0f;

		private Camera _camera;
		private Vector3 _originalPos;
		private float _shake = 0.0f;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_camera = GetComponent<Camera>();
		}

		void Update()
		{
			if (MainBase.isRunningOrInSequence)
			{
				if (_shake > 0.0f)
				{
					Vector2 shakePos = Random.insideUnitCircle * _shakeAmount * _shake;
					_camera.transform.localPosition = new Vector3(shakePos.x, shakePos.y, _originalPos.z);

					_shake -= Time.deltaTime * _decreaseFactor;

					if (_shake <= 0.0f)
					{
						_shake = 0.0f;
						_camera.transform.localPosition = _originalPos;
					}
				}
			}
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public void Shake(float amount)
		{
			if (_shake <= 0.0f)
			{
				_originalPos = _camera.transform.localPosition;
			}

			_shake = amount;
		}
	}
}