using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class Sensors : MonoBehaviour
	{
		// ================================================================================
		//  Sensor Events
		// --------------------------------------------------------------------------------

		public delegate void SensorEventDelegate(SensorEvent type, Actor otherActor);
		public event SensorEventDelegate sensorEvent;

		// ================================================================================
		//  public
		// --------------------------------------------------------------------------------

		public string Tag = "Player";

		public List<Actor> actors = new List<Actor>();

		public bool ActorsDetected
		{
			get
			{
				return actors.Count > 0;
			}
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		Transform _transform;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_transform = transform;
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == Tag)
			{
				Actor actor = other.GetComponent<Actor>();
				if (actor != null && !actors.Contains(actor) && actor.isAlive)
				{
					actors.Add(actor);

					actor.stateChanged += Actor_StateChanged;

					if (sensorEvent != null)
						sensorEvent(SensorEvent.ActorDetected, actor);
				}
			}
		}

		void Actor_StateChanged(IActor actor, ActorState state)
		{
			if (!actor.isAlive)
			{
				if (actors.Contains(actor as Actor))
				{
					RemoveActor(actor as Actor);
				}
				else
				{
					actor.stateChanged -= Actor_StateChanged;
				}
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (other.tag == Tag)
			{
				Actor actor = other.GetComponent<Actor>();
				RemoveActor(actor);
			}
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public Actor GetActorWithLowestHitPoints()
		{
			UpdateList();

			if (actors.Count == 0)
			{
				return null;
			}

			Actor targetActor = actors[0];
			float healthAmount = targetActor.maxHealth - targetActor.health;

			for (int i = 1; i < actors.Count; i++)
			{
				float newHealthAmount = actors[i].maxHealth - actors[i].health;
				if (newHealthAmount > healthAmount)
				{
					healthAmount = newHealthAmount;
					targetActor = actors[i];
				}
			}

			return targetActor;
		}

		public Actor GetNearestActor()
		{
			UpdateList();

			if (actors.Count == 0)
			{
				return null;
			}

			Vector2 position = _transform.position;
			Actor nearestActor = actors[0];
			float foundDistance = (position - nearestActor.position2D).sqrMagnitude;

			for (int i = 1; i < actors.Count; i++)
			{
				float newDistance = (position - actors[i].position2D).sqrMagnitude;
				if (newDistance < foundDistance)
				{
					foundDistance = newDistance;
					nearestActor = actors[i];
				}
			}

			return nearestActor;
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void RemoveActor(Actor actor)
		{
			if (actor != null && actors.Contains(actor))
			{
				actors.Remove(actor);

				actor.stateChanged -= Actor_StateChanged;

				if (sensorEvent != null)
					sensorEvent(SensorEvent.ActorLeft, actor);
			}
		}

		private void UpdateList()
		{
			for (int i = 0; i < actors.Count; i++)
			{
				if (actors[i] == null)
				{
					actors.RemoveAt(i);
					i--;
				}
			}
		}
	}
}