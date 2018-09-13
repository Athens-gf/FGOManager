using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KMUtility.Unity;
using KMUtility.Unity.UI;

namespace FGOManager.Register
{
	public class CommandCardSetter : MonoBehaviour
	{
		[Serializable]
		public class Setup_ic
		{
			public IntEvent HitQ;
			public IntEvent HitA;
			public IntEvent HitB;
			public IntEvent HitEx;
			public IntEvent HitNP;
		}

		[SerializeField]
		private CustomUIDropdown m_DpQ = null, m_DpA = null;
		[SerializeField]
		private Text m_TextB = null;

		public CommandCardEvent OnValueChanged;
		public Setup_ic Setup = new Setup_ic();

		public int Q { get { return m_DpQ.Dropdown.value; } }
		public int A { get { return m_DpA.Dropdown.value; } }
		public int B { get { return 2 - Q - A; } }
		public CommandCard.Type_e CommandCardType
		{
			get { return (CommandCard.Type_e)((Q > 0 ? CommandCard_e.Quick : 0) | (A > 0 ? CommandCard_e.Arts : 0) | (B > 0 ? CommandCard_e.Buster : 0)); }
			set
			{
				m_DpQ.Dropdown.value = value.GetCount(CommandCard_e.Quick) - 1;
				m_DpA.Dropdown.value = value.GetCount(CommandCard_e.Arts) - 1;
				m_TextB.text = (B + 1).ToString();
			}
		}
		private CommandCard m_CommandCard;
		public CommandCard CommandCard
		{
			get { return m_CommandCard; }
			set
			{
				m_CommandCard = value;
				CommandCardType = value.Type;
				Setup?.HitQ?.Invoke(value[CommandCard_e.Quick].Hit);
				Setup?.HitA?.Invoke(value[CommandCard_e.Arts].Hit);
				Setup?.HitB?.Invoke(value[CommandCard_e.Buster].Hit);
				Setup?.HitEx?.Invoke(value[CommandCard_e.Extra].Hit);
				Setup?.HitNP?.Invoke(value[CommandCard_e.NoblePhantasm].Hit);
			}
		}
		public int HitQ
		{
			get { return CommandCard[CommandCard_e.Quick].Hit; }
			set { CommandCard[CommandCard_e.Quick].Hit = (sbyte)value; OnValueChanged?.Invoke(CommandCard); }
		}
		public int HitA
		{
			get { return CommandCard[CommandCard_e.Arts].Hit; }
			set { CommandCard[CommandCard_e.Arts].Hit = (sbyte)value; OnValueChanged?.Invoke(CommandCard); }
		}
		public int HitB
		{
			get { return CommandCard[CommandCard_e.Buster].Hit; }
			set { CommandCard[CommandCard_e.Buster].Hit = (sbyte)value; OnValueChanged?.Invoke(CommandCard); }
		}
		public int HitEx
		{
			get { return CommandCard[CommandCard_e.Extra].Hit; }
			set { CommandCard[CommandCard_e.Extra].Hit = (sbyte)value; OnValueChanged?.Invoke(CommandCard); }
		}
		public int HitNP
		{
			get { return CommandCard[CommandCard_e.NoblePhantasm].Hit; }
			set { CommandCard[CommandCard_e.NoblePhantasm].Hit = (sbyte)value; OnValueChanged?.Invoke(CommandCard); }
		}

		public void ChangeQ()
		{
			m_DpA.Dropdown.ClearOptions();
			m_DpA.Dropdown.AddOptions((new int[3 - Q]).Select((_, i) => (i + 1).ToString()).ToList());
			m_TextB.text = (B + 1).ToString();
			CommandCard.Type = CommandCardType;
			OnValueChanged?.Invoke(CommandCard);
		}

		public void ChangeA()
		{
			m_DpQ.Dropdown.ClearOptions();
			m_DpQ.Dropdown.AddOptions((new int[3 - A]).Select((_, i) => (i + 1).ToString()).ToList());
			m_TextB.text = (B + 1).ToString();
			CommandCard.Type = CommandCardType;
			OnValueChanged?.Invoke(CommandCard);
		}
	}
}