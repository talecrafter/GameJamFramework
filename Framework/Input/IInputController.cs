using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public interface IInputController
	{
		InputScheme inputScheme { get; }
		event System.Action<InputScheme> inputSchemeChanged;
	}
}