using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Framework;

namespace CraftingLegends.Framework
{
	public class BaseNavigationInput : MonoBehaviour
	{
		// ================================================================================
		//  declarations
		// --------------------------------------------------------------------------------

		protected const float errorMargin = 0.3f;

		public enum VerticalDirection
		{
			Up,
			Center,
			Down
		}

		public enum HorizontalDirection
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

		public virtual void Update()
		{
			if (_skipFrame)
			{
				_skipFrame = false;
				return;
			}

			float hInput = Input.GetAxis("Horizontal");
			if (hInput > errorMargin)
				SetHorizontalInput(HorizontalDirection.Right);
			else if (hInput < -errorMargin)
				SetHorizontalInput(HorizontalDirection.Left);
			else
				SetHorizontalInput(HorizontalDirection.Center);

			float vInput = Input.GetAxis("Vertical");
			if (vInput > errorMargin)
				SetVerticalInput(VerticalDirection.Up);
			else if (vInput < -errorMargin)
				SetVerticalInput(VerticalDirection.Down);
			else
				SetVerticalInput(VerticalDirection.Center);

			if (Input.GetKeyDown(KeyCode.Escape))
				InputBack();
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		protected void SetHorizontalInput(HorizontalDirection direction)
		{
			if (direction != _lastHorizontalDirection)
			{
				if (direction == HorizontalDirection.Left)
					InputLeft();
				else if (direction == HorizontalDirection.Right)
					InputRight();

				_lastHorizontalDirection = direction;
			}
		}

		protected void SetVerticalInput(VerticalDirection direction)
		{
			if (direction != _lastVerticalDirection)
			{
				if (direction == VerticalDirection.Up)
					InputUp();
				else if (direction == VerticalDirection.Down)
					InputDown();

				_lastVerticalDirection = direction;
			}
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