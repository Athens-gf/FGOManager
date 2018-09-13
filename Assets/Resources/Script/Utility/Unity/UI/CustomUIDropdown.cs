using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	[RequireComponent(typeof(Dropdown))]
	public class CustomUIDropdown : CustomUI
	{
		[SerializeField]
		private bool m_IsScrollTrueDirection = true;
		[SerializeField]
		private Sprite m_SelectSprite = null;
		[SerializeField]
		private Color m_NotSelectColor = new Color(1, 1, 1, 0.25f), m_SelectColor = new Color(1, 1, 1, 0.75f);

		public StringEvent OnValueChanged = null;

		public Dropdown Dropdown { get { return GetComponent<Dropdown>(); } }
		public RectTransform ContentsParent { get { return transform.Find("Dropdown List")?.GetComponent<ScrollRect>()?.content; } }

		protected override void Awake()
		{
			base.Awake();
			Dropdown.onValueChanged.AddListener(i => OnValueChanged?.Invoke(Dropdown.options[i].text));
		}

		public List<RectTransform> Items
		{
			get
			{
				List<RectTransform> items = new List<RectTransform>();
				var childCount = ContentsParent.childCount;
				for (int i = 0; i < childCount; i++)
				{
					RectTransform item = ContentsParent.GetChild(i).gameObject.GetComponent<RectTransform>();
					if (item && item.gameObject.activeSelf)
						items.Add(item);
				}
				return items;
			}
		}

		private void Start()
		{
			var entry = new EventTrigger.Entry { eventID = EventTriggerType.Scroll };
			entry.callback.AddListener(e =>
			{
				Vector2? vec = (e as PointerEventData)?.scrollDelta;
				var flag = m_IsScrollTrueDirection ? vec.Value.y < 0 : vec.Value.y > 0;
				if (vec != null && vec.Value.y != 0 && (flag ? Dropdown.value < Dropdown.options.Count - 1 : Dropdown.value != 0))
					Dropdown.value += (flag ? 1 : -1);
			});
			Trigger.triggers.Add(entry);
		}

		private void Update()
		{
			if (!ContentsParent) return;
			RectTransform onItem = Items.FirstOrDefault(rt => rt.gameObject == EventSystem.current.currentSelectedGameObject);
			foreach (var item in Items)
			{
				Image image = item.Find("Item Background").GetComponent<Image>();
				if (image)
				{
					image.sprite = m_SelectSprite;
					image.color = item == onItem ? m_SelectColor : m_NotSelectColor;
				}
			}
			if (onItem)
			{
				if (((KeySet.Shift + KeyCode.Tab) | KeyCode.UpArrow).GetKeyDown())
				{
					var pos = ContentsParent.localPosition;
					var selectY = Items.First().localPosition.y - onItem.localPosition.y;
					if (pos.y > selectY)
					{
						pos.y = selectY;
						ContentsParent.localPosition = pos;
					}
				}
				else if ((KeySet.Tab | KeyCode.DownArrow).GetKeyDown())
				{
					var pos = ContentsParent.localPosition;
					var selectY = Items.First().localPosition.y - onItem.localPosition.y -
						ContentsParent.parent.parent.GetComponent<RectTransform>().sizeDelta.y + onItem.sizeDelta.y;
					if (pos.y < selectY)
					{
						pos.y = selectY;
						ContentsParent.localPosition = pos;
					}
				}
			}
		}
	}
}
