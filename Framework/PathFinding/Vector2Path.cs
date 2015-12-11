using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftingLegends.Framework
{
	public class Vector2Path
	{
		private List<Vector2> _path = new List<Vector2>();
		private int _count = 0;
		private int _index = 0;

		private bool _hasFinished = false;
		public bool hasFinished
		{
			get
			{
				return _hasFinished;
			}
		}

		public bool isValid
		{
			get
			{
				return _count > 0;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public Vector2 this[int index]
		{
			get
			{
				return _path[index];
			}
		}

		public Vector2 CurrentPosition
		{
			get
			{
				return _path[_index];
			}
		}

		// ================================================================================
		//  constructor
		// --------------------------------------------------------------------------------

		public Vector2Path(int allocationSize = 50)
		{
			for (int i = 0; i < allocationSize; i++)
			{
				_path.Add(new Vector2(0, 0));
			}
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public void AddPosition(float x, float y)
		{
			if (_count == _path.Count)
			{
				_path.Add(new Vector2(x, y));
				_count++;
				_hasFinished = false;
				return;
			}

			_path[_count] = new Vector2(x, y);
			_count++;
			_hasFinished = false;
		}

		public void NextPosition()
		{
			_index++;

			if (_index >= _count)
			{
				_hasFinished = true;
			}
		}

		public void Clear()
		{
			_index = 0;
			_count = 0;
			_hasFinished = true;
		}

		public override string ToString()
		{
			string posString = "";

			for (int i = 0; i < _count; i++)
			{
				posString += "[" + _path[i] + "] ";
			}

			return posString;
		}
	}
}