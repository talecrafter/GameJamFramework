using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CraftingLegends.Core;
using CraftingLegends.Framework;
using System.Collections.Generic;
using System;

namespace CraftingLegends.Framework
{
	public class BaseUIManager : MonoBehaviour {

		public static BaseUIManager Instance = null;

		public static bool useBigFont = false;

		public static bool pointerIsOverUI
		{
			get
			{
				if (EventSystem.current.IsPointerOverGameObject())
					return true;

				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					if (EventSystem.current.IsPointerOverGameObject(touches[i].fingerId))
					{
						return true;
					}
				}

				return false;
			}
		}

		public SceneFader sceneFader;

		public GameObject imageDisplay;
		private Animator _imageDisplayAnimator;

		protected InputScheme _inputScheme = InputScheme.None;

		protected List<IUiState<GameState>> _states = new List<IUiState<GameState>>();
		protected UIStateMachine<GameState> _stateMachine = new UIStateMachine<GameState>();

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		public void Start()
		{
			for (int i = 0; i < _states.Count; i++)
			{
				_states[i].SetInactive();
			}

			if (MainBase.Instance != null)
			{
				SetGameState(MainBase.Instance.state);
				MainBase.Instance.gameStateChanged += GameStateChanged;
			}
		}

		public void Update()
		{
			if (sceneFader != null)
				sceneFader.Update();
		}

		public void OnGUI()
		{
			if (sceneFader != null)
				sceneFader.OnGUI();
		}

		public void OnDestroy()
		{
			if (Instance != null && Instance == this)
				Instance = null;
		}

		// ================================================================================
		//  base methods
		// --------------------------------------------------------------------------------

		protected bool Init()
		{
			if (Instance != null && Instance != this)
			{
				return false;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);

			sceneFader = new SceneFader();

			InitStates();

			// get references to image display, which is used in sequences
			if (imageDisplay != null)
			{
				_imageDisplayAnimator = imageDisplay.GetComponentInChildren<Animator>();
				HideImage();
			}

			return true;
		}

		private void InitStates()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				(child as RectTransform).anchoredPosition = Vector2.zero;
				child.gameObject.SetActive(true);

				var stateList = child.GetInterfacesInChildren<IUiState<GameState>>();
				if (stateList.Length > 0)
				{
					foreach (var item in stateList)
					{
						_states.Add(item);
					}
				}
			}
		}

		// ================================================================================
		//  states
		// --------------------------------------------------------------------------------

		public void Push(MonoBehaviour behaviour)
		{
			if (behaviour is IUiState<GameState>)
			{
				_stateMachine.Push(behaviour as IUiState<GameState>);
			}
		}

		public void Push(Type type)
		{
			foreach (var item in _states)
			{
				if (item.GetType() == type)
				{
					_stateMachine.Push(item);
					break;
				}
			}
		}

		private void GameStateChanged(GameState newState)
		{
			SetGameState(newState);
		}

		private void SetGameState(GameState state)
		{
			if (state == GameState.Loading)
				sceneFader.Clear();

			IUiState<GameState> uiState = GetUIState(state);

			if (uiState == null)
				_stateMachine.PopAll();
			else
			{
				_stateMachine.SwitchTo(uiState);
			}
		}

		private IUiState<GameState> GetUIState(GameState state)
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (_states[i].gameState == state)
					return _states[i];
			}
			return null;
		}

		public void Pop()
		{
			_stateMachine.Pop();
		}

		// ================================================================================
		//  game related methods
		// --------------------------------------------------------------------------------

		public virtual void Pause()
		{
			MainBase.Instance.Pause();
		}

		public virtual void Resume()
		{
			MainBase.Instance.Resume();
		}

		public void Quit()
		{
			MainBase.Instance.Quit();
		}

		public void ShowImage(Sprite image)
		{
			// trigger fadeIn animation
			if (imageDisplay != null)
				imageDisplay.GetComponentInChildren<Image>().sprite = image;
			if (_imageDisplayAnimator != null)
				_imageDisplayAnimator.SetBool("show", true);
		}

		public void HideImage()
		{
			// trigger fadeOut animation
			if (_imageDisplayAnimator != null)
				_imageDisplayAnimator.SetBool("show", false);
		}

		public virtual void SetInputScheme(InputScheme inputScheme)
		{
			_inputScheme = inputScheme;
		}
	}
}
