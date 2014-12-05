using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class ActorFollowGrid : MonoBehaviour
	{
		void Start()
		{
			Actor actor = GetComponent<Actor>();
			actor.target.SetPathField(FindObjectOfType<LevelGrid>());
		}
	}
}