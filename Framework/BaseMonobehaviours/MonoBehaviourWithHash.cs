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
                return BaseGameController.Instance.gameStateData.wasUsed.Contains(hash);
            }
        }

        public void EnterHash()
        {
            BaseGameController.Instance.gameStateData.wasUsed.Add(hash);
        }

        public void RemoveHash()
        {
            if (hashWasEntered)
                BaseGameController.Instance.gameStateData.wasUsed.Remove(hash);
        }
    }
}

