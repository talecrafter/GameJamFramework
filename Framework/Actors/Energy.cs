using UnityEngine;
using System.Collections;

namespace CraftingLegends.Framework
{
	public class Energy
	{
		#region values

		const float min = 0;

		private float _startValue;

		private float _current = 0;
		public float current
		{
			get { return _current; }
			private set
			{
				_current = Mathf.Clamp(value, min, _max);
			}
		}

		public float woundedAmount
		{
			get
			{
				return _max - _current;
			}
		}

		private float _max = 1;
		public float max
		{
			get { return _max; }
		}

		#endregion

		#region state

		public float proportion
		{
			get { return _current / _max; }
		}

		public bool isFull
		{
			get { return _current >= _max; }
		}

		public bool isEmpty
		{
			get { return _current <= min; }
		}

		public float range
		{
			get { return _max - min; }
		}

		#endregion

		// ================================================================================
		//  constructor
		// --------------------------------------------------------------------------------

		public Energy(float max)
		{
			_max = max;
			_current = max;
			_startValue = _current;
        }

		public Energy(float current, float max)
		{
			_max = max;

			_current = current;
			_current = Mathf.Clamp(current, 0, max);

			_startValue = current;
		}

		// ================================================================================
		//  adding and removing
		// --------------------------------------------------------------------------------

		public void Add(float amount)
		{
			float before = _current;

			_current += amount;
			_current = Mathf.Clamp(_current, 0, _max);

			OnValueChanged();

			if (_current == _max
				&& before < _current)
			{
				OnGotFull();
			}
		}

		public void AddPortion(float portion)
		{
			Add(portion * range);
		}

		public void Lose(float amount)
		{
			_current -= amount;
			_current = Mathf.Clamp(_current, 0, _max);

			OnValueChanged();
		}

		public void LosePortion(float portion)
		{
			Lose(portion * range);
		}

		public void SetToFull()
		{
			if (_current < _max)
			{
				_current = _max;

				OnValueChanged();
				OnGotFull();
			}
		}

		public void Reset()
		{
			_current = _startValue;
		}

		public void Empty()
		{
			_current = min;

			OnValueChanged();
		}

		public void EmptySilently()
		{
			_current = min;
		}

		public void Scale(float factor)
		{
			_max *= factor;
			_current *= factor;
			_startValue *= factor;
		}

		// ================================================================================
		//  events
		// --------------------------------------------------------------------------------

		#region events

		public delegate void OnValueChangedDelegate();
		public delegate void OnGotFullDelegate();

		public event OnValueChangedDelegate onValueChanged;
		public event OnGotFullDelegate onGotFull;

		private void OnValueChanged()
		{
			if (onValueChanged != null)
				onValueChanged();
		}

		private void OnGotFull()
		{
			if (onGotFull != null)
				onGotFull();
		}

		#endregion
	}
}