using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// describes an action that lasts a certain duration and can be added to a chronological queue
	/// </summary>
	public interface IActionQueueElement
	{
		void OnStart();
		void Update();
		void OnExit();

		bool hasEnded
		{
			get;
		}
	}
}