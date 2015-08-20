
namespace CraftingLegends.Framework
{
	public interface IState
	{
		void OnEnter();
		void Update();
		void OnExit();
	}
}