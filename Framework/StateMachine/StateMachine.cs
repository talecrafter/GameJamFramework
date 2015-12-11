using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class StateMachine<T> where T : IState
	{
		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private Stack<T> _stack = new Stack<T>();

		// ================================================================================
		//  properties
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

		// ================================================================================
		//  core methods
		// --------------------------------------------------------------------------------

		public void Push(T state)
		{
			if (_stack.Count > 0)
				_stack.Peek().OnLostFocus();

			_stack.Push(state);
			state.OnEnter();
			state.OnGotFocus();
		}

		public T Pop()
		{
			if (_stack.Count > 0)
			{
				T state = _stack.Pop();
				state.OnLostFocus();
				state.OnExit();

				if (_stack.Count > 0)
					Peek().OnGotFocus();

				return state;
			}
			else
			{
				return default(T);
			}
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public T Peek()
		{
			if (_stack.Count > 0)
				return _stack.Peek();
			else
				return default(T);
		}

		public void SwitchTo(T state)
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

		/// <summary>
		/// should be called by the class holding the StateMachine
		/// </summary>
		public void Update()
		{
			if (_stack.Count > 0)
			{
				_stack.Peek().OnUpdate();
			}
		}
	}
}