using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	[RequireComponent(typeof(Shadow)), RequireComponent(typeof(Selectable))]
	public class CustomUI : MonoBehaviour
	{
		[Serializable]
		public class Navi_ic
		{
			public enum TabMove_e { UpDown = 0, LeftRight = 1, }

			/// <summary> Tabを使用して選択移動するかどうか </summary>
			public bool IsUseTab = true;

			public TabMove_e TabMove = TabMove_e.LeftRight;

			public CustomUI SelectOnUp = null;
			public CustomUI SelectOnDown = null;
			public CustomUI SelectOnLeft = null;
			public CustomUI SelectOnRight = null;

			public CustomUI Back { get { return (TabMove == TabMove_e.LeftRight ? SelectOnLeft : SelectOnUp); } }
			public CustomUI Next { get { return (TabMove == TabMove_e.LeftRight ? SelectOnRight : SelectOnDown); } }
		}

		private bool m_IsSelect;

		[SerializeField]
		private Shadow m_Shadow = null;

		[SerializeField]
		private Navi_ic m_Navi = new Navi_ic();

		public Navi_ic Navi { get { return m_Navi; } }

		public virtual bool IsSelectet { get { return true; } }

		/// <summary> トリガーイベントの参照 </summary>
		public EventTrigger Trigger { get { return GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>(); } }

		/// <summary> 選択中かどうか </summary>
		public bool IsSelect
		{
			get { return m_IsSelect; }
			set
			{
				OnSelected(value);
				if (value && IsSelectet)
					EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}

		/// <summary> UIManager </summary>
		public CustomUIManager UIManager { get; set; }

		/// <summary> UI要素 </summary>
		public virtual Selectable UIObject { get { return GetComponent<Selectable>(); } }

		public RectTransform Rect { get { return GetComponent<RectTransform>(); } }

		protected virtual void Reset()
		{
			m_Shadow = GetComponent<Shadow>();
			m_Shadow.effectDistance = new Vector2(5, -5);
			IsSelect = false;
		}

		protected virtual void Awake()
		{
			var entry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
			entry.callback.AddListener((_) => OnSelected(true));
			Trigger.triggers.Add(entry);
			entry = new EventTrigger.Entry { eventID = EventTriggerType.UpdateSelected };
			entry.callback.AddListener((_) => OnUpdateSelected());
			Trigger.triggers.Add(entry);

			var navi = UIObject.navigation;
			navi.mode = Navigation.Mode.Explicit;
			navi.selectOnUp = navi.selectOnDown = navi.selectOnLeft = navi.selectOnRight = null;
			UIObject.navigation = navi;
		}

		/// <summary> 選択された、または選択が解除されたとき </summary>
		public virtual void OnSelected(bool _isSelect)
		{
			if (!IsSelect && _isSelect)
				UIManager?.SetSelect(this);
			m_IsSelect = _isSelect;
			m_Shadow.enabled = _isSelect;
		}

		/// <summary> 選択されている間呼び出される </summary>
		protected virtual void OnUpdateSelected()
		{
			if (!UIObject) return;
			if (Input.GetKeyDown(KeyCode.UpArrow) && Navi.SelectOnUp)
				Navi.SelectOnUp.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.DownArrow) && Navi.SelectOnDown)
				Navi.SelectOnDown.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.LeftArrow) && Navi.SelectOnLeft)
				Navi.SelectOnLeft.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.RightArrow) && Navi.SelectOnRight)
				Navi.SelectOnRight.IsSelect = true;
			if (!Navi.IsUseTab) return;
			if ((KeySet.Shift + KeyCode.Tab).GetKeyDown() && Navi.Back)
				Navi.Back.IsSelect = true;
			else if (Input.GetKeyDown(KeyCode.Tab) && Navi.Next)
				Navi.Next.IsSelect = true;
		}
	}
}