using UnityEngine;
using System.Collections;

namespace CraftingLegends.Framework
{
    public class MonoBehaviourWithHash : MonoBehaviour
    {
        private int? _hash;
        public int hash
        {
            get
            {
                if (_hash == null)
                    _hash = transform.position.GetHashCode() + Application.loadedLevelName.GetHashCode();

                return _hash.Value;
            }
        }

        public bool hashWasEntered
        {
            get
            {
                return MainBase.Instance.gameStateData.wasUsed.Contains(hash);
            }
        }

        public void EnterHash()
        {
            MainBase.Instance.gameStateData.wasUsed.Add(hash);
        }

        public void RemoveHash()
        {
            if (hashWasEntered)
                MainBase.Instance.gameStateData.wasUsed.Remove(hash);
        }
    }
}

