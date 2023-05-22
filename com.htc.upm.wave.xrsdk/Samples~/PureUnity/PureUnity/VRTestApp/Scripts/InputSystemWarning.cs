using UnityEngine;

namespace Wave.Generic.Sample
{
	[RequireComponent(typeof(Canvas))]
	public class InputSystemWarning : MonoBehaviour
	{
		bool inputSystemOnly = false;

		private void Awake()
		{
#if ENABLE_LEGACY_INPUT_MANAGER
		inputSystemOnly = false;
#elif ENABLE_INPUT_SYSTEM
			inputSystemOnly = true;
#endif
		}

		void Start()
		{
			GetComponent<Canvas>().enabled = inputSystemOnly;
		}
	}
}
