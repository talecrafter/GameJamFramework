using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// list of actions that get executed all at the same time
	/// </summary>
	public class ActionList
	{
		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private List<IActionQueueElement> _list = new List<IActionQueueElement>();

		// temporary list for all objects that have ended
		private List<IActionQueueElement> _finishedList = new List<IActionQueueElement>();

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		/// <summary>
		/// should be called every frame by parent object
		/// </summary>
		public void Update()
		{
			if (_list.Count == 0)
				return;

			for (int i = 0; i < _list.Count; i++)
			{
				_list[i].Update();

				if (_list[i].hasEnded)
				{
					_finishedList.Add(_list[i]);
				}
			}

			for (int i = 0; i < _finishedList.Count; i++)
			{
				_finishedList[i].OnExit();
				_list.Remove(_finishedList[i]);
			}

			_finishedList.Clear();
		}

		/// <summary>
		/// adds a new element to the list
		/// </summary>
		/// <param name="element"></param>
		public void Add(IActionQueueElement element)
		{
			if (element != null)
			{
				_list.Add(element);

				element.OnStart();
			}
		}

		/// <summary>
		/// calls OnExit on the current element and clears all entries from the queue
		/// </summary>
		public void Clear()
		{
			if (_list.Count > 0)
			{
				for (int i = 0; i < _list.Count; i++)
				{
					_list[i].OnExit();
				}

				_list.Clear();
			}
		}

		public bool hasContent
		{
			get
			{
				return _list.Count > 0;
			}
		}
	}
}
