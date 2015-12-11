
using System;

namespace CraftingLegends.Framework
{
	public interface IUiState<T> : INavigationInput // where T : struct, IConvertible
	{
		void Enter();
		void Exit();

		void SetActive();
		void SetInactive();

		T gameState { get; }
	}
}