using UnityEngine;
using UnityEngine.UI;

namespace AthensUtility.Unity.UI
{
	[RequireComponent(typeof(Button))]
	public class CustomUIButton : CustomUI
	{
		public Button Button { get { return GetComponent<Button>(); } }
		public Text Text { get { return GetComponentInChildren<Text>(); } }

		private void Start()
		{
			Button.onClick.AddListener(() => IsSelect = false);
		}
	}
}
