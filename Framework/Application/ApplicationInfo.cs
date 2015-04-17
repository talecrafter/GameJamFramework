using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CraftingLegends.Framework
{
    public class ApplicationInfo
    {
        public bool hasTwoMouseButtons = false;
        public bool useTwoMouseButtons = false;
        public bool hasTouch = false;
        public bool hasMouse = false;

        public bool usesGamepad
        {
            get
            {
#if !UNITY_WP8
                if (Input.GetJoystickNames().Length > 0)
                    return true;

				if (isOuyaSupportedHardware)
					return true;
#endif
                return false;
            }
        }

        private bool _isOuya = false;
        public bool isOuyaSupportedHardware
        {
            get { return _isOuya; }
            set
            {
                _isOuya = value;

                if (_isOuya)
                {
                    hasTouch = false;
                }
            }
        }

        public bool canPostOnSocialNetworks
        {
            get
            {
                return !isOuyaSupportedHardware && !Application.isWebPlayer;
            }
        }

        public ApplicationInfo()
        {
            SetPlatformSettings();

            useTwoMouseButtons = hasTwoMouseButtons;

            // load input setting
            if (PlayerPrefs.HasKey("INPUT_USETWOMOUSEBUTTONS"))
            {
                int useTwoMouseButtonsSetting = PlayerPrefs.GetInt("INPUT_USETWOMOUSEBUTTONS");
                useTwoMouseButtons = useTwoMouseButtonsSetting == 1;
            }

            // Debug: override settings
            if (Application.isEditor && BaseGameController.Instance.debugIsTouch)
            {
                hasTouch = true;
                hasTwoMouseButtons = false;
                useTwoMouseButtons = false;
            }

            // Debug: override settings
            if (Application.isEditor && BaseGameController.Instance.debugDisableAudio)
            {
                AudioListener.volume = 0;
            }
        }

        public void SetTwoMouseButtonMode(bool setting)
        {
            useTwoMouseButtons = setting;

            // save input setting to File
            if (setting)
                PlayerPrefs.SetInt("INPUT_USETWOMOUSEBUTTONS", 1);
            else
                PlayerPrefs.SetInt("INPUT_USETWOMOUSEBUTTONS", 0);
        }

        private void SetPlatformSettings()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    hasTwoMouseButtons = false;
                    hasTouch = true;
                    if (isOuyaSupportedHardware)
                        hasTouch = false;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    hasTwoMouseButtons = false;
                    hasTouch = true;
                    break;
                case RuntimePlatform.LinuxPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WSAPlayerARM:
                    hasTwoMouseButtons = false;
                    hasMouse = true;
                    break;
				case RuntimePlatform.WSAPlayerX64:
                    hasTwoMouseButtons = false;
                    hasMouse = true;
                    break;
				case RuntimePlatform.WSAPlayerX86:
                    hasTwoMouseButtons = false;
                    hasMouse = true;
                    break;
                case RuntimePlatform.OSXEditor:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.OSXPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.OSXWebPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.TizenPlayer:
                    hasTwoMouseButtons = false;
                    break;
                case RuntimePlatform.WP8Player:
                    hasTwoMouseButtons = false;
                    hasTouch = true;
                    break;
                case RuntimePlatform.WindowsEditor:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WindowsWebPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                default:
                    break;
            }
        }

    }

}