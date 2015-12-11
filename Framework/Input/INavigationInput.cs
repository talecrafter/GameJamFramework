using System;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
    public interface INavigationInput
    {
        void InputUp();
        void InputDown();
        void InputLeft();
        void InputRight();
        void InputEnter();
        void InputBack();

		bool acceptsSecondaryButtons { get; }
    }
}