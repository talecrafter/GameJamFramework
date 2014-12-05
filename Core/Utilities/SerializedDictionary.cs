using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
	[System.Serializable]
	public class SerializedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IEquatable<TKey>
	{
		[SerializeField]
		private List<TKey> keysList = new List<TKey>();
		public List<TKey> KeysList
		{
			get { return keysList; }
			set { keysList = value; }
		}

		[SerializeField]
		private List<TValue> valuesList = new List<TValue>();
		public List<TValue> ValuesList
		{
			get { return valuesList; }
			set { valuesList = value; }
		}

		public void Add(TKey key, TValue data)
		{
			if (!ContainsKey(key))
			{
				keysList.Add(key);
				valuesList.Add(data);
			}
			else
				SetValue(key, data);
		}

		public void Remove(TKey key)
		{
			valuesList.Remove(GetValue(key));
			keysList.Remove(key);
		}

		public bool ContainsKey(TKey key)
		{
			return ConvertToDictionary().ContainsKey(key);
		}

		public bool ContainsValue(TValue data)
		{
			return ConvertToDictionary().ContainsValue(data);
		}

		public void Clear()
		{
			if (keysList.Count > 0)
				keysList.Clear();
			if (valuesList.Count > 0)
				valuesList.Clear();
		}

		public void SetValue(TKey key, TValue data)
		{
			int keyIndex = 0;
			for (int i = 0; i < keysList.Count; i++)
			{
				if (keysList[i].Equals(key))
					keyIndex = i;
			}

			valuesList[keyIndex] = data;

		}

		public TKey GetKey(TKey key)
		{
			for (int i = 0; i < keysList.Count; i++)
			{
				if (keysList[i].Equals(key))
					return keysList[i];
			}
			return default(TKey);

		}

		public TValue GetValue(TKey key)
		{
			int keyIndex = 0;
			for (int i = 0; i < keysList.Count; i++)
			{
				if (keysList[i].Equals(key))
					keyIndex = i;
			}

			return valuesList[keyIndex];

		}

		public Dictionary<TKey, TValue> ConvertToDictionary()
		{
			Dictionary<TKey, TValue> dictionaryData = new Dictionary<TKey, TValue>();

			try
			{

				for (int i = 0; i < keysList.Count; i++)
				{
					dictionaryData.Add(keysList[i], valuesList[i]);
				}

			}
			catch (Exception)
			{
				Debug.LogError("KeysList.Count is not equal to ValuesList.Count. It shouldn't happen!");
			}

			return dictionaryData;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			for (int i = 0; i < keysList.Count; i++)
			{
				yield return new KeyValuePair<TKey, TValue>(keysList[i], valuesList[i]);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
