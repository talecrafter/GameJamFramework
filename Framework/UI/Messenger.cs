using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CraftingLegends.Framework;

namespace CraftingLegends.Framework
{
	public class Messenger : MonoBehaviour
	{
		public float displayDuration = 2f;

		private FadingTimer _timer;

		private Text[] _text = null;

		// ================================================================================
		//  unity methods
		// --------------------------------------------------------------------------------

		public void Awake()
		{
			_timer = new FadingTimer(0.4f, displayDuration, 0.4f);
			_timer.Stop();

			if (_text == null)
			{
				// the MessengerText Object will be inside the MainCanvas
				var messengerObject = GameObject.Find("MessengerText") as GameObject;
				if (messengerObject != null)
					_text = messengerObject.GetComponentsInChildren<Text>();
			}

			if (_text != null)
			{
				for (int i = 0; i < _text.Length; i++)
				{
					_text[i].enabled = false;
				}
			}
		}

		public void Update()
		{
			if (!_timer.hasEnded)
			{
				_timer.Update();
				UpdateColor();
			}

			if (_timer.hasEnded)
			{
				for (int i = 0; i < _text.Length; i++)
				{
					_text[i].enabled = false;
				}
			}
		}

		public void OnLevelWasLoaded(int levelIndex)
		{
			_timer.Stop();
			UpdateColor();
		}

		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public void Message(string newMessage)
		{
			for (int i = 0; i < _text.Length; i++)
			{
				_text[i].text = newMessage;
				_text[i].enabled = true;
			}

			_timer.Reset();

			UpdateColor();
        }

		// ================================================================================
		//  private methods
		// --------------------------------------------------------------------------------

		private void UpdateColor()
		{
			float alpha = _timer.progress;
			if (_timer.hasEnded)
				alpha = 0;

			Color displayColor = new Color(1.0f, 1.0f, 1.0f, _timer.progress);
			if (_timer.hasEnded)
				displayColor.a = 0;

			for (int i = 0; i < _text.Length; i++)
			{
				Color color = _text[i].color;
				color.a = alpha;
				_text[i].color = color;
            }
		}
	}
}