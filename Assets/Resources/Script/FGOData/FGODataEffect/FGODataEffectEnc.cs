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
	/// <summary> 付与効果系 </summary>
	[DataContract]
	public class EffEnchant : Effect
	{
		public override bool IsBuff
		{
			get
			{
				switch (Type)
				{
					case Type_e.ENC_SpecialAttack:
					case Type_e.ENC_SpecialDefense:
					case Type_e.ENC_PlusDamage:
					case Type_e.ENC_CutDamage:
					case Type_e.ENC_UpHitCount:
					case Type_e.ENC_EveryTurnGetStar:
					case Type_e.ENC_EveryTurnGetNP:
					case Type_e.ENC_EveryTurnRecoveryHP:
					case Type_e.ENC_Guts:
					case Type_e.ENC_TargetConcentration:
					case Type_e.ENC_ChangeClassCompatibility:
					case Type_e.ENC_AddOverCharge:
					case Type_e.ENC_Counter:
					case Type_e.ENC_Invalid:
					case Type_e.ENC_Ignored:
						return true;
					case Type_e.ENC_SealNoblePhantasm:
					case Type_e.ENC_SealSkill:
					case Type_e.ENC_AddCharacteristic:
					case Type_e.ENC_Fear:
					case Type_e.ENC_Confusion:
					case Type_e.ENC_Charging:
					case Type_e.ENC_Inactivity:
					case Type_e.ENC_SlipDamage:
					case Type_e.ENC_DoubledSlipDamage:
						return false;
					case Type_e.ENC_Disabled:
						return DetailType != DetailType_e.Buff;
					case Type_e.ENC_State:
					default:
						throw new Exception();
				}
			}
		}

		public EffEnchant(Type_e _type, DetailType_e _detailType = DetailType_e.None) : base(_type, _detailType) { }
		public override string GetDescriptionText()
		{
			switch (Type)
			{
				case Type_e.ENC_SpecialAttack:
				case Type_e.ENC_SpecialDefense:
					return $"{Fillter}{Type.GetText()}状態を付与";
				case Type_e.ENC_UpHitCount:
					return "自身に通常攻撃のヒット数が2倍になる状態を付与";
				case Type_e.ENC_AddOverCharge:
					return "味方単体の宝具使用時のチャージ段階を2段階引き上げる";
				case Type_e.ENC_Counter:
					return $"{Type.GetText()}";
				case Type_e.ENC_PlusDamage:
				case Type_e.ENC_CutDamage:
				case Type_e.ENC_EveryTurnGetStar:
				case Type_e.ENC_EveryTurnGetNP:
				case Type_e.ENC_EveryTurnRecoveryHP:
				case Type_e.ENC_Guts:
				case Type_e.ENC_TargetConcentration:
				case Type_e.ENC_SealNoblePhantasm:
				case Type_e.ENC_SealSkill:
				case Type_e.ENC_Fear:
				case Type_e.ENC_Confusion:
				case Type_e.ENC_Charging:
				case Type_e.ENC_Invalid:
					return $"{DetailType.GetText()}状態を付与";
				case Type_e.ENC_Ignored:
					switch (DetailType)
					{
						case DetailType_e.Defense:
							return $"防御力無視状態を付与";
						case DetailType_e.Avoidance:
							return $"必中状態を付与";
						case DetailType_e.Invincible:
							return $"無敵貫通状態を付与";
						default:
							throw new Exception();
					}
				case Type_e.ENC_Disabled:
					return $"{DetailType.GetText()}無効状態を付与";
				case Type_e.ENC_DoubledSlipDamage:
					switch (DetailType)
					{
						case DetailType_e.SD_Burn:
							return $"延焼状態を付与";
						case DetailType_e.SD_Poison:
							return $"触毒状態を付与";
						case DetailType_e.SD_Curse:
							return $"呪厄状態を付与";
						default:
							throw new Exception();
					}
				case Type_e.ENC_Inactivity:
				case Type_e.ENC_SlipDamage:
					return $"{Type.GetText()}状態を付与";
				case Type_e.ENC_ChangeClassCompatibility:
				case Type_e.ENC_AddCharacteristic:
				case Type_e.ENC_State:
				default:
					throw new Exception();
			}
		}
	}

	/// <summary> クラス相性変更 </summary>
	[DataContract]
	public class EffEncChangeClassCompatibility : EffEnchant
	{
		/// <summary> 対象クラス </summary>
		[DataMember] public Class_e Class { get; set; }
		/// <summary> 対象クラスへ対しての操作か(falseの場合対象クラスからの攻撃・防御変更) </summary>
		[DataMember] public bool IsToClass { get; set; } = true;
		/// <summary> 攻撃相性を変更するかどうか </summary>
		[DataMember] public bool IsAttack { get; set; } = false;
		/// <summary> 防御相性を変更するかどうか </summary>
		[DataMember] public bool IsDefense { get; set; } = false;
		public EffEncChangeClassCompatibility() : base(Type_e.ENC_ChangeClassCompatibility) { }
		public override string GetDescriptionText()
			=> $"{Class.GetText()}クラス{(IsToClass ? "に対する" : "の")}{((IsAttack && IsDefense) ? "相性" : IsAttack ? "攻撃" : "防御")}不利を打ち消す状態を付与";
	}

	/// <summary> 特性付与 </summary>
	[DataContract]
	public class EffEncAddCharacteristic : EffEnchant
	{
		/// <summary> 追加特性 </summary>
		[DataMember] public string Characteristic { get; set; }
		public EffEncAddCharacteristic() : base(Type_e.ENC_AddCharacteristic) { }
		public override string GetDescriptionText() => $"〔{Characteristic}〕特性を付与";
	}

	/// <summary> 名称のみの状態付与 </summary>
	[DataContract]
	public class EffEncState : EffEnchant
	{
		public override bool IsBuff => IsBuffState;
		/// <summary> バフかどうか </summary>
		[DataMember] public bool IsBuffState { get; set; }
		/// <summary> 状態名 </summary>
		[DataMember] public string State { get; set; }
		public EffEncState() : base(Type_e.ENC_State) { }
		public override string GetDescriptionText() => $"{State}状態を付与";
	}

}

