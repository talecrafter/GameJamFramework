
using System;

namespace CraftingLegends.Framework
{
	public interface IUiState<T> : INavigationInput // where T : struct, IConvertible
	{
		void Enter();
		void Exit();

		T gameState { get; }
	}
}