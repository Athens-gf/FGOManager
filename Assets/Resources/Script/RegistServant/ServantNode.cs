using System;
using UnityEngine.UI;
using AthensUtility.Unity.UI;

namespace FGOManager.Register
{
	public class ServantNode : CustomUIButton
	{
		private ServantBase m_Servant;

		public Action OnClick { get; set; } = null;

		public ServantBase Servant
		{
			get { return m_Servant; }
			set
			{
				m_Servant = value;
				gameObject.name = $"ServantNode {Text.text}";
				SetName();
			}
		}

		private void Start() => Button.onClick.AddListener(() => OnClick());

		public void SetName() => Text.text = $"{m_Servant.No} : {m_Servant.Name}";
	}
}
