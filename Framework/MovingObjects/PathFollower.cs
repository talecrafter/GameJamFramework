using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
	public class PathFollower : MonoBehaviour
	{
		public enum FollowMoveType
		{
			MoveTowards,
			Lerp
		}

		public FollowMoveType followType = FollowMoveType.MoveTowards;

		public PathDefinition path;

		public float speed = 5;
		public float maxDistance = 0.1f;

		private IEnumerator<Transform> _currentPoint;

		public void Awake()
		{
			if (path == null)
			{
				Debug.LogError("PathFollower needs a PathDefinition", gameObject);
				return;
			}

			_currentPoint = path.GetPathEnumerator();
			_currentPoint.MoveNext();

			if (_currentPoint.Current != null)
				transform.position = _currentPoint.Current.position;
		}

		public void Update()
		{
			if (_currentPoint == null || _currentPoint.Current == null)
				return;

			if (followType == FollowMoveType.MoveTowards)
				transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
			else
				transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);

			float distanceSquared = (_currentPoint.Current.position - transform.position).sqrMagnitude;

			if (distanceSquared < maxDistance * maxDistance)
				_currentPoint.MoveNext();
		}
	}
}
