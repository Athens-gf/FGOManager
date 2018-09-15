using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KMUtility.Unity.Page
{
	/// <summary> ページ管理クラス </summary>
	public class PageManager : MonoBehaviour
	{
		public Stack<PageBase> Pages { get; private set; } = new Stack<PageBase>();

		// Use this for initialization
		void Start()
		{
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
