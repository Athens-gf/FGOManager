using System;
using UnityEngine.UI;
using KMUtility.Unity.UI;

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
				Text.text = $"{value.No} : {value.Name}";
				gameObject.name = $"ServantNode {Text.text}";
			}
		}

		private void Start()
		{
			Button.onClick.AddListener(() => OnClick());
		}
	}
}
