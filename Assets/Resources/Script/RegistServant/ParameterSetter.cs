using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KMUtility;
using System;

namespace FGOManager.Register
{
	public class ParameterSetter : MonoBehaviour
	{
		[SerializeField]
		private Parameter.Type_e m_ParameterType = Parameter.Type_e.Physics;
		[SerializeField]
		private Text m_Text = null;
		[SerializeField]
		private Dropdown m_Dropdown = null;

		public RankEvent OnRankChange = null;

		public int DefaultBIndex => m_Dropdown.options.FirstIndex(op => op.text == "B");

		private Rank m_Rank = new Rank();
		public Rank Rank
		{
			get { return m_Rank; }
			set
			{
				if (m_Rank.ToString() == value.ToString()) return;
				m_Rank = value;
				int index = m_Dropdown.options.FirstIndex(op => op.text == value.ToString(), DefaultBIndex);
				m_Dropdown.value = index;
				OnRankChange?.Invoke(m_Rank);
			}
		}

		[ContextMenu("SetupDropdown")]
		private void SetupDropdown()
		{
			m_Dropdown.ClearOptions();
			m_Dropdown.AddOptions(
				ExEnum.GetEnumIter<Rank.Type_e>()
				.OrderByDescending(t => t)
				.SelectMany(t => Enumerable.Range(t == Rank.Type_e.A ? -1 : 0,
				t == Rank.Type_e.EX ? 1
				: t == Rank.Type_e.A ? 4
				: t == Rank.Type_e.E ? 2
				: 3)
				.Select(p => (new Rank { Type = t, Plus = (sbyte)p, Other = "" }).ToString())
				.Reverse())
				.AddRetern("？")
				.Select(str => new Dropdown.OptionData { text = str })
				.ToList());
			m_Dropdown.value = DefaultBIndex;

		}

		public string RankOther { get { return m_Rank.Other; } set { m_Rank.Other = value; OnRankChange?.Invoke(m_Rank); } }

		void OnValidate() { if (m_Text) m_Text.text = $"{m_ParameterType.GetText()}："; }

		public void ChangeRank(int _value) => Rank = Rank.FromString(m_Dropdown.options[_value].text);

	}
}