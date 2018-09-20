using System;
using UnityEngine;
using UnityEngine.UI;
using AthensUtility.Unity.UI;

namespace FGOManager.Register
{
	public class CharacteristicNode : CustomUIToggle
	{
		[SerializeField]
		private Text m_Text = null;

		public string Text { get { return m_Text?.text ?? ""; } set { if (m_Text) m_Text.text = value; } }

	}
}
