using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	[CustomEditor(typeof(LevelConnection))]
	public class LevelConnectionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}
	}
}