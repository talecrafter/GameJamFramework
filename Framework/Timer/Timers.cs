using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public enum TimerActiveInStates
	{
		Running,
		RunningAndSequence,
		All
	}

	public class Timers
	{
		private List<Timer> _timersInRunning = new List<Timer>();
		private List<Timer> _timersInRunningAndSequence = new List<Timer>();
		private List<Timer> _timersForAllStates = new List<Timer>();

		private List<Timer> _timersToDelete = new List<Timer>();

		private Dictionary<Timer, System.Action> callbacks = new Dictionary<Timer, System.Action>();

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public Timer GetTimer(float duration, System.Action hasEndedCallback, TimerActiveInStates activeInStates = TimerActiveInStates.RunningAndSequence)
		{
			Timer timer = new Timer(duration);

			callbacks[timer] = hasEndedCallback;

			switch (activeInStates)
			{
				case TimerActiveInStates.Running:
					_timersInRunning.Add(timer);
					break;
				case TimerActiveInStates.RunningAndSequence:
					_timersInRunningAndSequence.Add(timer);
					break;
				default:
					_timersForAllStates.Add(timer);
					break;
			}

			return timer;
		}

		public void Stop(Timer timer)
		{
			if (_timersInRunning.Contains(timer))
				_timersInRunning.Remove(timer);
			if (_timersInRunningAndSequence.Contains(timer))
				_timersInRunningAndSequence.Remove(timer);
			if (_timersForAllStates.Contains(timer))
				_timersForAllStates.Remove(timer);
		}

		public void Update()
		{
			if (MainBase.isRunningOrInSequence)
			{
				UpdateTimers(_timersInRunningAndSequence);
			}
			else if (MainBase.isRunning)
			{
				UpdateTimers(_timersInRunning);
			}
			else
			{
				UpdateTimers(_timersForAllStates);
			}
		}

		public void Clear()
		{
			_timersInRunning.Clear();
			_timersInRunningAndSequence.Clear();
			_timersForAllStates.Clear();
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void UpdateTimers(List<Timer> timers)
		{
			_timersToDelete.Clear();

			for (int i = 0; i < timers.Count; i++)
			{
				Timer timer = timers[i];

				timer.Update();

				if (timer.hasEnded)
				{
					callbacks[timer]();
					callbacks.Remove(timer);

					_timersToDelete.Add(timer);
				}
			}

			for (int i = 0; i < _timersToDelete.Count; i++)
			{
				timers.Remove(_timersToDelete[i]);
			}
		}
	}
}