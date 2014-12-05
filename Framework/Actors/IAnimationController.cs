using UnityEngine;

namespace CraftingLegends.Framework
{
	interface IAnimationController
	{
		void FadeOut();
		void Reset();
		void SetMaterialColor(Color color);
	}
}