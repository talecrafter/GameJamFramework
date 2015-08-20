using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CraftingLegends.Framework
{
	public class NavigationInputHandler
	{
		// ================================================================================
		//  declarations
		// --------------------------------------------------------------------------------

		protected const float errorMargin = 0.5f;

		protected enum VerticalDirection
		{
			Up,
			Center,
			Down
		}

		protected enum HorizontalDirection
		{
			Left,
			Center,
			Right
		}

		// ================================================================================
		//  Input Receiver List
		// --------------------------------------------------------------------------------

		private List<INavigationInput> _inputReceiverList = new List<INavigationInput>();

		private INavigationInput current
		{
			get
			{
				if (_inputReceiverList.Count > 0)
					return _inputReceiverList[_inputReceiverList.Count - 1];
				else return null;
			}
		}

		public void Add(INavigationInput inputReceiver)
		{
			if (!_inputReceiverList.Contains(inputReceiver))
				_inputReceiverList.Add(inputReceiver);

			_skipFrame = true;
		}

		public void Remove(INavigationInput inputReceiver)
		{
			if (_inputReceiverList.Contains(inputReceiver))
				_inputReceiverList.Remove(inputReceiver);

			_skipFrame = true;
		}

		public bool hasTarget
		{
			get
			{
				return _inputReceiverList.Count > 0;
			}
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private VerticalDirection _lastVerticalDirection = VerticalDirection.Center;
		private HorizontalDirection _lastHorizontalDirection = HorizontalDirection.Center;

		// flag to skip input for a single Update; used when the receiver changes;
		// otherwise odd behaviour could occur when a receiver is added by an "Enter" command and gets "Enter" input directly afterwards in the same frame
		protected bool _skipFrame = false;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		public void Update(float hInput, float vInput)
		{
			if (_skipFrame)
			{
				_skipFrame = false;
				return;
			}

			if (Mathf.Abs(hInput) > Mathf.Abs(vInput))
			{
				if (!UpdateHorizontal(hInput))
					UpdateVertical(vInput);
			}
			else
			{
				if (!UpdateVertical(vInput))
					UpdateHorizontal(hInput);
			}			
		}

		// returns true when changed
		private bool UpdateVertical(float vInput)
		{
			if (vInput > errorMargin)
				return SetVerticalInput(VerticalDirection.Up);
			else if (vInput < -errorMargin)
				return SetVerticalInput(VerticalDirection.Down);
			else
				return SetVerticalInput(VerticalDirection.Center);
		}

		// returns true when changed
		private bool UpdateHorizontal(float hInput)
		{
			if (hInput > errorMargin)
				return SetHorizontalInput(HorizontalDirection.Right);
			else if (hInput < -errorMargin)
				return SetHorizontalInput(HorizontalDirection.Left);
			else
				return SetHorizontalInput(HorizontalDirection.Center);
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		// returns true when changed
		protected bool SetHorizontalInput(HorizontalDirection direction)
		{
			if (direction != _lastHorizontalDirection)
			{
				if (direction == HorizontalDirection.Left)
					InputLeft();
				else if (direction == HorizontalDirection.Right)
					InputRight();

				_lastHorizontalDirection = direction;

				return true;
			}

			return false;
		}

		// returns true when changed
		protected bool SetVerticalInput(VerticalDirection direction)
		{
			if (direction != _lastVerticalDirection)
			{
				if (direction == VerticalDirection.Up)
					InputUp();
				else if (direction == VerticalDirection.Down)
					InputDown();

				_lastVerticalDirection = direction;

				return true;
			}

			return false;
		}

		// ================================================================================
		//  enter Input into current receiver
		// --------------------------------------------------------------------------------

		public void InputUp()
		{
			if (current != null)
				current.InputUp();
		}

		public void InputDown()
		{
			if (current != null)
				current.InputDown();
		}

		public void InputLeft()
		{
			if (current != null)
				current.InputLeft();
		}

		public void InputRight()
		{
			if (current != null)
				current.InputRight();
		}

		public void InputEnter()
		{
			if (current != null)
				current.InputEnter();
		}

		public void InputBack()
		{
			if (current != null)
				current.InputBack();
		}
	}
}