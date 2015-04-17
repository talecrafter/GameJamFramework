using UnityEngine;
using System.Collections;
using CraftingLegends.Core;
using System.Collections.Generic;
using System;

namespace CraftingLegends.Framework
{
    public class BaseGameController : MonoBehaviour
    {
        // ================================================================================
        //  state
        // --------------------------------------------------------------------------------

        public delegate void GameStateChangeHandler(GameState newState);
        public event GameStateChangeHandler gameStateChanged;

        protected Stack<GameState> _stateMachine = new Stack<GameState>(new[] { GameState.Running });
        public GameState state
        {
            get
            {
                return _stateMachine.Peek();
            }
            protected set
            {
                // if paused then push on top
                if (value == GameState.Paused)
                {
                    _stateMachine.Push(GameState.Paused);
                }
                else
                {
                    _stateMachine.Clear();
                    _stateMachine.Push(value);
                }

                if (gameStateChanged != null)
                {
                    gameStateChanged(value);
                }
            }
        }

		public static bool isMenuLevel
		{
			get
			{
				return Application.loadedLevelName.ToLower().Contains("menu");
            }
		}
		public static bool isRunning { get { return Instance.state == GameState.Running; } }
		public static bool isPaused { get { return Instance.state == GameState.Paused; } }

        // ================================================================================
        //  singleton
        // --------------------------------------------------------------------------------

        public static BaseGameController Instance;

		// ================================================================================
		//  components
		// --------------------------------------------------------------------------------

		[HideInInspector]
		public BaseNavigationInput baseNavigationInput;

		[HideInInspector]
        public ApplicationInfo applicationInfo;
        [HideInInspector]
        public BaseAudioManager baseAudioManager;
        [HideInInspector]
        public Rect levelBounds;
        [HideInInspector]
        public LevelGrid levelGrid = null;
		[HideInInspector]
		public ScreenShake screenShake;

		public IInputController inputController = null;
		public IGamepadInput gamepadInput = null;

        public GameStateData gameStateData = new GameStateData();

        // ================================================================================
        //  debug
        // --------------------------------------------------------------------------------

        public GameObject debugInterfacePrefab;
        public bool debugIsTouch;
        public bool debugDisableAudio;

        // ================================================================================
        //  private
        // --------------------------------------------------------------------------------

        private Timer _loadingTimer = null;

        // flag thas is set when the Application looses Focus and switches automatically to pause mode
        private bool _automaticallyPaused = false;

		GameState? _stateBeforeLoading = null;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		protected virtual void Start()
        {
            LoadLevelData();

            InitLevel();
        }

        protected virtual void Update()
        {
            if (state == GameState.Loading)
            {
                if (_loadingTimer != null)
                {
                    _loadingTimer.Update();
                }

                if (!Application.isLoadingLevel)
                {
                    if (_loadingTimer != null)
                    {
                        if (_loadingTimer.hasEnded)
                        {
                            _loadingTimer = null;
                        }
                    }
                    else
                    {
						InitLevel();
                    }
                }
            }
		}

        protected virtual void OnLevelWasLoaded(int level)
        {
            LoadLevelData();

            if (_loadingTimer == null)
            {
                InitLevel();
            }

			// memory garbage collection (only Mono)
			System.GC.Collect();
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus &&
                (state == GameState.Running || state == GameState.Sequence))
            {
                Pause();
                _automaticallyPaused = true;
            }

            if (!pauseStatus && state == GameState.Paused && _automaticallyPaused)
                Resume();
        }

        // ================================================================================
        //  public methods
        // --------------------------------------------------------------------------------

        public void Resume()
        {
            _automaticallyPaused = false;
            Time.timeScale = 1.0f;

            // pop paused state from state machine
            if (state == GameState.Paused)
            {
                _stateMachine.Pop();
                if (gameStateChanged != null)
                {
                    gameStateChanged(state);
                }
            }
        }

        public virtual void Pause()
        {
            Time.timeScale = 0;
            state = GameState.Paused;
        }

        public void SetLoadingTimer()
        {
            _loadingTimer = new Timer(0.1f);
        }

        public void StartSequence()
        {
            state = GameState.Sequence;
        }

        public void EndSequence()
        {
			if (state != GameState.Loading)
				state = GameState.Running;

			// check if sequence ended right after loading new level
			if (_stateBeforeLoading.HasValue && _stateBeforeLoading.Value == GameState.Sequence)
				_stateBeforeLoading = null;
        }

        public void Quit()
        {
			Application.Quit();
        }

		public virtual object GetPrefab(Type type)
		{
			return null;
		}

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		protected bool Init()
        {
            if (Instance != null)
            {
                return false;
            }

            Instance = this;

            baseAudioManager = GetComponent<BaseAudioManager>();
			baseNavigationInput = GetComponent<BaseNavigationInput>();
			applicationInfo = new ApplicationInfo();

			gamepadInput = transform.GetInterface<IGamepadInput>();
			inputController = transform.GetInterface<IInputController>();

			// object shall persist through all levels
			DontDestroyOnLoad(gameObject);

            return true;
        }

		// use this for finding references in the scene
        protected virtual void LoadLevelData()
        {
			screenShake = FindObjectOfType<ScreenShake>();
        }

		// use this for initialising gameplay
        protected virtual void InitLevel()
        {
            if (isMenuLevel)
            {
                state = GameState.Menu;
            }
            else
            {
				if (_stateBeforeLoading != null)
				{
					state = _stateBeforeLoading.Value;
					_stateBeforeLoading = null;
				}
				else
				{
					state = GameState.Running;
				}
            }
        }

        protected IEnumerator LoadLevel(string levelName)
        {
			if (state == GameState.Sequence)
				_stateBeforeLoading = state;

            state = GameState.Loading;
            SetLoadingTimer();
			yield return null;
			Application.LoadLevel(levelName);
        }

        protected IEnumerator LoadLevel(int levelIndex)
        {
            state = GameState.Loading;
            SetLoadingTimer();
            yield return null;
            Application.LoadLevel(levelIndex);
        }
    }

}