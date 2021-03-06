﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AthensUtility.Unity.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class CustomUIScrollView : CustomUI
	{
		public List<CustomUI> Contents { get; private set; } = new List<CustomUI>();
		public RectTransform ContentsParent { get { return GetComponent<ScrollRect>().content; } }
		public CustomUI SelectUI { get { return Contents.FirstOrDefault(ui => ui.IsSelect); } }

		/// <summary> UI要素 </summary>
		public override Selectable UIObject { get { return GetComponent<Selectable>() ?? gameObject.AddComponent<Selectable>(); } }

		public override bool IsSelectet { get { return false; } }

		private void Start() { }

		protected override void OnSelected(bool _isSelect)
		{
			if (_isSelect)
				if (Contents.Any())
					((KeySet.Shift + KeyCode.Tab).GetKeyDown() ? Contents.Last() : Contents.First()).IsSelect = true;
				else if (Navigation.Next != null)
					Navigation.Next.IsSelect = true;
			if (!IsSelect && _isSelect) OnSelect?.Invoke();
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
			navi.mode = UnityEngine.UI.Navigation.Mode.Explicit;
			navi.selectOnUp = navi.selectOnDown = navi.selectOnLeft = navi.selectOnRight = null;
			_ui.UIObject.navigation = navi;

			_ui.Navigation.SelectOnLeft = Navigation.SelectOnLeft;
			_ui.Navigation.SelectOnRight = Navigation.SelectOnRight;
			Contents.Add(_ui);
			SetupChain();

			_ui.transform.SetParent(ContentsParent);
			_ui.transform.localScale = Vector3.one;
		}

		public void SetupChain()
		{
			if (!Contents.Any())
				return;
			Contents.First().Navigation.SelectOnUp = Navigation.SelectOnUp;
			for (int i = 0; i < Contents.Count; i++)
			{
				if (i != 0)
					Contents[i].Navigation.SelectOnUp = Contents[i - 1];
				if (i != Contents.Count - 1)
					Contents[i].Navigation.SelectOnDown = Contents[i + 1];
			}
			Contents.Last().Navigation.SelectOnDown = Navigation.SelectOnDown;
		}

		public void RemoveContent(CustomUI _ui)
		{
			if (!Contents.Contains(_ui)) return;
			Contents.Remove(_ui);
			Destroy(_ui.gameObject);
		}

		public void Clear()
		{
			Contents.ForEach(ui => Destroy(ui.gameObject));
			Contents.Clear();
		}

		public void OrderSort<T>(Func<CustomUI, T> _keySelector) where T : IComparable
		{
			ContentsParent.OrderSort(t => _keySelector(t.GetComponent<CustomUI>()));
			Contents = Contents.OrderBy(_keySelector).ToList();
			SetupChain();
		}
	}
}