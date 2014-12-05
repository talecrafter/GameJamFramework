
namespace CraftingLegends.Framework
{
	public interface IState
	{
		void Enter();
		void OnGUI();
		void Update();
		void Exit();
	}
}