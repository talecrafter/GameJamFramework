using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace CraftingLegends.Framework
{
	public class SceneText : SceneWidget
	{
		[SerializeField]
		private Text _frontText;
		[SerializeField]
		private Text _shadowText;

		public void SetText(string content)
		{
			if (_frontText != null)
				_frontText.text = content;
			if (_shadowText != null)
				_shadowText.text = content;
		}
	}
}