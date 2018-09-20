using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using AthensUtility;
using AthensUtility.Unity;
using AthensUtility.Unity.UI;

namespace FGOManager.Register
{
	public class MaterialsDisplay : CustomUI
	{
		[SerializeField]
		private CustomUIToggle m_TgLabel = null;
		[SerializeField]
		private CustomUIScrollView m_NodeList = null;
		[SerializeField]
		private ToggleGroup m_ContentGroup = null;

		public Toggle Toggle { get { return m_TgLabel.Toggle; } }
		public MaterialNode PrefabNode { get; private set; } = null;
		public MaterialNumber MaterialCount { get; private set; } = null;

		public override bool IsSelectet { get { return false; } }

		[ContextMenu("SetupNavigation")]
		private void SetupNavigation()
		{
			m_TgLabel.Navigation.SelectOnLeft = m_NodeList.Navigation.SelectOnLeft = Navigation.SelectOnLeft;
			m_TgLabel.Navigation.SelectOnRight = m_NodeList.Navigation.SelectOnRight = Navigation.SelectOnRight;
			m_TgLabel.Navigation.SelectOnUp = Navigation.SelectOnUp;
			m_NodeList.Navigation.SelectOnDown = Navigation.SelectOnDown;
		}

		protected override void Awake()
		{
			base.Awake();
			m_NodeList.OnSelect.AddListener(() => m_TgLabel.Toggle.isOn = true);
		}

		protected override void OnSelected(bool _isSelect)
		{
			if (_isSelect) m_TgLabel.IsSelect = true;
		}

		public void Setup(MaterialNode _prefab, MaterialNumber _mc, ToggleGroup _toggleGroup)
		{
			PrefabNode = _prefab;
			MaterialCount = _mc;
			Toggle.group = _toggleGroup;
			Toggle.isOn = false;
			var buff = MaterialCount.ToList();
			MaterialCount.ToList().ForEach(kv => MaterialCount[kv.Key] = 0);
			buff.ForEach(kv => AddNode(kv.Key, kv.Value));
		}

		public void AddNode(Material_e _material, int _count)
		{
			if (_count == 0 || MaterialCount?[_material] != 0) return;
			MaterialNode node = Instantiate(PrefabNode);
			node?.transform.SetParent(m_ContentGroup.transform);
			node.transform.localScale = Vector3.one;
			node.Material = _material;
			node.Count = _count;
			node.OnChangeValue.AddListener(c => MaterialCount[_material] = c);
			node.OnSelect.AddListener(() => Toggle.isOn = true);
			node.Toggle.group = m_ContentGroup;
			m_NodeList?.AddContent(node);
			MaterialCount[_material] = _count;

			m_NodeList?.OrderSort(ui => ui.GetComponent<MaterialNode>().Material);
		}

		public void RemoveNode(Material_e _material)
		{
			if (MaterialCount == null || !MaterialCount.ContainsKey(_material)) return;
			MaterialCount.Remove(_material);
			m_NodeList.RemoveContent(m_NodeList.Contents.First(ui => ui.GetComponent<MaterialNode>()?.Material == _material));
		}

		public void RemoveNode()
		{
			var mat = m_ContentGroup.ActiveToggles().FirstOrDefault()?.GetComponent<MaterialNode>().Material;
			if (mat != null)
				RemoveNode(mat.Value);
		}

		public void Clear() => m_NodeList.Clear();
	}
}