using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CraftingLegends.Framework
{
	public class UIStateMachine<T>// where T : struct, IConvertible
	{
		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private Stack<IUiState<T>> _stack = new Stack<IUiState<T>>();

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public int count
		{
			get
			{
				return _stack.Count;
			}
		}

		public bool hasMultipleStates
		{
			get
			{
				return _stack.Count > 1;
			}
		}

		public bool hasState
		{
			get
			{
				return _stack.Count > 0;
			}
		}

		public IUiState<T> current
		{
			get
			{
				if (_stack.Count > 0)
					return _stack.Peek();
				else
					return default(IUiState<T>);
			}
		}

		public void Push(IUiState<T> state)
		{
			DeactivateCurrent();
			_stack.Push(state);
			ActivateCurrent();
			state.Enter();
		}

		public IUiState<T> Pop()
		{
			if (_stack.Count > 0)
			{
				DeactivateCurrent();
				IUiState<T> state = _stack.Pop();
				state.Exit();

				if (hasState)
					ActivateCurrent();

				return state;
			}
			else
			{
				return default(IUiState<T>);
			}
		}

		public void SwitchTo(IUiState<T> state)
		{
			PopAll();
			Push(state);
		}

		public void PopAll()
		{
			while (_stack.Count > 0)
			{
				Pop();
			}
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void ActivateCurrent()
		{
			if (hasState)
				(current as MonoBehaviour).gameObject.SetActive(true);
		}

		private void DeactivateCurrent()
		{
			if (hasState)
				(current as MonoBehaviour).gameObject.SetActive(false);
		}
	}
}