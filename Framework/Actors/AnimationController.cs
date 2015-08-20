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
		public List<SpriteRenderer> additionalSprites;

		public List<Renderer> excludeRendererFromEffects = new List<Renderer>();

		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private Actor _actor;
		private Animator _animator;

		private AvatarAnimation _currentAnimation = AvatarAnimation.idle;

		private FadingTimer _fadeInTimer = null;
		private FadingTimer _fadeOutTimer = null;

		private List<Material> _materials = new List<Material>();

		private bool _isInitialized = false;

		// ================================================================================
		//  Unity methods
		// --------------------------------------------------------------------------------

		void Awake()
		{
			_actor = GetComponent<Actor>();
			if (_actor != null)
				_actor.stateChanged += ActorStateChangedHandler;

			if (displayObject != null)
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
			// TODO: had to comment this because of main character having no walk animation when steered by Sequence
			//if (_animator != null)
			//{
			//	if (BaseGameController.Instance.state == GameState.Running)
			//	{
			//		_animator.speed = 1.0f;
			//	}
			//	else
			//	{
			//		_animator.speed = 0f;
			//	}
			//}

			// show fadeout when game is running or has ended
			if (MainBase.Instance.state == GameState.Running || MainBase.Instance.state == GameState.Ended)
			{
				if (_fadeInTimer != null)
				{
					_fadeInTimer.Update();
					ApplyFadeIn();

					if (_fadeInTimer.hasEnded)
					{
						SetMaterialColor(new Color(1.0f, 1.0f, 1.0f, 1f));
						_fadeInTimer = null;
					}
				}
				else if (_fadeOutTimer != null)
				{
					_fadeOutTimer.Update();
					ApplyFadeOut();

					if (_fadeOutTimer.hasEnded)
					{
						SetMaterialColor(new Color(1.0f, 1.0f, 1.0f, 0f));
						_fadeOutTimer = null;
					}
				}
			}
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public void SetLookDirection(float direction)
		{
			if (direction > 0)
			{
				_animator.SetFloat("lookY", 1f);
			}
			else if (direction < 0)
			{
				_animator.SetFloat("lookY", -1f);
			}
		}

		// sets color for all sprite children
		public void SetMaterialColor(Color color)
		{
			for (int i = 0; i < _materials.Count; i++)
			{
				_materials[i].color = color;
			}

			for (int i = 0; i < additionalSprites.Count; i++)
			{
				additionalSprites[i].color = color;
			}
		}

		public void FadeIn()
		{
			SetMaterialColor(new Color(1.0f, 1.0f, 1.0f, 0f));
			_fadeInTimer = new FadingTimer(1f, 1f);
		}

		public void FadeOut()
		{
			if (_fadeOutTimer == null)
			{
				_fadeOutTimer = new FadingTimer(0, Actor.TIME_UNTIL_DESTRUCTION, 2.0f);
			}
		}

		public void FadeOutFast(float time)
		{
			_fadeOutTimer = new FadingTimer(0f, time, time);
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

		private void ApplyFadeIn()
		{
			Color color = new Color(1.0f, 1.0f, 1.0f, _fadeInTimer.progress);
			SetMaterialColor(color);
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
				if (!excludeRendererFromEffects.Contains(rendererList[i]))
					_materials.Add(rendererList[i].material);
			}
		}
	}
}