using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System;

namespace CraftingLegends.Framework
{
	public class GameStateData
	{
		public HashSet<int> wasUsed = new HashSet<int>();

		#region loading and saving
		// ================================================================================
		//  loading and saving
		// --------------------------------------------------------------------------------

		public JSONObject GetJSON()
		{
			JSONObject data = new JSONObject();

			JSONArray wasUsedArray = new JSONArray();
			foreach (var item in wasUsed)
			{
				wasUsedArray.Add(item);
			}
			data.Add("wasUsed", wasUsedArray);

			return data;
		}

		public void FromJSON(JSONObject data)
		{
			// clear old values
			Reset();

			JSONArray wasUsedArray = data.GetArray("wasUsed");
			for (int i = 0; i < wasUsedArray.Length; i++)
			{
				wasUsed.Add((int)wasUsedArray[i].Number);
			}
		}

		public void Reset()
		{
			wasUsed.Clear();
		}

		#endregion
	}
}