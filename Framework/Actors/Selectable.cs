using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
    public class Selectable : MonoBehaviour
    {
        public enum SearchForSelectable
        {
            InCurrent,
            InParent,
            InChildren,
        }

        public ISelectable selectable = null;

        [SerializeField]
        private SearchForSelectable searchType = SearchForSelectable.InCurrent;

        public void Awake()
        {
            switch (searchType)
            {
                case SearchForSelectable.InCurrent:
                    selectable = transform.GetInterface<ISelectable>();
                    break;
                case SearchForSelectable.InParent:
                    selectable = transform.GetInterfaceInParentAndChildren<ISelectable>();
                    break;
                case SearchForSelectable.InChildren:
                    selectable = transform.GetInterfaceInChildren<ISelectable>();
                    break;
                default:
                    break;
            }
        }
    }
}