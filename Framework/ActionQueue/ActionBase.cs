using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ActionBase : IActionQueueElement
	{
		public virtual void OnStart()
		{
			// empty
		}

		public virtual void Update()
		{
			// empty
		}

		public virtual void OnExit()
		{
			// empty
		}

		public virtual bool hasEnded
		{
			get { return true; }
		}
	}
}