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
	public class MaterialsSetter : MonoBehaviour
	{
		[SerializeField]
		private MaterialNode m_PrefabNode = null;
		[SerializeField]
		private Dropdown m_Dropdown = null;
		[SerializeField]
		private ToggleGroup m_ToggleGroup = null;
		[SerializeField]
		private MaterialsDisplay[] m_MD_SeccondComming = new MaterialsDisplay[4];
		[SerializeField]
		private MaterialsDisplay[] m_MD_Skill = new MaterialsDisplay[8];

		public Dictionary<int, Material_e> MaterialIndex { get; set; } = null;

		[ContextMenu("Setup")]
		private void SetupContens()
		{
			m_ToggleGroup = GetComponent<ToggleGroup>() ?? gameObject.AddComponent<ToggleGroup>();
			if (m_Dropdown == null) return;
			var list = ExEnum.GetEnumIter<Material_e>()
				.Select(m => new { Material = m, Sprite = GameData.Instance.MaterialSprites[m] })
				.Where(x => x.Sprite != null)
				.Select((ms, i) => new { ms.Material, ms.Sprite, Index = i });
			MaterialIndex = list.ToDictionary(x => x.Index, x => x.Material);
			m_Dropdown.ClearOptions();
			m_Dropdown.AddOptions(list.Select(x => new Dropdown.OptionData { image = x.Sprite }).ToList());
		}

		private void Awake() => SetupContens();

		public void Setup(MaterialNumber[] _mnSC, MaterialNumber[] _mnSkill)
		{
			for (int i = 0; i < m_MD_SeccondComming.Length && i < _mnSC?.Length; i++)
				if (m_MD_SeccondComming[i]) m_MD_SeccondComming[i].Setup(m_PrefabNode, _mnSC[i], m_ToggleGroup);
			for (int i = 0; i < m_MD_Skill.Length && i < _mnSkill?.Length; i++)
				if (m_MD_Skill[i]) m_MD_Skill[i].Setup(m_PrefabNode, _mnSkill[i], m_ToggleGroup);
			m_MD_SeccondComming[0].Toggle.isOn = true;
		}

		public void AddNode(bool _isSC, int _step, Material_e _material, int _count) => (_isSC ? m_MD_SeccondComming : m_MD_Skill)[_step].AddNode(_material, _count);

		public void AddNode()
			=> m_ToggleGroup.ActiveToggles().FirstOrDefault()?.transform.parent?.GetComponent<MaterialsDisplay>()?.AddNode(MaterialIndex[m_Dropdown.value], 1);

		public void RemoveNode(bool _isSC, int _step, Material_e _material) => (_isSC ? m_MD_SeccondComming : m_MD_Skill)[_step].RemoveNode(_material);

		public void RemoveNode()
			=> m_ToggleGroup.ActiveToggles().FirstOrDefault()?.transform.parent
			?.GetComponent<MaterialsDisplay>()?.RemoveNode();

		public void Clear(bool _isSC) => (_isSC ? m_MD_SeccondComming : m_MD_Skill).ToList().ForEach(md => md.Clear());
		public void AllClear() => m_MD_SeccondComming.AddRetern(m_MD_Skill).ToList().ForEach(md => md.Clear());
	}
}