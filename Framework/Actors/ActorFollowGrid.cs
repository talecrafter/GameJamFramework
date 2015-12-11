using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ActorFollowGrid : MonoBehaviour
	{
		void Start()
		{
			Actor2D actor = GetComponent<Actor2D>();
			actor.target.SetPathField(FindObjectOfType<LevelGrid>());
		}
	}
}