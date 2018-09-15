using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
using KMUtility.Math;
using KMUtility.Unity;
using System.Runtime.Serialization;

namespace FGOManager
{
	/// <summary> 数値アップ・ダウン系 </summary>
	[DataContract]
	public class EffUpDown : Effect
	{
		public bool IsUp { get; set; } = true;
		public override bool IsBuff => IsUp;
		protected virtual string TypeStr => Type.GetText();
		public EffUpDown(Type_e _type) : base(_type) { }
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

	/// <summary> 成功率系 </summary>
	[DataContract]
	public class EffUDSuccessRate : EffUpDown
	{
		[DataMember] private CommandCard_e m_CardType;
		public CommandCard_e CardType { get { return m_CardType; } set { if (value != CommandCard_e.Extra && value != CommandCard_e.NoblePhantasm) m_CardType = value; } }
		public EffUDSuccessRate(Type_e _type) : base(_type) { }
		protected override string TypeStr => $"{CardType}{Type.GetText()}";
	}

	/// <summary>  </summary>
	[DataContract]
	public class EffUD : EffUpDown
	{
		public EffUD() : base(Type_e.UD_Attack) { }
		protected override string TypeStr => $"";
	}

}

