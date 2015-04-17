using UnityEngine;
using System.Collections;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class RenderDepthUpdate : MonoBehaviour
	{
		// ================================================================================
		//  public
		// --------------------------------------------------------------------------------

		public bool toBackgroundWhenDead = false;

		public float offset = 0f;

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		// state
		private bool _isActive = true;

		private Transform _transform;
		private Actor _actor;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_transform = transform;

			_actor = GetComponent<Actor>();
			if (_actor != null)
				_actor.stateChanged += ActorStateChanged;
		}

		void Update()
		{
			if (_isActive)
				_transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y + offset);
		}

		public Vector3 UpdatePosition(Vector3 current)
		{
			return new Vector3(current.x, current.y, current.y + offset);
		}

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private void ActorStateChanged(IActor activeActor, ActorState state)
		{
			if (state == ActorState.Dead && toBackgroundWhenDead == true)
			{
				// set a bit further towards background
				_transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y + 1.0f + offset);

				_isActive = false;
			}

			// reset depth rendering when actor is active again
			if (activeActor.isAlive && !_isActive)
				_isActive = true;
		}
	}
}