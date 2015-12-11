using UnityEngine;

namespace CraftingLegends.Framework
{
	interface IAnimationController
	{
		void FadeOut(float time = 1f);
        void FadeOutAfterDeath();
		void Reset();
		void SetMaterialColor(Color color);
	}
}