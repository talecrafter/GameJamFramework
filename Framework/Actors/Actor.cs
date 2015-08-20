using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;
using System;

namespace CraftingLegends.Framework
{
	public class Actor : MonoBehaviour, IPooledObject, IActor, IHasHealth
	{
		#region declarations and events

		public const float MINIMUM_MOVEMENT_FOR_ANIMATIONS = 0.01f;
		public const float TARGET_DISTANCE = 0.7f;
		public const float TIME_UNTIL_DESTRUCTION = 10.0f;

		// ================================================================================
		//  declarations
		// --------------------------------------------------------------------------------

		public delegate Vector2 GetEstimatedFuturePositionDelegate(float time);
		public GetEstimatedFuturePositionDelegate GetEstimatedFuturePosition;

		public delegate bool HasPossibleTargetsInRangeDelegate();
		public HasPossibleTargetsInRangeDelegate HasPossibleTargetsInRange = delegate { return false; };

		public delegate void DeathExecutionDelegate();
		public DeathExecutionDelegate deathExecutionHandler;

		public delegate void WasDamagedDelegate();
		public event WasDamagedDelegate wasDamaged;

		// ================================================================================
		//  state and events
		// --------------------------------------------------------------------------------

		public event StateChanged stateChanged;

		#endregion

		#region state
		[SerializeField]
		private ActorState _state = ActorState.Idle;
		public ActorState state
		{
			get
			{
				return _state;
			}
			private set
			{
				if (_transform == null)
					return;

				if (_state == value)
					return;

				_state = value;

				if (stateChanged != null)
				{
					stateChanged(this, _state);
				}

				if (_state == ActorState.Disabled)
				{
					if (isDisabled != null)
						isDisabled(this);
				}

			}
		}

		#endregion

		#region unit stats, health
		// ================================================================================
		//  stats
		// --------------------------------------------------------------------------------

		[SerializeField]
		private float _maxHealth = 10.0f;

		private Energy _health;
		public Energy health
		{
			get
			{
				if (_health == null)
					health = new Energy(_maxHealth);

				return _health;
			}
			set
			{
				_health = value;
			}
		}

		public bool needsHealing
		{
			get
			{
				return isAlive && !health.isFull;
			}
		}

		public float movementSpeed = 6.0f;

		[HideInInspector]
		public Target target;

		public Vector2 lookDirection = Vector2.zero;

		public Transform actionPivot;

		// ================================================================================
		//  getters and setters
		// --------------------------------------------------------------------------------

		public Vector2 position2D
		{
			get
			{
				return _transform.position;
			}
		}

		public Vector3 position
		{
			get
			{
				return _transform.position;
			}
		}

		public bool isAlive
		{
			get
			{
				return !(state == ActorState.Dead || state == ActorState.Disabled);
			}
		}

		public bool isEnabled
		{
			get
			{
				return !(state == ActorState.Disabled);
			}
		}

		public bool isReady
		{
			get
			{
				return (state == ActorState.Idle || state == ActorState.Moving);
			}
		}

		#endregion

		#region actions

		public IActorTimedAction action;

		#endregion

		#region display
		// ================================================================================
		//  display
		// --------------------------------------------------------------------------------

		public Texture2D interfacePortrait;
		public GameObject displayObject;
		public bool directionRight = true;
		public bool canFlip = true;

		[HideInInspector]
		public bool isMoving = false;

		private IAnimationController _animationController;

		// variables for showing a hit
		private Timer _hitTimer = null;
		private Color _originalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		#endregion

		#region helper
		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private Transform _transform;
		private Rigidbody2D _rigidbody2D;
		private Collider2D _collider2D;
		private Timer _actionTimer = null;

		private Vector3 _lastMovement = Vector3.zero;
		#endregion

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		#region unity methods

		void Awake()
		{
			_transform = transform;
			_rigidbody2D = GetComponent<Rigidbody2D>();
			_collider2D = GetComponent<Collider2D>();
			_animationController = _transform.GetInterface<IAnimationController>();
			target = new Target(this, _transform);

			health = new Energy(_maxHealth);

			// set callback functions
			GetEstimatedFuturePosition = EstimateFuturePosition;
			deathExecutionHandler = DestroyAtDeath;

			Reset();
		}

		void FixedUpdate()
		{
			// do nothing when game is not running
			if (MainBase.Instance == null ||
				(MainBase.Instance.state != GameState.Running && MainBase.Instance.state != GameState.Sequence))
				return;

			// slow movement
			if (_rigidbody2D != null)
			{
				if (_rigidbody2D.velocity.magnitude < 0.005f)
				{
					_rigidbody2D.velocity = Vector2.zero;
					lookDirection = Vector2.zero;
				}
				else
				{
					_rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity, Vector2.zero, Time.fixedDeltaTime * 5.0f);
				}
			}

			// do nothing when dead
			if (state == ActorState.Disabled || state == ActorState.Dead)
				return;

			// timer for hit visuals
			if (_hitTimer != null && isAlive)
			{
				_hitTimer.Update();

				if (_hitTimer.hasEnded)
				{
					HideDamageDisplay();
				}
			}

			// update target
			target.Update();

			// update movement
			if (state == ActorState.Moving)
			{
				if (!target.hasTarget)
				{
					isMoving = false;
					state = ActorState.Idle;
					return;
				}
				else
				{
					MoveTowardsTarget();
				}
			}

			// update action
			if (_actionTimer != null)
			{
				_actionTimer.Update();
				if (_actionTimer.hasEnded)
				{
					state = ActorState.Idle;
					_actionTimer = null;
				}
				else
				{
					return; // wait for actions to finish
				}
			}

			// resume actions when idle
			if (state == ActorState.Idle && target.hasTarget
				//&& !(weapon.type == WeaponType.Healing && target.otherActor != null && target.otherActor.health >= target.otherActor.maxHealth)
				)
			{
				state = ActorState.Moving;
				return;
			}
		}

		#endregion

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		#region state manipulation

		public void MakeInActive()
		{
			target.DisableTarget();

			state = ActorState.Disabled;
		}

		public void Disable()
		{
			target.DisableTarget();

			state = ActorState.Disabled;

			if (!_isInactiveInObjectPool)
				Destroy(gameObject);
		}

		public void Kill()
		{
			ApplyDamage(health.current);
		}

		public void SetToDead()
		{
			health.EmptySilently();

			state = ActorState.Dead;
		}

		public void Reset()
		{
			health.Reset();
			if (target != null)
				target.DisableTarget();

			if (_collider2D != null)
				_collider2D.enabled = true;

			if (_animationController != null)
				_animationController.Reset();

			HideDamageDisplay();

			state = ActorState.Idle;
		}

		// decrease health, can be called from outside
		public void ApplyDamage(float damage)
		{
			if (!isAlive)
				return;

			health.Lose(damage);

			if (health.isEmpty)
			{
				Die();
			}
			else
			{
				ShowDamageDisplay(0.15f, new Color(1.0f, 0.4f, 0.4f, 1.0f)); // red color
			}

			if (wasDamaged != null)
				wasDamaged();
		}

		// increase health, can be called from outside
		public void ApplyHealing(float amount)
		{
			if (!isAlive)
				return;

			if (health.isFull)
				return;

			health.Add(amount);

			//ShowDamageDisplay(0.3f, new Color(0.58f, 0.91f, 0.82f)); // turquoise color
		}

		#endregion

		#region set targets

		// transform as new target, e.g. walking to a building, flag
		public void SetTarget(Transform newTarget, float distance = TARGET_DISTANCE, bool determined = false)
		{
			if (!isAlive)
				return;

			target.SetTarget(newTarget, distance, determined);

			if (state == ActorState.Idle)
				state = ActorState.Moving;
		}

		// position as new target
		public void SetTarget(Vector2 position, float distance, bool determined = false)
		{
			if (!isAlive)
				return;

			target.SetTarget(position, distance, determined);

			if (state == ActorState.Idle)
				state = ActorState.Moving;
		}

		// other actor as new target, e.g. enemy or friendly unit to be healed
		public void SetTarget(Actor otherActor, bool determined = false)
		{
			if (!isAlive)
				return;

			if (action != null)
			{
				target.SetTarget(otherActor, action.range * 0.9f, determined);
			}

			if (state == ActorState.Idle)
				state = ActorState.Moving;
		}

		public void SetMovement(Vector2 moveDirection)
		{
			if (!isAlive)
				return;

			target.DisableTarget();

			if (moveDirection == Vector2.zero)
				return;

			if (moveDirection.sqrMagnitude > 1.0f)
				moveDirection = moveDirection.normalized;

			Move(moveDirection);
			if (moveDirection.x > 0)
				SetDisplayDirection(true);
			else if (moveDirection.x < 0)
				SetDisplayDirection(false);

			if (state == ActorState.Idle)
				state = ActorState.Moving;
		}

		public void HaltMovement()
		{
			if (isAlive)
			{
				isMoving = false;
				target.DisableTarget();

				if (state == ActorState.Moving)
					state = ActorState.Idle;
			}
		}

		public void TakeAction(Actor targetActor)
		{
			if (!isAlive)
				return;

			_actionTimer = new Timer(action.cooldown);
			isMoving = false;

			// update look direction
			SetLookDirectionToTarget(targetActor.position2D);

			state = ActorState.TakingAction;

			action.Execute();
		}

		public void TakeAction(IEnumeratedAction enumeratedAction)
		{
			isMoving = false;
			state = ActorState.TakingAction;

			StartCoroutine(StartAction(enumeratedAction));
		}

		private IEnumerator StartAction(IEnumeratedAction enumeratedAction)
		{
			yield return StartCoroutine(enumeratedAction.Execute());
			state = ActorState.Idle;
		}

		#endregion

		#region helper methods

		public Actor GetNearest(List<Actor> otherActors)
		{
			otherActors.Sort(
				(firstObject, secondObject) =>
				Vector2.SqrMagnitude(secondObject.position2D - position2D).CompareTo(Vector2.SqrMagnitude(firstObject.position2D - position2D)));

			return otherActors.First();
		}

		#endregion

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		#region movement and actions

		/// <summary>
		/// calculate avoidance on collision
		/// </summary>
		/// <param name="coll"></param>
		void OnCollisionEnter2D(Collision2D coll)
		{
			// only relevant for moving objects which have a target
			if (movementSpeed <= 0 || _rigidbody2D == null || target == null || !target.hasTarget)
				return;

			Vector2 targetLocation = target.GetCurrentTargetLocation();
			Vector2 moveDirection = Utilities2D.GetNormalizedDirection(position2D, targetLocation);

			if (moveDirection.sqrMagnitude <= 0.03f)
				return;
			moveDirection = moveDirection.normalized;

			// get contact
			ContactPoint2D contactPoint = coll.contacts[0];

			// calculate force to the right or to the left
			Vector2 toTheRight = new Vector2(moveDirection.y, -moveDirection.x);
			Vector2 toTheLeft = -toTheRight;
			float angle = Vector2.Angle(contactPoint.normal, toTheRight);
			Vector2 avoidanceVector;
			if (angle > 90.0f)
				avoidanceVector = toTheLeft;
			else
				avoidanceVector = toTheRight;

			//Debug.DrawRay(contactPoint.point, avoidanceVector, Color.red, 1.0f);

			_rigidbody2D.AddForce(avoidanceVector * 30.0f);
		}

		private void MoveTowardsTarget()
		{
			// check if target reached
			if (target.isReached)
			{
				// attack if possible
				if (target.type == Target.TargetType.Actor && TargetInReach()
					&& action != null)
				{
					TakeAction(target.otherActor);
					return;
				}

				HaltMovement(); // TODO: these two lines were disabled in Indomitable - why?
				return;
			}

			// get target direction
			Vector2 targetLocation = target.GetCurrentTargetLocation();
			Vector2 moveDirection = Utilities2D.GetNormalizedDirection(position2D, targetLocation);

			Move(moveDirection);

			// update look direction
			SetLookDirectionToTarget(targetLocation);
		}

		// Hack; should not be necessary to double check this
		private bool TargetInReach()
		{
			Vector2 direction = position2D - target.otherActor.position2D;
			return direction.magnitude <= action.range;
		}

		private void Move(Vector2 moveDirection)
		{
			//// movement with physic forces
			//Vector2 movement = moveDirection * speed * Time.fixedDeltaTime;
			//if (_rigidbody2D != null)
			//{
			//	_rigidbody2D.AddForce(movement * 180.0f);
			//}

			// movement alternative 1
			Vector3 movement = moveDirection * movementSpeed;
			_rigidbody2D.velocity = movement;
			// also uncomment slowing of velocity in Update

			// movement alternative 2
			//Vector3 movement = moveDirection * speed * Time.deltaTime;
			//_transform.Translate(movement);

			//Debug.DrawLine(_transform.position, _transform.position + new Vector3(moveDirection.x, moveDirection.y, 0), Color.red);

			// mark controller as moving or not (for animations)
			if (movement.magnitude > MINIMUM_MOVEMENT_FOR_ANIMATIONS)
			{
				lookDirection = movement;
				isMoving = true;
			}
			else
			{
				isMoving = false;
			}

			_lastMovement = movement; // save movement value to calculate future position for targeting this Actor
		}

		private Vector2 EstimateFuturePosition(float time)
		{
			Vector2 futurePos = position2D;

			if (isAlive && isMoving)
			{
				Vector2 direction = _lastMovement.normalized;

				if (target.hasTarget)
				{
					direction = Utilities2D.GetNormalizedDirection(futurePos, target.GetFinalTargetPosition());
				}

				futurePos = futurePos + direction * time * movementSpeed;
			}

			return futurePos;
		}

		private void Die()
		{
			// reset hit display
			HideDamageDisplay();

			state = ActorState.Dead;

			deathExecutionHandler();
		}

		private void DestroyAtDeath()
		{
			if (_animationController != null)
				_animationController.FadeOut();

			// disable components
			if (_collider2D != null)
				_collider2D.enabled = false;

			float timeUntilDestroy = TIME_UNTIL_DESTRUCTION; // deathtime
			// expand time if there is a possibility of gun launch
			//			if (weapon != null && weapon.type == WeaponType.Artillery && (weapon.rangeDelay * 1.1f > timeUntilDestroy))
			//			{
			//				timeUntilDestroy = weapon.rangeDelay * 1.1f;
			//			}

			if (_isInactiveInObjectPool)
			{
				StartCoroutine(WaitAndDisable(timeUntilDestroy));
			}
			else
				Destroy(gameObject, timeUntilDestroy);
		}

		private IEnumerator WaitAndDisable(float time)
		{
			yield return new WaitForSeconds(time);
			Disable();
		}

		#endregion

		#region display methods

		public void SetDisplayDirection(bool toTheRight)
		{
			if (toTheRight != directionRight)
			{
				FlipDirection();
			}
		}

		private void SetLookDirectionToTarget(Vector2 targetLocation)
		{
			// check direction for display purposes
			if (targetLocation.x > _transform.position.x)
				SetDisplayDirection(true);
			if (targetLocation.x < _transform.position.x)
				SetDisplayDirection(false);
		}

		private void FlipDirection()
		{
			directionRight = !directionRight;

			if (!canFlip)
				return;

			// change visual avatar
			if (displayObject != null)
			{
				Transform displayTransform = displayObject.transform;
				displayTransform.localScale = new Vector3(displayTransform.localScale.x * -1.0f, displayTransform.localScale.y, displayTransform.localScale.z);
			}
		}

		private void ShowDamageDisplay(float time, Color color)
		{
			_hitTimer = new Timer(time);

			if (_animationController != null)
				_animationController.SetMaterialColor(color);
		}

		private void HideDamageDisplay()
		{
			if (_hitTimer != null)
			{
				_hitTimer = null;

				if (_animationController != null)
					_animationController.SetMaterialColor(_originalColor);
			}
		}

		#endregion

		#region ITogglable implementation

		public void ToggleOn()
		{
			gameObject.SetActive(true);
			Reset();
		}

		public void ToggleOff()
		{
			StopAllCoroutines();
			Disable();
			gameObject.SetActive(false);
		}

		public event System.Action<IPooledObject> isDisabled;

		private bool _isInactiveInObjectPool = false;
		public bool isUsedByObjectPool
		{
			get
			{
				return _isInactiveInObjectPool;
			}
			set
			{
				_isInactiveInObjectPool = value;
			}
		}

		#endregion
	}
}