using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AthensUtility.Unity.UI
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

			public CustomUI SelectOnLeft = null;
			public CustomUI SelectOnRight = null;
			public CustomUI SelectOnUp = null;
			public CustomUI SelectOnDown = null;

			public CustomUI Back { get { return (TabMove == TabMove_e.LeftRight ? SelectOnLeft : SelectOnUp); } }
			public CustomUI Next { get { return (TabMove == TabMove_e.LeftRight ? SelectOnRight : SelectOnDown); } }
		}

		private bool m_IsSelect;

		[SerializeField]
		private Shadow m_Shadow = null;

		[SerializeField]
		private Navi_ic m_Navigation = new Navi_ic();

		public Navi_ic Navigation { get { return m_Navigation; } }

		public virtual bool IsSelectet { get { return true; } }

		public UnityEvent OnSelect = null;

		/// <summary> トリガーイベントの参照 </summary>
		public EventTrigger Trigger { get { return GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>(); } }

		/// <summary> 選択中かどうか </summary>
		public virtual bool IsSelect
		{
			get { return m_IsSelect; }
			set
			{
				OnSelected(value);
				if (value && IsSelectet && !EventSystem.current.alreadySelecting)
					EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}
		//		EventSystem.current.SetSelectedGameObject(gameObject);

		/// <summary> UIManager </summary>
		public CustomUIManager UIManager => CustomUIManager.Instance;

		/// <summary> UI要素 </summary>
		public virtual Selectable UIObject { get { return GetComponent<Selectable>(); } }

		public RectTransform Rect { get { return GetComponent<RectTransform>(); } }

		protected virtual void Reset()
		{
			m_Shadow = GetComponent<Shadow>();
			m_Shadow.effectDistance = new Vector2(3, -3);
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
			navi.mode = UnityEngine.UI.Navigation.Mode.Explicit;
			navi.selectOnUp = navi.selectOnDown = navi.selectOnLeft = navi.selectOnRight = null;
			UIObject.navigation = navi;

			UIManager.UIList.Add(this);
			if (m_Shadow) m_Shadow.enabled = false;
		}

		protected void OnDestroy() => UIManager.UIList.Remove(this);

		/// <summary> 選択された、または選択が解除されたとき </summary>
		protected virtual void OnSelected(bool _isSelect)
		{
			if (!IsSelect && _isSelect)
				UIManager.SetSelect(this);
			m_IsSelect = _isSelect;
			if (m_Shadow) m_Shadow.enabled = _isSelect;
			if (!IsSelect && _isSelect) OnSelect?.Invoke();
		}

		/// <summary> 選択されている間呼び出される </summary>
		protected virtual void OnUpdateSelected()
		{
			if (!UIObject) return;
			if (Input.GetKeyDown(KeyCode.UpArrow) && Navigation.SelectOnUp)
				Navigation.SelectOnUp.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.DownArrow) && Navigation.SelectOnDown)
				Navigation.SelectOnDown.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.LeftArrow) && Navigation.SelectOnLeft)
				Navigation.SelectOnLeft.IsSelect = true;
			if (Input.GetKeyDown(KeyCode.RightArrow) && Navigation.SelectOnRight)
				Navigation.SelectOnRight.IsSelect = true;
			if (!Navigation.IsUseTab) return;
			if ((KeySet.Shift + KeyCode.Tab).GetKeyDown() && Navigation.Back)
				Navigation.Back.IsSelect = true;
			else if (Input.GetKeyDown(KeyCode.Tab) && Navigation.Next)
				Navigation.Next.IsSelect = true;
		}
	}
}