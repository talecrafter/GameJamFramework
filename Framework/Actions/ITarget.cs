using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Framework
{
	public interface ITarget
	{
		event System.Action becomesInvalid;
		Vector2 position2D { get; }
	}
}