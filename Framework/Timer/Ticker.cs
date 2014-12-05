using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// timer helper which 'ticks' at every duration
	/// gets updated from outside
	/// </summary>
	public class Ticker
	{

		// ================================================================================
		//  declarations
		// --------------------------------------------------------------------------------

		public delegate void TickAction();

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private TickAction _tickEvent;   // will be called at every tick
		private float _interval = 1.0f;
		private float _tickTime = 0;

		// ================================================================================
		//  constructor
		// --------------------------------------------------------------------------------

		public Ticker(TickAction tickHandler, float interval = 1.0f)
		{
			_tickEvent = tickHandler;
			_interval = interval;
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public void Update()
		{
			_tickTime += Time.deltaTime;

			if (_tickTime >= _interval)
			{
				if (_tickEvent != null)
				{
					_tickEvent();
				}
				_tickTime = 0;
			}
		}

		public void Reset()
		{
			_tickTime = 0;
		}
	}
}
