
namespace CraftingLegends.Framework
{
	public interface IActorAction
	{
		float range { get; }
		float cooldown { get; }
		void Execute();
	}
}