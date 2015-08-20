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

		public void Push(T state)
		{
			_stack.Push(state);
			state.OnEnter();
		}

		public void SwitchTo(T state)
		{
			PopAll();
			Push(state);
		}

		public T Pop()
		{
			if (_stack.Count > 0)
			{
				T state = _stack.Pop();
				state.OnExit();
				return state;
			}
			else
			{
				return default(T);
			}
		}

		public IState Peek()
		{
			if (_stack.Count > 0)
				return _stack.Peek();
			else
				return default(T);
		}

		public void PopAll()
		{
			while (_stack.Count > 0)
			{
				Pop();
			}
		}

		// ================================================================================
		//  update methods
		// --------------------------------------------------------------------------------

		public void Update()
		{
			if (_stack.Count > 0)
			{
				_stack.Peek().Update();
			}
		}
	}
}