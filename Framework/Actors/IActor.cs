using System;
using UnityEngine;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
    public delegate void StateChanged(IActor actor, ActorState state);

    public interface IActor
    {
        bool isAlive { get; }
        Vector3 position { get; }
        Vector2 position2D { get; }

        event StateChanged stateChanged;
    }
}
