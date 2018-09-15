using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KMUtility;
using KMUtility.Unity;
using KMUtility.Unity.UI;

namespace FGOManager.Register
{
	public class CharacteristicSetter : MonoBehaviour
	{
		[SerializeField]
		private CharacteristicNode m_PrefabCharacteristicNode = null;
		[SerializeField]
		private CustomUIScrollView m_NodeList = null;
		[SerializeField]
		private ToggleGroup m_ContentParent = null;

		public OptionDatasEvent SetupOpstions = null;

		public List<string> Characteristic { get; set; } = null;

		private void Start()
		{
			SetupDropdownOptions();
		}

		public void AddNode(string _str)
		{
			if (_str == "" || Characteristic?.Exists(s => s == _str) == true) return;
			CharacteristicNode node = Instantiate(m_PrefabCharacteristicNode);
			node?.transform.SetParent(m_ContentParent.transform);
			node.transform.localScale = Vector3.one;
			node.Text = _str;
			node.Toggle.group = m_ContentParent;
			m_NodeList?.AddContent(node);
			Characteristic?.Add(_str);

			m_NodeList?.OrderSort(ui => ui.GetComponent<CharacteristicNode>().Text);
			Characteristic.Sort((c0, c1) => c0.CompareTo(c1));
		}

		public void RemoveNode()
		{
			if (Characteristic?.Any() == false) return;
			CharacteristicNode node = m_ContentParent?.ActiveToggles().FirstOrDefault()?.GetComponent<CharacteristicNode>();
			if (!node) return;
			Characteristic.Remove(node.Text);
			SetupNodes();
		}

		public void SetupNodes()
		{
			if (Characteristic == null) return;
			var list = new List<string>(Characteristic).OrderBy(s => s);
			m_NodeList?.Clear();
			Characteristic.Clear();
			list.ToList().ForEach(AddNode);
		}

		public void SetupDropdownOptions()
			=> SetupOpstions?.Invoke(GameData.Instance.Servants.SelectMany(s => s.Characteristic)
				.AddRetern(FGOData.DefaultCharacteristics).Distinct().OrderBy(s => s)
				.Select(s => new Dropdown.OptionData { text = s }).ToList());
	}
}