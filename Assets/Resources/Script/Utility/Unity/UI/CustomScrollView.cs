using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KMUtility.Unity.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class CustomScrollView : CustomUI
	{
		public List<CustomUI> Contents { get; private set; } = new List<CustomUI>();
		public RectTransform ContentsParent { get { return GetComponent<ScrollRect>().content; } }
		public CustomUI SelectUI { get { return Contents.FirstOrDefault(ui => ui.IsSelect); } }

		/// <summary> UI要素 </summary>
		public override Selectable UIObject { get { return GetComponent<Selectable>() ?? gameObject.AddComponent<Selectable>(); } }

		public override bool IsSelectet { get { return false; } }

		private void Start() { }

		public override void OnSelected(bool _isSelect)
		{
			if (_isSelect && Contents.Any())
				((KeySet.Shift + KeyCode.Tab).GetKeyDown() ? Contents.Last() : Contents.First()).IsSelect = true;
		}

		private void Update()
		{
			if (Contents.All(ui => !ui.IsSelect))
				return;
			if (((KeySet.Shift + KeyCode.Tab) | KeyCode.UpArrow).GetKeyDown())
			{
				var pos = ContentsParent.localPosition;
				var selectY = Contents.First().Rect.localPosition.y - SelectUI.Rect.localPosition.y;
				if (pos.y > selectY)
				{
					pos.y = selectY;
					ContentsParent.localPosition = pos;
				}
			}
			else if ((KeySet.Tab | KeyCode.DownArrow).GetKeyDown())
			{
				var pos = ContentsParent.localPosition;
				var selectY = Contents.First().Rect.localPosition.y - SelectUI.Rect.localPosition.y - Rect.sizeDelta.y + SelectUI.Rect.sizeDelta.y;
				if (pos.y < selectY)
				{
					pos.y = selectY;
					ContentsParent.localPosition = pos;
				}
			}
		}

		public void AddContent(CustomUI _ui)
		{
			var navi = _ui.UIObject.navigation;
			navi.mode = Navigation.Mode.Explicit;
			navi.selectOnUp = navi.selectOnDown = navi.selectOnLeft = navi.selectOnRight = null;
			_ui.UIObject.navigation = navi;
			if (Contents.Any())
			{
				Contents.Last().Navi.SelectOnDown = _ui;
				_ui.Navi.SelectOnUp = Contents.Last();
			}
			else
				_ui.Navi.SelectOnUp = Navi.SelectOnUp;
			_ui.Navi.SelectOnLeft = Navi.SelectOnLeft;
			_ui.Navi.SelectOnRight = Navi.SelectOnRight;

			Contents.Add(_ui);
			_ui.transform.SetParent(ContentsParent);
			_ui.transform.localScale = Vector3.one;

			UIManager?.UIList.Add(_ui);
			_ui.UIManager = UIManager;
		}
	}
}