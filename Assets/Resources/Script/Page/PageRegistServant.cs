using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KMUtility.Unity.UI;

namespace KMUtility.Unity.Page
{
	public class PageRegistServant : PageBase
	{
		[SerializeField]
		private CustomScrollView m_ScrollList = null;
		[SerializeField]
		private CustomUIButton m_ServantNameNode = null;

		private void Start()
		{
			for (int i = 0; i < 30; i++)
			{
				AddName(i.ToString());
			}
		}

		public void AddName(string _name)
		{
			CustomUIButton node = Instantiate(m_ServantNameNode);
			node.Text.text = _name;
			if (node)
				m_ScrollList?.AddContent(node);
		}
	}
}