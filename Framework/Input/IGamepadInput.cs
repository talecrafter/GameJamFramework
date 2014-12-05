
using UnityEngine;

namespace CraftingLegends.Framework
{
	public interface IGamepadInput {

		// movement

		bool hasMovement { get; }
		Vector2 movement { get; }
		bool directionChanged { get; }
		float horizontal { get; }
		float vertical { get; }

		bool leftWasPressed { get; }
		bool rightWasPressed { get; }
		bool upWasPressed { get; }
		bool downWasPressed { get; }

		// buttons

		bool anyButton { get; }
		bool menuButton { get; }

		bool action1 { get; }
		bool action2 { get; }
		bool action3 { get; }
		bool action4 { get; }

		bool dpadLeft { get; }
		bool dpadRight { get; }

		bool leftBumper { get; }
		bool rightBumper { get; }

		bool leftTriggerWasPressed { get; }
		bool rightTriggerWasPressed { get; }

	}
}