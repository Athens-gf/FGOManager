using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KMUtility.Unity.Page
{
	[Serializable]
	public class ComparableButton : IComparable
	{
		public Button Button = null;
		public int CompareTo(object obj) { return 0; }
	}

	[Serializable]
	public class ButtonPaseDict : UnityDictionary<ComparableButton, PageBase, ButtonPasePair> { }
	[Serializable]
	public class ButtonPasePair : KeyAndValue<ComparableButton, PageBase> { }

	/// <summary> ページ管理クラス </summary>
	public class PageManager : MonoBehaviour
	{
		public ButtonPaseDict ButtonPaseDict = null;

		public Stack<PageBase> Pages { get; private set; } = new Stack<PageBase>();

		// Use this for initialization
		void Start()
		{
			foreach (var button in ButtonPaseDict.Keys)
				button.Button.onClick.AddListener(() => AddPage(ButtonPaseDict[button]));

		}

		/// <summary> ページを追加する </summary>
		/// <param name="_page">追加するページ</param>
		public PageBase AddPage(PageBase _page)
		{
			var page = Instantiate(_page, transform);
			Pages.Push(page);
			page.gameObject.SetActive(true);
			page.ButtonBack.onClick.AddListener(() => DestroyPage());
			return page;
		}

		/// <summary> ページを戻す </summary>
		public void DestroyPage()
		{
			if (Pages.Count > 0)
				Pages.Pop().Destroy();
		}
	}
}
