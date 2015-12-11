using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Core
{
	public class TextManagerInit : MonoBehaviour
	{
		public TextAsset languages;
		public List<TextAsset> languageFiles;

		public void Init()
		{
			TextManager.Load(languages, languageFiles);
		}
	}
}