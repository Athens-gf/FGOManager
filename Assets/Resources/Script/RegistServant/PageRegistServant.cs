using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KMUtility.Unity;
using KMUtility.Unity.UI;
using KMUtility.Unity.Page;

namespace FGOManager.Register
{
	public class PageRegistServant : PageBase
	{
		[SerializeField]
		private CustomScrollView m_ScrollList = null;
		[SerializeField]
		private ServantNode m_ServantNameNode = null;

		private void Start()
		{
			foreach (var servant in GameData.Instance.Servants)
				AddNode(servant);
		}

		public void AddNode(ServantBase _servant)
		{
			ServantNode node = Instantiate(m_ServantNameNode);
			node.Servant = _servant;
			node.OnClick = () => OnSelect(_servant);
			if (node)
			{
				m_ScrollList?.AddContent(node);
				m_ScrollList?.OrderSort((c0, c1) => c0.GetComponent<ServantNode>()?.Servant.No.CompareTo(c1.GetComponent<ServantNode>()?.Servant.No) ?? 0);
			}
		}

		public void OnSelect(ServantBase _servant)
		{
			Debug.Log(_servant.Name);
		}

		public void Save()
		{
			var servants = GameData.Instance.Servants;
			if (servants.Any())
			{
				List<string> filePaths = new List<string>();
				int count = (servants.Max(s => s.No) - 1) / 10 + 1;
				for (int i = 0; i < count; i++)
				{
					List<ServantBase> saveSev = servants.Where(s => s.No > i * 10 && s.No <= (i + 1) * 10).ToList();
					string filePath = $"Data/sev{i}.png";
					if (saveSev.Any())
						SaveJsonPng.SaveList(filePath, saveSev);
					filePaths.Add(filePath);
				}
				SaveJsonPng.SaveList(GameData.SavePath, filePaths);
			}
		}

		public void NewRegist()
		{

		}

		public void Delete()
		{

		}

		public void ChangeNo(int _no)
		{
		}

		public void ChangeName(string _name)
		{
		}

		public void ChangeClass(Class_e _class)
		{
			Debug.Log(_class);
		}
	}
}