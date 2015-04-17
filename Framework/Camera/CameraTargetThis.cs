using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CraftingLegends.Framework;

namespace CraftingLegends.Framework
{
	public class CameraTargetThis : MonoBehaviour
	{
		void Awake()
		{
			CameraSmoothFollow2D camera = FindObjectOfType<CameraSmoothFollow2D>();
			if (camera != null)
				camera.target = transform;
		}
	}
}