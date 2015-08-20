using UnityEngine;
using System.Collections;

namespace CraftingLegends.Framework
{
	public interface IHasHealth
	{
		Energy health
		{
			get;
			set;
		}
	}
}