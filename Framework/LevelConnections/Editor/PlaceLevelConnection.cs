using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CraftingLegends.Framework
{
	public class PlaceLevelConnection
	{
		[MenuItem("Framework/Level Connection Entry #F3")]   // Shift+F2
		public static void PlaceLevelConnectionMenuEntry()
		{
			CreateConnection();
		}

		[MenuItem("Framework/Level Connection Entry #F3", true)]
		public static bool PlaceLevelConnectionMenuEntryValidator()
		{
			return !Application.isPlaying;
		}

		private static void CreateConnection()
		{
			Transform active = Selection.activeTransform;

			if (active == null)
			{
				Debug.LogWarning("No GameObject selected");

				return;
			}

			LevelEntry entry = active.gameObject.AddComponent<LevelEntry>();

			//LevelConnection connection = new LevelConnection();
			LevelConnection connection = ScriptableObject.CreateInstance<LevelConnection>();
			connection.levelName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);

			string fileName = EditorUtility.SaveFilePanelInProject("Save Connection", "Connection", "asset", "Save new Connection");

			if (!string.IsNullOrEmpty(fileName))
			{
				string shortName = Path.GetFileNameWithoutExtension(fileName);

				AssetDatabase.CreateAsset(connection, fileName);
				AssetDatabase.SaveAssets();

				entry.incomingConnection = connection;

				active.name = shortName;
			}
		}
	}
}