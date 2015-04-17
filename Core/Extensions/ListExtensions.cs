using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Core
{
	public static class ListExtensions
	{
		public static T PickRandom<T>(this List<T> source)
		{
			if (source.Count == 0)
				return default(T);

			int i = Random.Range(0, source.Count);
			return source[i];
		}

		public static List<T> PickRandom<T>(this List<T> source, int count)
		{
			if (source.Count == 0)
				return new List<T>();

			if (count > source.Count)
				count = source.Count;

			List<T> sourceCopy = source.Clone();
			List<T> selection = new List<T>();

			for (int i = 0; i < count; i++) {
				selection.Add(sourceCopy.PopRandom());
			}

			return selection;
		}

		public static T PopRandom<T>(this List<T> source)
		{
			if (source.Count == 0)
				return default(T);

			int i = Random.Range(0, source.Count);
			T value = source[i];
			source.RemoveAt(i);

			return value;
		}

		public static T First<T>(this List<T> source)
		{
			if (source.Count == 0)
				return default(T);

			return source[0];
		}

		public static T Last<T>(this List<T> source)
		{
			if (source.Count == 0)
				return default(T);

			return source[source.Count - 1];
		}

		public static T PopLast<T>(this List<T> source)
		{
			if (source.Count == 0)
				return default(T);

			T value = source.Last();
			source.RemoveAt(source.Count - 1);

			return value;
		}

		public static List<T> Clone<T>(this List<T> source)
		{
			List<T> newList = new List<T>();

			for (int i = 0; i < source.Count; i++)
			{
				newList.Add(source[i]);
			}

			return newList;
		}

		// randomizes the order of a generic list
		public static void Shuffle<T>(this IList<T> list)
		{
			int index = list.Count;
			while (index > 1)
			{
				index--;
				int newPos = Random.Range(0, index + 1);
				T temp = list[newPos];
				list[newPos] = list[index];
				list[index] = temp;
			}
		}
	}
}