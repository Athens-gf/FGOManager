using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	public class CustomUIManager : MonoBehaviour
	{
		[SerializeField]
		private CustomUI m_FirstSelectUI = null;
		[SerializeField]
		private List<CustomUI> m_UIList = null;

		public List<CustomUI> UIList { get { return m_UIList; } }

		private void Start()
		{
			foreach (var ui in UIList)
				ui.IsSelect = false;
			if (m_FirstSelectUI != null)
				m_FirstSelectUI.IsSelect = true;
		}

		public void SetSelect(CustomUI _UI)
		{
			foreach (var ui in UIList)
				if (ui != _UI)
					ui.IsSelect = false;

		}
	}
}