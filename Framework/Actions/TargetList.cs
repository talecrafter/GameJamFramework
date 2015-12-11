using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Framework;

namespace CraftingLegends.Framework
{
	public class TargetList<T> where T : MonoBehaviour, ITarget
	{
		public event System.Action<T> focusChanged;

		public bool hasObjects
		{
			get
			{
				return _targets.Count > 0;
			}
		}

		public T current
		{
			get
			{
				if (_targets.Count > 0)
					return _targets[_targets.Count - 1];
				else
					return default(T);
			}
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		protected List<T> _targets = new List<T>();

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public int Count
		{
			get
			{
				return _targets.Count;
			}
		}

		public void Add(T target)
		{
			// check if valid
			if (target == null)
				return;

			if (_targets.Contains(target))
				return;

			_targets.Add(target);

			OnFocusChanged();
		}

		public void Remove(T target)
		{
			// check if valid
			if (target == null || !_targets.Contains(target))
				return;

			if (target == current)
			{
				_targets.Remove(target);
				
				OnFocusChanged();
			}
			else
			{
				// just remove from the list
				_targets.Remove(target);
			}
		}

		public void Clear()
		{
			if (_targets.Count > 0)
			{
				_targets.Clear();
				OnFocusChanged();
			}
		}

		public bool Contains(T focusObject)
		{
			return _targets.Contains(focusObject);
		}

		public void SortByNearest(Vector2 pos)
		{
			if (_targets.Count <= 1)
				return;

			T oldFocus = current;

			_targets.Sort(
				(firstObject, secondObject) =>
				Vector2.SqrMagnitude(secondObject.position2D - pos).CompareTo(Vector2.SqrMagnitude(firstObject.position2D - pos)));

			if (oldFocus != current)
			{
				OnFocusChanged();
			}
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void OnFocusChanged()
		{
			if (focusChanged != null)
				focusChanged(current);
		}
	}

}
