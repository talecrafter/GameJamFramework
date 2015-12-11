using UnityEngine;
using System.Collections;
using System;

namespace CraftingLegends.Framework
{
	public class Energy
	{
		// ================================================================================
		//  values
		// --------------------------------------------------------------------------------

		const float min = 0;

		private float _current = 0;
		private float _max = 1;
		private float _startValue;

		// ================================================================================
		//  state
		// --------------------------------------------------------------------------------

		public float current
		{
			get { return _current; }
			private set
			{
				_current = Mathf.Clamp(value, min, _max);
			}
		}

		public float max
		{
			get { return _max; }
		}

		public float missingAmount
		{
			get
			{
				return _max - _current;
			}
		}

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

			this.current = current;

			_startValue = current;
		}

		// ================================================================================
		//  adding
		// --------------------------------------------------------------------------------

		public void AddPortion(float portion)
		{
			Add(portion * range);
		}

		public void AddFull()
		{
			Add(_max - _current);
		}

		public void Add(float amount)
		{
			float before = _current;

			current += amount;

			// check for events now
			OnValueChanged();
			OnGotEnergy(_current - before);
			if (isFull && before < _current)
			{
				OnGotFull();
			}
		}

		// ================================================================================
		//  removing
		// --------------------------------------------------------------------------------

		public void LosePortion(float portion)
		{
			Lose(portion * range);
		}

		public void LoseAll()
		{
			Lose(_current);
		}

		public void Lose(float amount)
		{
			if (isEmpty)
				return;

			float before = _current;

			current -= amount;

			// check for events now
			OnLostEnergy(before - _current);
			OnValueChanged();
		}

		// ================================================================================
		//  utilities
		// --------------------------------------------------------------------------------

		public void Reset()
		{
			_current = _startValue;
		}

		// for balancing
		public void Scale(float factor)
		{
			_max *= factor;
			_current *= factor;
			_startValue *= factor;
		}

		public float GetAmountThatWillBeAdded(float maxPossibleAmount)
		{
			return Mathf.Min(maxPossibleAmount, missingAmount);
		}

		public override string ToString()
		{
			return ((int)_current).ToString() + " / " + ((int)max).ToString();
		}

		// ================================================================================
		//  events
		// --------------------------------------------------------------------------------

		#region events

		public delegate void EnergyDelegate();
		public delegate void EnergyChangedDelegate(float amount);

		public event EnergyDelegate valueChanged;
		public event EnergyDelegate gotFull;
		public event EnergyChangedDelegate gotEnergy;
		public event EnergyChangedDelegate lostEnergy;

		private void OnValueChanged()
		{
			if (valueChanged != null)
				valueChanged();
		}

		private void OnGotFull()
		{
			if (gotFull != null)
				gotFull();
		}

		private void OnGotEnergy(float amount)
		{
			if (gotEnergy != null && amount >= 0)
				gotEnergy(amount);
		}

		private void OnLostEnergy(float amount)
		{
			if (lostEnergy != null && amount > 0)
				lostEnergy(amount);
		}

		#endregion
	}
}