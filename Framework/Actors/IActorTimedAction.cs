
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public interface IActorTimedAction
	{
		float range { get; }
		float cooldown { get; }
		void Execute();
	}
}