using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class Actor : MonoBehaviour, IPooledObject, IActor
	{
		#region declarations and events

		public const float MINIMUM_MOVEMENT_FOR_ANIMATIONS = 0.01f;
		public const float TARGET_DISTANCE = 0.7f;
		public const float TIME_UNTIL_DESTRUCTION = 20.0f;

		// ================================================================================
		//  declarations
		// --------------------------------------------------------------------------------

		public delegate Vector2 GetEstimatedFuturePositionDelegate(float time);
		public GetEstimatedFuturePositionDelegate GetEstimatedFuturePosition;

		public delegate bool HasPossibleTargetsInRangeDelegate();
		public HasPossibleTargetsInRangeDelegate HasPossibleTargetsInRange = delegate { return false; };

		public delegate void DeathExecutionDelegate();
		public DeathExecutionDelegate deathExecutionHandler;

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

		public float maxHealth = 10.0f;
		public float health = 10.0f;
		private float _startHealth;

		public Weapon weapon;
		public float movementSpeed = 6.0f;

		[HideInInspector]
		public Target target;

		public Vector2 lookDirection = Vector2.zero;

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
		private Actor _targetActor = null;
		#endregion

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		#region unity methods

		void Awake()
		{
			_transform = transform;
			_rigidbody2D = rigidbody2D;
			_collider2D = collider2D;
			_animationController = _transform.GetInterface<IAnimationController>();
			target = new Target(this, _transform);

			// set callback functions
			GetEstimatedFuturePosition = EstimateFuturePosition;
			deathExecutionHandler = DestroyAtDeath;

			if (health > 0)
			{
				_startHealth = health;
			}
			else
			{
				_startHealth = maxHealth;
			}

			Reset();
		}

		void FixedUpdate()
		{
			// do nothing when game is not running
			if (BaseGameController.Instance.state != GameState.Running && BaseGameController.Instance.state != GameState.Sequence)
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

			// resume actions when idle
			if (state == ActorState.Idle && target.hasTarget
				&& !(weapon.type == Weapon.WeaponType.Healing && target.otherActor != null && target.otherActor.health >= target.otherActor.maxHealth))
			{
				state = ActorState.Moving;
				return;
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
			}
		}

		#endregion

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		#region state manipulation

		public void Disable()
		{
			target.DisableTarget();

			state = ActorState.Disabled;

			if (!_isInactiveInObjectPool)
				Destroy(gameObject);
		}

		public void SetToDead()
		{
			health = 0;

			state = ActorState.Dead;
		}

		public void Reset()
		{
			health = _startHealth;
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

			health -= damage;

			if (health <= 0)
			{
				health = 0;
				Die();
			}
			else
			{
				ShowDamageDisplay(0.15f, new Color(1.0f, 0.4f, 0.4f, 1.0f)); // red color
			}
		}

		// increase health, can be called from outside
		public void ApplyHealing(float amount)
		{
			if (!isAlive)
				return;

			if (health >= maxHealth)
				return;

			health += amount;

			if (health > maxHealth)
			{
				health = maxHealth;
			}

			ShowDamageDisplay(0.3f, new Color(0.58f, 0.91f, 0.82f)); // turquoise color
		}

		#endregion

		#region set targets

		// transform as new target, e.g. walking to a building, flag
		public void SetTarget(Transform newTarget, bool determined = false)
		{
			if (!isAlive)
				return;

			target.SetTarget(newTarget, TARGET_DISTANCE, determined);

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

			if (weapon != null)
			{
				target.SetTarget(otherActor, weapon.range * 0.9f, determined);
			}

			if (state == ActorState.Idle)
				state = ActorState.Moving;
		}

		public void SetMovement(Vector2 moveDirection)
		{
			target.DisableTarget();

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

		#endregion

		#region animation callbacks

		public void ExecuteAttack()
		{
			if (!isAlive)
				return;

			if (_targetActor == null)
				return;

			// melee
			if (weapon.type == Weapon.WeaponType.Melee)
			{
				_targetActor.ApplyDamage(weapon.amount);
			}
			else if (weapon.type == Weapon.WeaponType.Healing)
			{
				_targetActor.ApplyHealing(weapon.amount);
			}
			else // range
			{
				// calculate target position from character movement
				Vector2 targetPos = _targetActor.GetEstimatedFuturePosition(weapon.rangeDelay + 0.3f);   // 0.5f => estimated time from shell spawn to hit

				// check max distance
				Vector2 direction = targetPos - position2D;
				if (direction.magnitude > weapon.range)
				{
					targetPos = position2D + direction.normalized * weapon.range;

					// do not follow target anymore
					target.DisableTarget();
				}

				// randomize target location by weapon precision
				if (weapon.precision < 1.0f)
				{
					targetPos += Random.insideUnitCircle * (1.0f - weapon.precision) * 10.0f;
				}

				// create shell
				StartCoroutine(CreateShellObject(targetPos, weapon.shellPrefab, weapon.rangeDelay));
			}
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
			if (movementSpeed <= 0 || _rigidbody2D == null || !target.hasTarget)
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
				if (target.type == Target.TargetType.Actor)
				{
					Attack(target.otherActor);
					return;
				}

				HaltMovement();
				return;
			}

			// get target direction
			Vector2 targetLocation = target.GetCurrentTargetLocation();
			Vector2 moveDirection = Utilities2D.GetNormalizedDirection(position2D, targetLocation);

			Move(moveDirection);

			// update look direction
			SetLookDirectionToTarget(targetLocation);
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

			Debug.DrawLine(_transform.position, _transform.position + new Vector3(moveDirection.x, moveDirection.y, 0), Color.red);

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
		}

		private void Attack(Actor enemy)
		{
			if (!isAlive)
				return;

			if (weapon == null)
			{
				Debug.Log("Actor " + gameObject.name + " tries to attack but has no weapon");
			}

			_actionTimer = new Timer(weapon.attackDuration);
			isMoving = false;
			_targetActor = enemy;

			// update look direction
			SetLookDirectionToTarget(enemy.position2D);

			state = ActorState.TakingAction;

			// now wait for animation to trigger Execution
		}

		private Vector2 EstimateFuturePosition(float time)
		{
			Vector2 futurePos = position2D;

			if (isAlive && isMoving && target.hasTarget)
			{
				Vector2 direction = Utilities2D.GetNormalizedDirection(futurePos, target.GetFinalTargetPosition());
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
			if (weapon != null && weapon.type == Weapon.WeaponType.Artillery && (weapon.rangeDelay * 1.1f > timeUntilDestroy))
			{
				timeUntilDestroy = weapon.rangeDelay * 1.1f;
			}

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

		#region helper methods

		private IEnumerator CreateShellObject(Vector2 targetPos, GameObject prefab, float delay)
		{
			yield return new WaitForSeconds(delay);
			Vector3 shellPosition = new Vector3(targetPos.x, targetPos.y, targetPos.y);
			Instantiate(prefab, shellPosition, Quaternion.identity);
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
		public bool isInactiveInObjectPool
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