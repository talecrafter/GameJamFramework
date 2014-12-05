using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	/// <summary>
	/// list of actions that get executed one after another
	/// </summary>
	public class ActionQueue
	{
		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private Queue<IActionQueueElement> _queue = new Queue<IActionQueueElement>();

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		/// <summary>
		/// should be called every frame by parent object
		/// </summary>
		public void Update()
		{
			if (_queue.Count == 0)
				return;

			IActionQueueElement currentElement = _queue.Peek();
			currentElement.Update();

			if (currentElement.hasEnded)
			{
				currentElement.OnExit();
				_queue.Dequeue();

				if (_queue.Count > 0)
				{
					currentElement = _queue.Peek();
					currentElement.OnStart();
				}
			}
		}

		/// <summary>
		/// adds a new element to the back of the queue
		/// </summary>
		/// <param name="element"></param>
		public void Add(IActionQueueElement element)
		{
			if (element != null)
			{
				_queue.Enqueue(element);

				if (_queue.Count == 1)
				{
					element.OnStart();
				}
			}
		}

		/// <summary>
		/// adds a time duration where nothing happens
		/// </summary>
		/// <param name="duration"></param>
		public void AddPause(float duration)
		{
			Add(new ActionWait(duration));
		}

		/// <summary>
		/// calls OnExit on the current element and clears all entries from the queue
		/// </summary>
		public void Clear()
		{
			if (_queue.Count > 0)
			{
				IActionQueueElement currentElement = _queue.Peek();
				currentElement.OnExit();

				_queue.Clear();
			}
		}

		public bool hasContent
		{
			get
			{
				return _queue.Count > 0;
			}
		}
	}
}