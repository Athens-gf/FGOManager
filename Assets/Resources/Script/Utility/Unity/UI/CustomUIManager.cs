using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AthensUtility.Unity.UI
{
	public class CustomUIManager : SingletonMonoBehaviour<CustomUIManager>
	{
		public List<CustomUI> UIList { get; private set; } = new List<CustomUI>();

		private void Update()
		{
			if (UIList.Any(ui => ui.IsSelect))
			{
				if (EventSystem.current.currentSelectedGameObject)
					return;
				else
					SetSelect(null);
			}

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
	}
}