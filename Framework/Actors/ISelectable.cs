using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftingLegends.Framework
{
    public interface ISelectable
    {
        void Select();
        void Deselect();

        bool canGetSelected { get; }
        event Action isDisabled;
    }
}
