// "Wave SDK 
// © 2023 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wave.OpenXR
{
	public class TrackerTextManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject trackerPanelTemplate = null;

		private Dictionary<InputDeviceTracker.TrackerId, GameObject> trackerTextList = new Dictionary<InputDeviceTracker.TrackerId, GameObject>();
		private List<InputDeviceTracker.TrackerId> trackerIdList = new List<InputDeviceTracker.TrackerId>();

		private const float kMaxCount = 8;
		/// <summary>
		///  Base on <see cref="kMaxCount"/> to divide 180 degrees into kMaxCount -1 of equal divisions
		/// </summary>
		private const float kDiffDeg = 180 / (kMaxCount - 1);
		private const float kRadius = 175;
		private bool showAll = true;

		private void Awake()
		{
			if (trackerPanelTemplate != null)
				trackerPanelTemplate.SetActive(false);
		}

		private void Start()
		{
			foreach (InputDeviceTracker.TrackerId id in Enum.GetValues(typeof(InputDeviceTracker.TrackerId)))
			{
				if (!trackerTextList.ContainsKey(id) && trackerPanelTemplate != null)
				{
					GameObject trackerPanel = Instantiate(trackerPanelTemplate, parent: transform);
					trackerPanel.SetActive(true);
					SetRandomColor(trackerPanel);
					TrackerText[] trackerTexts = trackerPanel.GetComponentsInChildren<TrackerText>();
					foreach (TrackerText trackerText in trackerTexts)
						trackerText.SetTrackerId(id);
					trackerTextList.Add(id, trackerPanel);
				}
				trackerIdList.Add(id);
			}
		}

		private void Update()
		{
			if (showAll)
			{
				ShowPanels();
			}
			else
			{
				ShowTrackedPanels();
			}
		}

		private void SetRandomColor(GameObject gameObject)
		{
			Image image = gameObject.GetComponent<Image>();
			if (image == null) { return; }

			float r = UnityEngine.Random.Range(50, 200) / 255.0f;
			float g = UnityEngine.Random.Range(50, 200) / 255.0f;
			float b = UnityEngine.Random.Range(50, 200) / 255.0f;
			image.color = new Color(r, g, b);
		}

		private void ShowPanels()
		{
			SetTrackerTextTranfom(trackerIdList);
		}

		private void ShowTrackedPanels()
		{
			List<InputDeviceTracker.TrackerId> trackerIds = new List<InputDeviceTracker.TrackerId>();
			foreach (InputDeviceTracker.TrackerId id in Enum.GetValues(typeof(InputDeviceTracker.TrackerId)))
			{
				if (InputDeviceTracker.IsTracked(id))
				{
					trackerIds.Add(id);
				}
				else
				{
					trackerTextList[id].SetActive(false);
				}
			}
			SetTrackerTextTranfom(trackerIds);
		}

		private void SetTrackerTextTranfom(List<InputDeviceTracker.TrackerId> trackerIds)
		{
			int index = 0;
			foreach (var id in trackerIds)
			{
				float sort = index % kMaxCount;
				float offsetDeg = trackerIds.Count > 1 ? (Mathf.Min(trackerIds.Count, kMaxCount) - 1) * kDiffDeg / 2 : 0;
				float objectDeg = offsetDeg - kDiffDeg * sort;
				float objectRad = objectDeg * Mathf.Deg2Rad;
				float height = index >= kMaxCount ? 50 : 0;
				trackerTextList[id].transform.position = new Vector3(-kRadius * Mathf.Sin(objectRad), height, kRadius * Mathf.Cos(objectRad));
				trackerTextList[id].transform.rotation = Quaternion.Euler(0, -objectDeg, 0);
				trackerTextList[id].SetActive(true);
				index++;
			}
		}

		public void SwitchPanel()
		{
			showAll = !showAll;
		}
	}
}
