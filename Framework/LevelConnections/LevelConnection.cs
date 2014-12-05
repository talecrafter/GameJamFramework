using UnityEngine;

namespace CraftingLegends.Framework
{
	public class LevelConnection : ScriptableObject
	{
		public string levelName;

		public override string ToString()
		{
			return levelName;
		}
	}
}