using System;

namespace CraftingLegends.Core
{
    public interface IPooledObject
    {
        void ToggleOn(); // used by the object pool to reset the object to its original state
        void ToggleOff(); // called by object pool after notification of 'getsDisabled'
        event Action<IPooledObject> getsDisabled;
        bool isUsedByObjectPool { get; set; }
    }
}