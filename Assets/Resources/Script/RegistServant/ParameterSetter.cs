using UnityEngine;
using UnityEngine.UI;
using KMUtility;

namespace FGOManager.Register
{
	public class ParameterSetter : MonoBehaviour
	{
		[SerializeField]
		private Parameter.Type_e m_ParameterType = Parameter.Type_e.Physics;
		[SerializeField]
		private Text m_Text = null;

		public RankEvent OnRankChange = null;

		private Rank m_Rank = new Rank();
		public Rank Rank { get { return m_Rank; } set { m_Rank = value; OnRankChange?.Invoke(m_Rank); } }
		public string RankOther { get { return m_Rank.Other; } set { m_Rank.Other = value; OnRankChange?.Invoke(m_Rank); } }

		void OnValidate() { if (m_Text) m_Text.text = $"{m_ParameterType.GetText()}："; }

		public void ChangeRank(int _value)
		{
			Rank.Type = _value == 0 ? Rank.Type_e.EX : _value == 16 ? Rank.Type_e.B : (Rank.Type_e)(1 - (_value - 2) / 3);
			Rank.Plus = (sbyte)(_value == 0 ? 0 : (_value <= 4 ? 3 - _value : (_value >= 14 ? 15 - _value : (13 - _value) % 3)));
			Rank.Other = _value == 16 ? "？" : "";
			OnRankChange?.Invoke(m_Rank);
		}
	}
}