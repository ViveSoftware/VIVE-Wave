using UnityEngine;
using UnityEngine.UI;
using Wave.Essence.Hand;
using Wave.Essence.Raycast;

namespace Wave.Essence.Samples.Raycast
{
	[RequireComponent(typeof(Image))]
	public class HandPinchArea : MonoBehaviour
	{
		[SerializeField]
		private HandRaycastPointer leftHandRay;
		[SerializeField]
		private HandRaycastPointer rightHandRay;

		private Image image;
		private RectTransform rectTransform;
		private float m_Left = 0;
		private float m_Bottom = 0;
		private float m_Right = 0;
		private float m_Top = 0;
		private bool isShowArea = false;

		private void Awake()
		{
			image = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();

			if (leftHandRay != null || rightHandRay != null)
			{
				m_Left = leftHandRay.LeftInteractive < rightHandRay.LeftInteractive ? leftHandRay.LeftInteractive : rightHandRay.LeftInteractive;
				m_Bottom = leftHandRay.BottomInteractive < rightHandRay.BottomInteractive ? leftHandRay.BottomInteractive : rightHandRay.BottomInteractive;
				m_Right = leftHandRay.RightInteractive > rightHandRay.RightInteractive ? leftHandRay.RightInteractive : rightHandRay.RightInteractive;
				m_Top = leftHandRay.TopInteractive > rightHandRay.TopInteractive ? leftHandRay.TopInteractive : rightHandRay.TopInteractive;
			}
		}

		private void Update()
		{
			bool isValid = (HandManager.Instance.IsHandPoseValid(leftHandRay.Hand) || HandManager.Instance.IsHandPoseValid(rightHandRay.Hand)) & isShowArea;
			image.enabled = isValid;
			if (!isValid)
			{
				return;
			}

			rectTransform.anchorMin = new Vector2(m_Left, m_Bottom);
			rectTransform.anchorMax = new Vector2(m_Right, m_Top);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
		}

		public void OnToggleShowArea(bool enable)
		{
			isShowArea = enable;
		}
		public void OnLeftValueChange(float vlaue)
		{
			leftHandRay.LeftInteractive = rightHandRay.LeftInteractive = m_Left = vlaue;
		}
		public void OnBottomValueChange(float vlaue)
		{
			leftHandRay.BottomInteractive = rightHandRay.BottomInteractive = m_Bottom = vlaue;
		}
		public void OnRightValueChange(float vlaue)
		{
			leftHandRay.RightInteractive = rightHandRay.RightInteractive = m_Right = vlaue;
		}
		public void OnTopValueChange(float vlaue)
		{
			leftHandRay.TopInteractive = rightHandRay.TopInteractive = m_Top = vlaue;
		}
		public void OnLeftValueChange(Text text)
		{
			text.text = m_Left.ToString("F2");
		}
		public void OnBottomValueChange(Text text)
		{
			text.text = m_Bottom.ToString("F2");
		}
		public void OnRightValueChange(Text text)
		{
			text.text = m_Right.ToString("F2");
		}
		public void OnTopValueChange(Text text)
		{
			text.text = m_Top.ToString("F2");
		}
	}
}
