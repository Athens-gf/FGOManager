using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	[RequireComponent(typeof(Dropdown))]
	public class CustomDropdown : CustomUI
	{
		public Dropdown Dropdown { get { return GetComponent<Dropdown>(); } }
		public RectTransform ContentsParent { get { return transform.Find("Dropdown List")?.GetComponent<ScrollRect>()?.content; } }

		public List<Toggle> Items
		{
			get
			{
				List<Toggle> items = new List<Toggle>();
				var childCount = ContentsParent.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Toggle item = ContentsParent.GetChild(i).gameObject.GetComponent<Toggle>();
					if (item && item.gameObject.activeSelf)
						items.Add(item);
				}
				return items;
			}
		}

		private void Update()
		{
			if (!ContentsParent) return;
			Toggle onItem = Items.FirstOrDefault(i => i.isOn);
			Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		}
	}
}
