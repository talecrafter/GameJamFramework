using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
    public class ActorTarget
    {
        // ================================================================================
        //  types
        // --------------------------------------------------------------------------------

        const float PATH_NODE_DISTANCE_SQUARED = 0.05f;

        public enum TargetType
        {
            Position,
            Transform,
            Actor,
            None
        }

        public TargetType type;

        // ================================================================================
        //  events
        // --------------------------------------------------------------------------------

        public enum TargetEventType
        {
            TargetReached,
            CannotReachTarget
        }

        public delegate void TargetEventDelegate(TargetEventType type);
        public event TargetEventDelegate targetEvent;

        // ================================================================================
        //  public
        // --------------------------------------------------------------------------------

        private bool _isReached = true;
		public bool isReached { get { return _isReached; } }

        public bool hasTarget
        {
            get
            {
                return type != TargetType.None;
            }
        }

        public bool hasActorTarget
        {
            get
            {
                return type == TargetType.Actor;
            }
        }

        // ================================================================================
        //  private
        // --------------------------------------------------------------------------------

        private Transform _protagonistTransform;
        private Actor2D _protagonist;

        private Vector2 _targetPosition;
        private Transform _targetTransform;

        private Actor2D _otherActor;
        public Actor2D otherActor
        {
            get
            {
                return _otherActor;
            }
        }

        private float _targetReachedDistanceSquared; // saving squared value because of performance reasons when comparing vector length
		private float _targetReachedDistance;
		public float targetReachedDistance
		{
			get { return _targetReachedDistance; }
			private set
			{
				_targetReachedDistance = value;
				_targetReachedDistanceSquared = value * value;
			}
		}
        private bool _determined = false;

		private IPathField _pathField = null;
        private Vector2Path _path = null;
        private bool _hasPath = false;

        private int _ticker = 0;
        private int _maxTicker = 10;

        private bool _calculatePathAtNextPossibility = false;

        // ================================================================================
        //  constructor
        // --------------------------------------------------------------------------------

        public ActorTarget(Actor2D protagonist, Transform protagonistTransform)
        {
            _protagonist = protagonist;
            _protagonistTransform = protagonistTransform;

            _ticker = Random.Range(0, _maxTicker);

            _path = new Vector2Path(80);
        }

        // ================================================================================
        //  public methods
        // --------------------------------------------------------------------------------

        public void Update()
        {
            // make sure we follow no null target enemy
            if (type == TargetType.Actor && _otherActor == null)
            {
                DisableTarget();
                return;
            }

            // performance heavy calculations only every few frames
            if (!_isReached)
                PerformanceTicker();

            if (type != TargetType.None)
            {
                Vector2 targetPos = GetFinalTargetPosition();

                if (Utilities2D.Vector2SqrDistance(_protagonistTransform.position, targetPos) <= _targetReachedDistanceSquared)
                {
                    ReachTarget();
				}
                else
                {
                    _isReached = false;					
                }

                // check for next target on path
                if (_hasPath && !_isReached)
                {
                    if (Utilities2D.Vector2SqrDistance(_protagonistTransform.position, GetCurrentTargetLocation()) <= PATH_NODE_DISTANCE_SQUARED)
                    {
                        NextNodeInPath();
                        if (!_hasPath)
                        {
							_calculatePathAtNextPossibility = false;
                            ReachTarget();
                        }
                    }
                }
            }
        }

        public Vector2 GetFinalTargetPosition()
        {
            Vector2 targetPos = _targetPosition;

            if (type == TargetType.Transform || type == TargetType.Actor)
            {
                targetPos = _targetTransform.position;
            }

            return targetPos;
        }

        public Vector2 GetCurrentTargetLocation()
        {
            if (_hasPath && !_path.hasFinished)
            {
                return _path.CurrentPosition;
            }
            else
            {
				return GetFinalTargetPosition();
            }
        }

        // set position as target
        public void SetTarget(Vector2 targetPos, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false)
            {
                return;
            }

            DisableTarget();

            type = TargetType.Position;
            _targetPosition = targetPos;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            _calculatePathAtNextPossibility = true;
        }

        // set object as target
        public void SetTarget(Transform targetTransform, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false)
            {
                return;
            }

            DisableTarget();

            type = TargetType.Transform;
            _targetTransform = targetTransform;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            _calculatePathAtNextPossibility = true;
        }

        // set other Actor as target
        public void SetTarget(Actor2D otherActor, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false)
            {
                return;
            }
            _determined = newDetermination;

            DisableTarget();

            if (otherActor == null)
                return;

            type = TargetType.Actor;
            _otherActor = otherActor;
            _targetTransform = otherActor.transform;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            _otherActor.stateChanged += Actor_StateChanged;

            _calculatePathAtNextPossibility = true;
        }

        public void DisableTarget()
        {
            type = TargetType.None;

            if (_otherActor != null)
            {
                _otherActor.stateChanged -= Actor_StateChanged;
                _otherActor = null;
            }
            _targetTransform = null;

            // path parameters
            _hasPath = false;
            _calculatePathAtNextPossibility = false;

            _isReached = true;

            _determined = false;
        }

		public void SetPathField(IPathField field)
		{
			DisableTarget();
			_pathField = field;
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void PerformanceTicker()
		{
			_ticker++;
			if (_ticker >= _maxTicker)
			{
				_ticker = 0;

				if (_calculatePathAtNextPossibility)
				{
					_calculatePathAtNextPossibility = false;

					CalculatePath();
				}
			}
		}

		private void ReachTarget()
		{
			_isReached = true;

			// target reached
			if (type == TargetType.Position || type == TargetType.Transform)
			{
				DisableTarget();
			}

			if (targetEvent != null)
			{
				targetEvent(TargetEventType.TargetReached);
			}
		}

		private void CalculatePath()
        {
            // if level grid exists and actor can move, otherwise ignore
            if (_pathField != null && _protagonist.movementSpeed > 0)
            {
				_pathField.GetPath(_protagonistTransform.position, GetFinalTargetPosition(), _path);

                _hasPath = _path.isValid;
				if (!_hasPath)
				{
					if (type == TargetType.Position)
					{
						ReachTarget();
					}
				}
            }
        }

        private void NextNodeInPath()
        {
            if (_hasPath && _path.isValid)
            {
                _path.NextPosition();
            }
			if (!_path.isValid || _path.hasFinished)
            {
                _hasPath = false;
            }
        }

        private void Actor_StateChanged(IActor actor, ActorState state)
        {
            if (!actor.isAlive)
            {
                DisableTarget();

                if (targetEvent != null)
                {
                    targetEvent(TargetEventType.CannotReachTarget);
                }
            }
        }
    }

}