using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	public class CustomUIManager : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		private List<CustomUI> m_UIList = null;

		public List<CustomUI> UIList { get { return m_UIList; } }

		private void Start()
		{
			foreach (var ui in UIList)
			{
				ui.IsSelect = false;
				ui.UIManager = this;
			}
			if (UIList.First())
				UIList.First().IsSelect = true;
		}

		private void Update()
		{
			if (UIList.Any(ui => ui.IsSelect))
				return;
			if (UIList.Last() && ((KeySet.Shift + KeyCode.Tab) | KeyCode.UpArrow | KeyCode.LeftArrow).GetKeyDown())
				UIList.Last().IsSelect = true;
			else if (UIList.First() && (KeySet.Tab | KeyCode.DownArrow | KeyCode.RightArrow).GetKeyDown())
				UIList.First().IsSelect = true;
		}

		public void SetSelect(CustomUI _UI)
		{
			foreach (var ui in UIList)
				if (ui != _UI)
					ui.IsSelect = false;

		}

		public void OnPointerClick(PointerEventData eventData) => SetSelect(null);
	}
}