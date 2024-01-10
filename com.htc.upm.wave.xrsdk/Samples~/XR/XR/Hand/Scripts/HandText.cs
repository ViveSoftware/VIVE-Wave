using UnityEngine;
using UnityEngine.UI;
using Wave.OpenXR;

namespace Wave.XR.Sample.Hand 
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class HandText : MonoBehaviour
	{
		[SerializeField]
		private bool isLeft = false;
		private Text m_Text = null;

		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		private Vector3 origin = Vector3.zero, direction = Vector3.zero;
		private float pinchStrength = 0, graspStrength = 0;
		private bool isPinching = false, isGrasping = false;
		void Update()
		{
			if (m_Text == null) { return; }

			InputDeviceHand.GetPinchStrength(isLeft, out pinchStrength);
			isPinching = InputDeviceHand.IsHandPinching(isLeft);
			InputDeviceHand.GetPinchOrigin(isLeft, out origin);
			InputDeviceHand.GetPinchDirection(isLeft, out direction);
			InputDeviceHand.GetGraspStrength(isLeft, out graspStrength);
			isGrasping = InputDeviceHand.IsHandGrasping(isLeft);

			m_Text.text = (isLeft ? "Left Hand: " : "Right Hand: ");
			m_Text.text += $"\nPinch Strength : {pinchStrength}";
			m_Text.text += $"\nPinching : {isPinching}";
			m_Text.text += $"\nPinch Origin {origin.ToString("F3")}";
			m_Text.text += $"\nPinch Direction {direction.ToString("F3")}";
			m_Text.text += $"\nGrasp Strength : {graspStrength}";
			m_Text.text += $"\nGrasping : {isGrasping}";
		}

	}
}
