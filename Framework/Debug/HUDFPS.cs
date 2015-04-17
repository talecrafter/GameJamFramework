#if !UNITY_4_5

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CraftingLegends.Framework {

	public class HUDFPS : MonoBehaviour
	{

		// Attach this to a Text component to make a frames/second indicator.
		//
		// It calculates frames/second over each updateInterval,
		// so the display does not keep changing wildly.
		//
		// It is also fairly accurate at very low FPS counts (<10).
		// We do this not by simply counting frames per interval, but
		// by accumulating FPS for each frame. This way we end up with
		// correct overall FPS even if the interval renders something like
		// 5.5 frames.

		public float updateInterval = 0.5F;

		private float accum = 0; // FPS accumulated over the interval
		private int frames = 0; // Frames drawn over the interval
		private float timeleft;	// Left time for current interval

		private Text _textComponent;

		void Start()
		{
			_textComponent = GetComponent<Text>();
			if (!_textComponent)
			{
				Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
				enabled = false; // disable this component
				return;
			}
			timeleft = updateInterval;
		}

		void Update()
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			++frames;

			// Interval ended - update GUI text and start new interval
			if (timeleft <= 0.0)
			{
				// display two fractional digits (f2 format)
				float fps = accum / frames;
				string format = System.String.Format("{0:F2} FPS", fps);
				_textComponent.text = format;

				if (fps < 30)
					_textComponent.color = new Color(1.0f, 0.35f, 0);
				else
					if (fps < 10)
						_textComponent.color = Color.red;
					else
						_textComponent.color = Color.green;
				//	DebugConsole.Log(format,level);
				timeleft = updateInterval;
				accum = 0.0F;
				frames = 0;
			}
		}
	}
}

#endif