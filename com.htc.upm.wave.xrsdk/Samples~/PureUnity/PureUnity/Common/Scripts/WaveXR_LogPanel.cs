using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Wave.XR.Sample
{
	public class WaveXR_LogPanel : MonoBehaviour
	{
		public int maxLines = 20;
		[SerializeField] private Text logText;
		[SerializeField] private ScrollRect logScrollRect;

		private StringBuilder sb;
		private List<string> logList;

		[SerializeField] bool newLogAtTop = false;
		[SerializeField] Color lineColor1 = new Color(0.263f, 0.129361f, 0.1370191f);
		[SerializeField] Color lineColor2 = new Color(0.07062121f, 0.0831159f, 0.2169811f);
		string color1Hex;
		string color2Hex;

		readonly object lockObj = new object();

		int serialNumber = 0;

		private void Awake()
		{
			if (logText == null || logScrollRect == null) return;
			logList = new List<string>(512);
			sb = new StringBuilder();
			color1Hex = ColorUtility.ToHtmlStringRGB(lineColor1);
			color2Hex = ColorUtility.ToHtmlStringRGB(lineColor2);
			logText.alignment = newLogAtTop ? TextAnchor.UpperLeft : TextAnchor.LowerLeft;
		}

		// This is thread safe
		public void AddLog(string message)
		{
			lock (lockObj)
			{
				if (logText == null || logScrollRect == null) return;

				var msg = string.Format("{0:D2} {1}", serialNumber, message);
				serialNumber++;
				if (serialNumber > 99)
					serialNumber = 0;

				if (newLogAtTop)
				{
					logList.Insert(0, msg);
					// Remove log out of maxLines
					if (logList.Count > maxLines)
						logList.RemoveAt(logList.Count - 1);
				}
				else
				{
					// new log at bottom
					logList.Add(msg);
					// Remove log out of maxLines
					if (logList.Count > maxLines)
						logList.RemoveAt(0);
				}
				doUpdate = true;
			}
		}

		// Use update to avoid too many log in one frame.
		bool doUpdate;
		private void Update()
		{
			lock (lockObj)
			{
				if (doUpdate)
				{
					if (newLogAtTop)
						ScrollToTopAfterAdd();
					else
						ScrollToBottomAfterAdd();
					UpdateLogText();
					doUpdate = false;
				}
			}
		}

		public void Clear()
		{
			lock (lockObj)
			{
				logList.Clear();
				doUpdate = true;
			}
		}

		private void UpdateLogText()
		{
			sb.Clear();
			bool useColor1 = true;
			foreach (string log in logList)
			{
				var color = useColor1 ? color1Hex : color2Hex;
				useColor1 = !useColor1;
				sb.Append("<color=#").Append(color).Append(">").Append(log).Append("</color>").AppendLine();
			}
			logText.text = sb.ToString();
		}

		public void ScrollToTopAfterAdd()
		{
			StartCoroutine(ScrollToTopCoroutine());
		}

		public void ScrollToBottomAfterAdd()
		{
			StartCoroutine(ScrollToBottomCoroutine());
		}

		private IEnumerator ScrollToTopCoroutine()
		{
			yield return null;
			logScrollRect.verticalNormalizedPosition = 1f;
		}

		private IEnumerator ScrollToBottomCoroutine()
		{
			yield return null;
			logScrollRect.verticalNormalizedPosition = 0f;
		}
	}
}
