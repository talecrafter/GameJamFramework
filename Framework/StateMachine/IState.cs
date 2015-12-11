
namespace CraftingLegends.Framework
{
	public interface IState
	{
		void OnEnter();
		void OnUpdate();
		void OnExit();

		void OnGotFocus();
		void OnLostFocus();
	}
}