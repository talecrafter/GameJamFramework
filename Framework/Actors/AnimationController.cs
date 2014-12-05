using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Core;

namespace CraftingLegends.Framework
{
	public class AnimationController : MonoBehaviour, IAnimationController
	{
		// ================================================================================
		//  declarations
		// --------------------------------------------------------------------------------

		public enum AvatarAnimation
		{
			idle,
			walk,
			attack,
			die
		}

		// ================================================================================
		//  public
		// --------------------------------------------------------------------------------

		public GameObject displayObject;

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private Actor _actor;
		private Animator _animator;

		private AvatarAnimation _currentAnimation = AvatarAnimation.idle;

		private FadingTimer _fadeOutTimer = null;

		private List<Material> _materials = new List<Material>();

		private bool _isInitialized = false;

		// ================================================================================
		//  Unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_actor = GetComponent<Actor>();
			_actor.stateChanged += ActorStateChangedHandler;
			_animator = displayObject.GetComponentInChildren<Animator>();

			if (displayObject != null)
			{
				GatherRendererMaterials();
			}
			else
			{
				Debug.Log("Actor " + gameObject.name + " has AnimationController but no display object assigned");
			}

			_isInitialized = true;
		}

		void Update()
		{
			if (_animator != null)
			{
				if (BaseGameController.Instance.state == GameState.Running)
				{
					_animator.speed = 1.0f;
				}
				else
				{
					_animator.speed = 0f;
				}
			}

			// show fadeout when game is running or has ended
			if (BaseGameController.Instance.state == GameState.Running || BaseGameController.Instance.state == GameState.Ended)
			{
				if (_fadeOutTimer != null)
				{
					_fadeOutTimer.Update();
					ApplyFadeOut();

					if (_fadeOutTimer.hasEnded)
					{
						SetMaterialColor(new Color(1.0f, 1.0f, 1.0f, 0f));
					}
				}
			}
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		// sets color for all sprite children
		public void SetMaterialColor(Color color)
		{
			for (int i = 0; i < _materials.Count; i++)
			{
				_materials[i].color = color;
			}
		}

		public void FadeOut()
		{
			if (_fadeOutTimer == null)
			{
				_fadeOutTimer = new FadingTimer(0, Actor.TIME_UNTIL_DESTRUCTION, 2.0f);
			}
		}

		public void Reset()
		{
			if (_isInitialized)
			{
				if (_animator != null)
					SetAnimation(AvatarAnimation.idle);
				_currentAnimation = AvatarAnimation.idle;
				_fadeOutTimer = null;
				SetMaterialColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
			}
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		void ActorStateChangedHandler(IActor activeActor, ActorState state)
		{
			if (state == ActorState.Dead)
			{
				SetAnimation(AvatarAnimation.die);
			}
			else if (state == ActorState.TakingAction)
			{
				SetAnimation(AvatarAnimation.attack);
			}
			else if (state == ActorState.Moving)
			{
				SetAnimation(AvatarAnimation.walk);
			}
			else if (state == ActorState.Idle)
			{
				SetAnimation(AvatarAnimation.idle);
			}
		}

		private void SetAnimation(AvatarAnimation anim)
		{
			if (_animator == null)
			{
				return;
			}

			if (_currentAnimation != anim)
			{
				switch (anim)
				{
					case AvatarAnimation.idle:
						_animator.SetTrigger("idle");
						if (_animator.GetBool("walk"))
						{
							_animator.SetBool("walk", false);
						}
						break;
					case AvatarAnimation.walk:
						if (_animator.GetBool("idle"))
						{
							_animator.SetBool("idle", false);
						}
						if (_animator.GetBool("die"))
						{
							_animator.SetBool("die", false);
						}
						_animator.SetTrigger("walk");
						break;
					case AvatarAnimation.attack:
						// this is important
						// it could be that both parameters are triggered in this frame and in this case the action is more important, so we disable the other parameter
						if (_animator.GetBool("walk"))
						{
							_animator.SetBool("walk", false);
						}
						_animator.SetTrigger("attack");
						break;
					case AvatarAnimation.die:
						_animator.SetTrigger("die");

						// set all other triggers to false so Mecanim knows exactly which state to trigger
						if (_animator.GetBool("walk"))
						{
							_animator.SetBool("walk", false);
						}
						if (_animator.GetBool("idle"))
						{
							_animator.SetBool("idle", false);
						}
						if (_animator.GetBool("attack"))
						{
							_animator.SetBool("attack", false);
						}
						break;
					default:
						_animator.SetTrigger(anim.ToString());
						break;
				}

				_currentAnimation = anim;
			}
		}

		private void ApplyFadeOut()
		{
			Color color = new Color(1.0f, 1.0f, 1.0f, _fadeOutTimer.progress);
			SetMaterialColor(color);
		}

		private void GatherRendererMaterials()
		{
			_materials.Clear();
			Renderer[] rendererList = displayObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < rendererList.Length; i++)
			{
				_materials.Add(rendererList[i].material);
			}
		}
	}
}