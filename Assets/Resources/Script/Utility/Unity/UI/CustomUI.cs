using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	[RequireComponent(typeof(Shadow)), RequireComponent(typeof(Selectable))]
	public class CustomUI : MonoBehaviour
	{
		public enum TabMove_e { UpDown = 0, LeftRight = 1, }

		private bool m_IsSelect;

		[SerializeField]
		private Shadow m_Shadow = null;
		[SerializeField]
		private TabMove_e m_TabMove = TabMove_e.LeftRight;

		/// <summary> Tabを使用して選択移動するかどうか </summary>
		public bool IsUseTab = true;

		/// <summary> トリガーイベントの参照 </summary>
		public EventTrigger Trigger { get; protected set; }

		/// <summary> 選択中かどうか </summary>
		public bool IsSelect { get { return m_IsSelect; } set { OnSelected(value); } }

		/// <summary> Tabでの移動方向 </summary>
		public TabMove_e TabMove { get { return m_TabMove; } set { m_TabMove = value; } }

		/// <summary> UIManager </summary>
		public CustomUIManager UIManager { get; set; } = null;

		/// <summary> UI要素 </summary>
		public Selectable UIObject { get; private set; } = null;

		protected virtual void Reset()
		{
			m_Shadow = GetComponent<Shadow>();
			m_Shadow.effectDistance = new Vector2(5, -5);
			IsSelect = false;
		}

		protected virtual void Start()
		{
			UIObject = GetComponent<Selectable>();

			Trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
			entry.callback.AddListener((_) => OnSelected(true));
			Trigger.triggers.Add(entry);
			entry = new EventTrigger.Entry { eventID = EventTriggerType.UpdateSelected };
			entry.callback.AddListener((_) => OnUpdateSelected());
			Trigger.triggers.Add(entry);
		}

		/// <summary> 選択された、または選択が解除されたとき </summary>
		protected virtual void OnSelected(bool _isSelect)
		{
			m_IsSelect = _isSelect;
			m_Shadow.enabled = _isSelect;
			if (_isSelect)
				UIManager?.SetSelect(this);
		}

		/// <summary> 選択されている間呼び出される </summary>
		protected virtual void OnUpdateSelected()
		{
			if (!IsUseTab || !UIObject) return;
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				Selectable sel = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
					? (TabMove == TabMove_e.LeftRight ? UIObject.navigation.selectOnLeft : UIObject.navigation.selectOnUp)
					: (TabMove == TabMove_e.LeftRight ? UIObject.navigation.selectOnRight : UIObject.navigation.selectOnDown);
				if (sel)
					EventSystem.current.SetSelectedGameObject(sel.gameObject);
			}
		}
	}
}