using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using AthensUtility;
using AthensUtility.Math;
using AthensUtility.Unity;
using System.Runtime.Serialization;

namespace FGOManager
{
	/// <summary> 数値アップ・ダウン系 </summary>
	[DataContract]
	public class EffUpDown : Effect
	{
		[DataMember] public bool IsUp { get; set; } = true;
		public override bool IsBuff => IsUp;
		protected virtual string TypeStr
		{
			get
			{
				switch (Type)
				{
					case Type_e.UD_Attack:
					case Type_e.UD_Defense:
					case Type_e.UD_NoblePhantasmPower:
					case Type_e.UD_CriticalPower:
					case Type_e.UD_CriticalIncidence:
					case Type_e.UD_StarGet:
					case Type_e.UD_SterWeight:
					case Type_e.UD_NPGet:
					case Type_e.UD_SufferDamageNPGet:
					case Type_e.UD_HPGet:
					case Type_e.UD_HPGive:
					case Type_e.UD_MaxHP:
						return Type.GetText();
					case Type_e.UD_SuccessRate:
						return $"{DetailType.GetText()}{(DetailType == DetailType_e.Buff ? "" : "付与")}成功率";
					case Type_e.UD_Resistance:
						return $"{DetailType.GetText()}{(DetailType == DetailType_e.Attack || DetailType == DetailType_e.Defense ? "弱体" : "")}耐性";
					case Type_e.UD_CardBuff:
					case Type_e.UD_CardResistance:
					default:
						throw new Exception();
				}
			}
		}

		public EffUpDown(Type_e _type, DetailType_e _detailType = DetailType_e.None) : base(_type, _detailType) { }
		public override string GetDescriptionText() => $"{TypeStr}を{(IsUp ? "アップ" : "ダウン")}";
	}

	/// <summary> カード系 </summary>
	[DataContract]
	public class EffUDCard : EffUpDown
	{
		[DataMember] private CommandCard_e m_CardType;
		public CommandCard_e CardType { get { return m_CardType; } set { if (value != CommandCard_e.Extra && value != CommandCard_e.NoblePhantasm) m_CardType = value; } }
		public EffUDCard(bool _isBuff) : base(_isBuff ? Type_e.UD_CardBuff : Type_e.UD_CardResistance) { }
		protected override string TypeStr => $"{CardType}{Type.GetText()}";
	}

}

