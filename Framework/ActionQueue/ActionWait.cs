using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ActionWait : IActionQueueElement
	{
		Timer _timer;

		float _duration;

		public ActionWait(float duration)
		{
			_duration = duration;
		}

		public void OnStart()
		{
			_timer = new Timer(_duration);
		}

		public void Update()
		{
			_timer.Update();
		}

		public void OnExit()
		{

		}

		public bool hasEnded
		{
			get
			{
				return _timer.hasEnded;
			}
		}
	}
}