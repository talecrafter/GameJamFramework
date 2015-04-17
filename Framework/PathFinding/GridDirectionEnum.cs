using UnityEngine;

namespace CraftingLegends.Framework
{
	public enum GridDirection : int
	{
		None = -1,
		North = 0,
		East = 1,
		South = 2,
		West = 3
	}

	public static class GridDirectionUtilities
	{
		public static GridDirection randomDirection
		{
			get
			{
				int dir = Random.Range(0, 4);

				switch (dir)
				{
					case 0:
						return GridDirection.North;
					case 1:
						return GridDirection.East;
					case 2:
						return GridDirection.South;
					case 3:
						return GridDirection.West;
					default:
						return GridDirection.North;
				}
			}
		}

		public static GridDirection Opposite(this GridDirection direction)
		{
			switch (direction)
			{
				case GridDirection.North:
					return GridDirection.South;
				case GridDirection.East:
					return GridDirection.West;
				case GridDirection.South:
					return GridDirection.North;
				case GridDirection.West:
					return GridDirection.East;
			}

			return GridDirection.North;
		}
	}
}