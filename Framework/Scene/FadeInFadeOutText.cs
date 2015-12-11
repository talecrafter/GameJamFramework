using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class FadeInFadeOutText : FadeInFadeOutBase
	{
		private Text[] _textDisplays;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		protected override void Awake()
		{
			_textDisplays = GetComponentsInChildren<Text>();

			base.Awake();
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		protected override void ApplyAmplitude(float amplitude)
		{
			for (int i = 0; i < _textDisplays.Length; i++)
			{
				_textDisplays[i].color = Utilities.ColorWithAlpha(_textDisplays[i].color, amplitude);
			}
		}
	}
}