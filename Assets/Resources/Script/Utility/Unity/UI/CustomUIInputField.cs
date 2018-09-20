using System;
using UnityEngine;
using UnityEngine.UI;

namespace AthensUtility.Unity.UI
{
	[RequireComponent(typeof(InputField))]
	public class CustomUIInputField : CustomUI
	{
		public InputField InputField { get { return GetComponent<InputField>(); } }
		public string Text { get { return InputField.textComponent.text; } set { InputField.text = value; InputField.onEndEdit?.Invoke(value); } }
		public InputField.SubmitEvent OnEndEdit { get { return InputField.onEndEdit; } }

		protected virtual void Start() => OnEndEdit.AddListener(s => IsSelect = false);

		protected override void OnUpdateSelected()
		{
			if (!UIObject) return;
			if (!Navigation.IsUseTab) return;
			if ((KeySet.Shift + KeyCode.Tab).GetKeyDown() && Navigation.Back)
				Navigation.Back.IsSelect = true;
			else if (Input.GetKeyDown(KeyCode.Tab) && Navigation.Next)
				Navigation.Next.IsSelect = true;
			if (InputField.isFocused)
				return;
			if (Input.GetKeyDown(KeyCode.UpArrow) && Navigation.SelectOnUp)
				Navigation.SelectOnUp.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.DownArrow) && Navigation.SelectOnDown)
				Navigation.SelectOnDown.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.LeftArrow) && Navigation.SelectOnLeft)
				Navigation.SelectOnLeft.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.RightArrow) && Navigation.SelectOnRight)
				Navigation.SelectOnRight.IsSelect = true;
		}
	}
}
