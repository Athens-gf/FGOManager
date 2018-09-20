using System;
using System.Linq;
using System.Collections.Generic;
using AthensUtility;
using System.Runtime.Serialization;

namespace FGOManager
{
	public static partial class FGOData
	{
		/// <summary> 効果の対象が味方かどうか </summary>
		/// <param name="_target">効果の対象</param>
		public static bool IsParty(this Effect.Target_e _target)
		{
			switch (_target)
			{

				case Effect.Target_e.Party_Whole:
				case Effect.Target_e.Party_Single:
				case Effect.Target_e.Party_Myself:
				case Effect.Target_e.Party_OtherMyself:
					return true;
				case Effect.Target_e.Enemy_Whole:
				case Effect.Target_e.Enemy_Single:
					return false;
				default:
					throw new Exception();
			}
		}

		/// <summary> 効果の対象が全体かどうか </summary>
		/// <param name="_target">効果の対象</param>
		public static bool IsWhole(this Effect.Target_e _target)
		{
			switch (_target)
			{
				case Effect.Target_e.Party_Whole:
				case Effect.Target_e.Enemy_Whole:
					return true;
				case Effect.Target_e.Party_Single:
				case Effect.Target_e.Party_Myself:
				case Effect.Target_e.Party_OtherMyself:
				case Effect.Target_e.Enemy_Single:
					return false;
				default:
					throw new Exception();
			}
		}

	}

	/// <summary> 効果フィルター </summary>
	[DataContract]
	public class Fillter
	{
		public static Dictionary<string, Fillter> Fillers { get; } = new Dictionary<string, Fillter>();

		static Fillter()
		{
			FGOData.DefaultCharacteristics.ToList().ForEach(c => Fillers[c] = new Fillter { Name = c, Tags = new List<string> { c } });
		}

		[DataMember] public string Name { get; set; } = "";
		[DataMember] public List<string> Tags { get; private set; } = new List<string>();
		[DataMember] public bool IsTargetServant { get; set; } = false;

		public bool IsTarget(ICharacter _character) => (!IsTargetServant || _character.IsServant) && Tags.Except(_character.Tags).Any();

		public override string ToString() => $"〔{Name}〕";
	}

	/// <summary> 効果抽象クラス </summary>
	[KnownType(typeof(EffAttack))]
	[KnownType(typeof(EffUpDown))]
	[KnownType(typeof(EffUDCard))]
	[KnownType(typeof(EffEnchant))]
	[KnownType(typeof(EffEncChangeClassCompatibility))]
	[KnownType(typeof(EffEncAddCharacteristic))]
	[KnownType(typeof(EffEncState))]
	[KnownType(typeof(EffImmediate))]
	[DataContract]
	public abstract class Effect
	{
		[DataMember] private sbyte m_Times = 0;
		[DataMember] private sbyte m_Turn = 0;
		[DataMember] private decimal m_ActivationRate = -1;

		public enum DetailType_e
		{
			None = 0,
			/// <summary> 強化 </summary>
			[EnumText("強化")] Buff = 100,
			/// <summary> 攻撃 </summary>
			[EnumText("攻撃")] Attack,
			/// <summary> 防御 </summary>
			[EnumText("防御")] Defense,
			/// <summary> 回避 </summary>
			[EnumText("回避")] Avoidance,
			/// <summary> 無敵 </summary>
			[EnumText("無敵")] Invincible,

			/// <summary> 弱体 </summary>
			[EnumText("弱体")] Debuff = 200,
			/// <summary> 精神異常 </summary>
			[EnumText("精神異常")] MentalAbnormality,
			/// <summary> 即死 </summary>
			[EnumText("即死")] InstantDeath,

			SlipDamage,
			/// <summary> やけど </summary>
			[EnumText("やけど")] SD_Burn,
			/// <summary> 毒 </summary>
			[EnumText("毒")] SD_Poison,
			/// <summary> 呪い </summary>
			[EnumText("呪い")] SD_Curse,

			/// <summary> 行動不能 </summary>
			[EnumText("行動不能")] Inactivity,
			/// <summary> スタン </summary>
			[EnumText("スタン")] Stan,
			/// <summary> 魅了 </summary>
			[EnumText("魅了")] Charm,
			/// <summary> 豚化 </summary>
			[EnumText("豚化")] Pig,


		}

		public enum Type_e
		{
			/// <summary> 宝具攻撃 </summary>
			NoblePhantasmAttack = 0,

			// 上昇・減少系
			UP = 100,
			/// <summary> カード性能 </summary>
			[EnumText("カード性能")] UD_CardBuff,
			/// <summary> カード耐性 </summary>
			[EnumText("カード耐性")] UD_CardResistance,
			/// <summary> 攻撃力 </summary>
			[EnumText("攻撃力")] UD_Attack,
			/// <summary> 防御力 </summary>
			[EnumText("防御力")] UD_Defense,
			/// <summary> 宝具威力 </summary>
			[EnumText("宝具威力")] UD_NoblePhantasmPower,
			/// <summary> クリティカル威力 </summary>
			[EnumText("クリティカル威力")] UD_CriticalPower,
			/// <summary> クリティカル発生率 </summary>
			[EnumText("クリティカル発生率")] UD_CriticalIncidence,
			/// <summary> スター発生率 </summary>
			[EnumText("スター発生率")] UD_StarGet,
			/// <summary> スター集中度 </summary>
			[EnumText("スター集中度")] UD_SterWeight,
			/// <summary> NP獲得量 </summary>
			[EnumText("NP獲得量")] UD_NPGet,
			/// <summary> 被ダメージNP獲得量 </summary>
			[EnumText("被ダメージNP獲得量")] UD_SufferDamageNPGet,
			/// <summary> 被HP回復量 </summary>
			[EnumText("被HP回復量")] UD_HPGet,
			/// <summary> 与HP回復量 </summary>
			[EnumText("与HP回復量")] UD_HPGive,
			/// <summary> 最大HP </summary>
			[EnumText("最大HP")] UD_MaxHP,
			/// <summary> 成功率 </summary>
			[EnumText("成功率")] UD_SuccessRate,
			/// <summary> 耐性 </summary>
			[EnumText("耐性")] UD_Resistance,

			// 付与系
			ENC = 200,
			/// <summary> 特攻 </summary>
			[EnumText("特攻")] ENC_SpecialAttack,
			/// <summary> 特防 </summary>
			[EnumText("特防")] ENC_SpecialDefense,
			/// <summary> 与ダメージプラス </summary>
			[EnumText("与ダメージプラス")] ENC_PlusDamage,
			/// <summary> 被ダメージカット </summary>
			[EnumText("被ダメージカット")] ENC_CutDamage,
			/// <summary> ヒット数増加 </summary>
			[EnumText("ヒット数増加")] ENC_UpHitCount,
			/// <summary> 毎ターンスター獲得 </summary>
			[EnumText("毎ターンスター獲得")] ENC_EveryTurnGetStar,
			/// <summary> 毎ターンNP獲得 </summary>
			[EnumText("毎ターンNP獲得")] ENC_EveryTurnGetNP,
			/// <summary> 毎ターンHP回復 </summary>
			[EnumText("毎ターンHP回復")] ENC_EveryTurnRecoveryHP,
			/// <summary> ガッツ </summary>
			[EnumText("ガッツ")] ENC_Guts,
			/// <summary> ターゲット集中 </summary>
			[EnumText("ターゲット集中")] ENC_TargetConcentration,
			/// <summary> クラス相性変更 </summary>
			[EnumText("クラス相性変更")] ENC_ChangeClassCompatibility,
			/// <summary> 宝具チャージ段階引き上げ </summary>
			[EnumText("宝具チャージ段階引き上げ")] ENC_AddOverCharge,
			/// <summary> 宝具封印 </summary>
			[EnumText("宝具封印")] ENC_SealNoblePhantasm,
			/// <summary> スキル封印 </summary>
			[EnumText("スキル封印")] ENC_SealSkill,
			/// <summary> 特性付与 </summary>
			[EnumText("特性付与")] ENC_AddCharacteristic,
			/// <summary> そのターン中に受けたダメージを敵全体に倍化して返す </summary>
			[EnumText("そのターン中に受けたダメージを敵全体に倍化して返す")] ENC_Counter,
			/// <summary> 名前だけの状態付与 </summary>
			[EnumText("名前だけの状態付与")] ENC_State,

			/// <summary> 恐怖 </summary>
			[EnumText("恐怖")] ENC_Fear,
			/// <summary> 混乱 </summary>
			[EnumText("混乱")] ENC_Confusion,
			/// <summary> 帯電 </summary>
			[EnumText("帯電")] ENC_Charging,

			/// <summary> ダメージ無効化 </summary>
			[EnumText("ダメージ無効化")] ENC_Invalid,
			/// <summary> 無視 </summary>
			[EnumText("防御無視")] ENC_Ignored,
			/// <summary> 無効 </summary>
			[EnumText("無効")] ENC_Disabled,
			/// <summary> 行動不能 </summary>
			[EnumText("行動不能")] ENC_Inactivity,
			/// <summary> 持続ダメージ </summary>
			[EnumText("持続ダメージ")] ENC_SlipDamage,
			/// <summary> 持続ダメージ増強 </summary>
			[EnumText("持続ダメージ増強")] ENC_DoubledSlipDamage,


			// 即時発動系
			IMM = 300,
			/// <summary> スター獲得 </summary>
			[EnumText("スター獲得")] IMM_GetStar,
			/// <summary> NP増加 </summary>
			[EnumText("NP増加")] IMM_GetNP,
			/// <summary> HP回復 </summary>
			[EnumText("HP回復")] IMM_RecoveryHP,
			/// <summary> 強化解除 </summary>
			[EnumText("強化解除")] IMM_CanselBuff,
			/// <summary> 弱体解除 </summary>
			[EnumText("弱体解除")] IMM_ReleaseDebuff,
			/// <summary> 宝具チャージ上昇 </summary>
			[EnumText("宝具チャージ上昇")] IMM_AddCharge,
			/// <summary> 宝具チャージ減少 </summary>
			[EnumText("宝具チャージ減少")] IMM_DecreaseCharge,
			/// <summary> スキルチャージを進める </summary>
			[EnumText("スキルチャージを進める")] IMM_HasteSkillCharge,
			/// <summary> スキルチャージを戻す </summary>
			[EnumText("スキルチャージを戻す")] IMM_DelaySkillCharge,
			/// <summary> HP残量にしたがって追加ダメージ </summary>
			[EnumText("HP残量にしたがって追加ダメージ")] IMM_DamageByRemainingHP,
		}
		public enum Target_e
		{
			[EnumText("味方全体")] Party_Whole,
			[EnumText("味方単体")] Party_Single,
			[EnumText("自身")] Party_Myself,
			[EnumText("自身を除く味方全体")] Party_OtherMyself,
			[EnumText("敵全体")] Enemy_Whole,
			[EnumText("敵単体")] Enemy_Single,
		}
		public enum LvType_e
		{
			/// <summary> スキルレベル </summary>
			Skill,
			/// <summary> 宝具レベル </summary>
			NP,
			/// <summary> 宝具オーバーチャージ </summary>
			OC,
		}

		/// <summary> 効果の種別 </summary>
		[DataMember] public Type_e Type { get; set; }
		/// <summary> 効果の種別詳細 </summary>
		[DataMember] public DetailType_e DetailType { get; set; } = DetailType_e.None;
		/// <summary> 効果の範囲 </summary>
		[DataMember] public Target_e Target { get; set; }
		/// <summary> 効果の変化量タイプ </summary>
		[DataMember] public LvType_e LvType { get; set; }

		/// <summary> 効果のターン数(0で無限持続or即時) </summary>
		public sbyte Turn { get { return m_Turn; } set { m_Turn = Math.Max(value, (sbyte)0); } }
		/// <summary> 効果の回数(0で無限回、ターン持続が終わるまで) </summary>
		public sbyte Times { get { return m_Times; } set { m_Times = Math.Max(value, (sbyte)0); } }
		/// <summary> 発動率(負の場合確定発動) </summary>
		public decimal ActivationRate { get { return m_ActivationRate; } set { m_ActivationRate = Math.Max(value, -1); } }
		/// <summary> 効果の変化量 </summary>
		[DataMember] public List<decimal> Values { get; private set; } = new List<decimal>();
		/// <summary> 発動率を効果量とし、効果量を発動率とする </summary>
		[DataMember] public bool IsChangeValueAndActivationRate { get; set; } = false;

		/// <summary> 毎ターン徐々に効果を発生させるかどうか </summary>
		[DataMember] public bool IsGradually { get; set; } = false;
		/// <summary> デメリットかどうか </summary>
		[DataMember] public bool IsDemerit { get; set; } = false;

		/// <summary> 対象範囲 </summary>
		[DataMember] public Fillter Fillter { get; set; } = null;

		/// <summary> 効果の説明 </summary>
		[DataMember] public string Description { get; set; } = "";

		/// <summary> 精神異常系かどうか </summary>
		public bool IsMentalAbnormalityType => (Type == Type_e.ENC_Inactivity && DetailType == DetailType_e.Charm) || Type == Type_e.ENC_Fear || Type == Type_e.ENC_Confusion;
		/// <summary> 攻撃強化系かどうか </summary>
		public bool IsAttakType
		{
			get
			{
				switch (Type)
				{
					case Type_e.UD_CardBuff:
					case Type_e.UD_Attack:
					case Type_e.UD_NoblePhantasmPower:
					case Type_e.UD_CriticalPower:
					case Type_e.UD_CriticalIncidence:
					case Type_e.UD_StarGet:
					case Type_e.ENC_SpecialAttack:
					case Type_e.ENC_PlusDamage:
					case Type_e.ENC_UpHitCount:
					case Type_e.ENC_Ignored:
						return true;
					default:
						return false;
				}
			}
		}
		/// <summary> 防御強化系かどうか </summary>
		public bool IsDefencetype
		{
			get
			{
				switch (Type)
				{
					case Type_e.UD_CardResistance:
					case Type_e.UD_Defense:
					case Type_e.UD_Resistance:
					case Type_e.ENC_SpecialDefense:
					case Type_e.ENC_CutDamage:
					case Type_e.ENC_Invalid:
					case Type_e.ENC_Disabled:
						return true;
					default:
						return false;
				}
			}
		}

		public Effect(Type_e _type, DetailType_e _detailType = DetailType_e.None) { Type = _type; DetailType = _detailType; }

		protected void CheckDetailType()
		{
			switch (Type)
			{
				case Type_e.UD_SuccessRate:
					switch (DetailType)
					{
						case DetailType_e.Buff:
						case DetailType_e.Debuff:
						case DetailType_e.MentalAbnormality:
						case DetailType_e.InstantDeath:
						case DetailType_e.Inactivity:
						case DetailType_e.Stan:
						case DetailType_e.Charm:
						case DetailType_e.Pig:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.UD_Resistance:
					switch (DetailType)
					{
						case DetailType_e.Attack:
						case DetailType_e.Defense:
						case DetailType_e.Debuff:
						case DetailType_e.MentalAbnormality:
						case DetailType_e.InstantDeath:
						case DetailType_e.SD_Burn:
						case DetailType_e.SD_Poison:
						case DetailType_e.SD_Curse:
						case DetailType_e.Inactivity:
						case DetailType_e.Stan:
						case DetailType_e.Charm:
						case DetailType_e.Pig:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.ENC_Invalid:
					if (DetailType != DetailType_e.Avoidance && DetailType != DetailType_e.Invincible)
						throw new Exception();
					break;
				case Type_e.ENC_Ignored:
					switch (DetailType)
					{
						case DetailType_e.Defense:
						case DetailType_e.Avoidance:
						case DetailType_e.Invincible:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.ENC_Disabled:
					switch (DetailType)
					{
						case DetailType_e.Buff:
						case DetailType_e.Debuff:
						case DetailType_e.MentalAbnormality:
						case DetailType_e.InstantDeath:
						case DetailType_e.SlipDamage:
						case DetailType_e.SD_Burn:
						case DetailType_e.SD_Poison:
						case DetailType_e.SD_Curse:
						case DetailType_e.Inactivity:
						case DetailType_e.Stan:
						case DetailType_e.Charm:
						case DetailType_e.Pig:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.ENC_Inactivity:
					switch (DetailType)
					{
						case DetailType_e.Inactivity:
						case DetailType_e.Stan:
						case DetailType_e.Charm:
						case DetailType_e.Pig:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.ENC_SlipDamage:
				case Type_e.ENC_DoubledSlipDamage:
					switch (DetailType)
					{
						case DetailType_e.SD_Burn:
						case DetailType_e.SD_Poison:
						case DetailType_e.SD_Curse:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.IMM_CanselBuff:
					switch (DetailType)
					{
						case DetailType_e.Buff:
						case DetailType_e.Attack:
						case DetailType_e.Defense:
						case DetailType_e.Avoidance:
						case DetailType_e.Invincible:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.IMM_ReleaseDebuff:
					switch (DetailType)
					{
						case DetailType_e.Debuff:
						case DetailType_e.MentalAbnormality:
						case DetailType_e.SlipDamage:
						case DetailType_e.SD_Burn:
						case DetailType_e.SD_Poison:
						case DetailType_e.SD_Curse:
						case DetailType_e.Inactivity:
						case DetailType_e.Stan:
						case DetailType_e.Charm:
						case DetailType_e.Pig:
							break;
						default:
							throw new Exception();
					}
					break;
				case Type_e.IMM_AddCharge:
					break;
				case Type_e.IMM_DecreaseCharge:
					break;
				case Type_e.IMM_HasteSkillCharge:
					break;
				case Type_e.IMM_DelaySkillCharge:
					break;
				case Type_e.IMM_DamageByRemainingHP:
					break;
				default:
					if (DetailType != DetailType_e.None)
						throw new Exception();
					break;
			}
		}

		protected void SetupValuesSize()
		{
			Values = new List<decimal>();
			for (int i = 0; i < (LvType == LvType_e.Skill ? 10 : 5); i++) Values.Add(0);
		}

		protected string GetTargetText() => Target.GetText() + (Fillter == null ? "" : $"{Fillter}");

		protected string GetEndText()
		{
			string str = "";
			if (LvType != LvType_e.OC) str += "[Lv.]";
			if (Turn > 0 || Times > 0)
			{
				str += "(";
				if (Times > 0) str += $"{Times}回";
				if (Turn > 0 && Times > 0) str += "・";
				if (Turn > 0) str += $"{Turn}ターン";
				str += ")";
			}
			if (LvType == LvType_e.OC) str += "〈オーバーチャージで効果アップ〉";
			if (IsDemerit) str += "【デメリット】";
			return str;
		}

		public abstract bool IsBuff { get; }
		public abstract string GetDescriptionText();

		public string BaseDescriptionText { get { return GetDescriptionText() + GetEndText(); } }
	}

	/// <summary> 宝具攻撃クラス </summary>
	[DataContract]
	public class EffAttack : Effect
	{
		/// <summary> 宝具威力の基本値(Arts) </summary>
		private static readonly decimal[] BasePower = new decimal[] { 450, 600, 675, 712.5m, 750 };
		/// <summary> 宝具威力の強化値(Arts)、倍率を掛ける前に加算 </summary>
		private static readonly decimal AddEnhanced = 150;

		public enum Powerful_e
		{
			/// <summary> 超強力 </summary>
			// 宝具威力に×2
			[EnumText("超強力")] Super = 3,
			/// <summary> とても強力 </summary>
			// 宝具威力に＋200、倍率を掛けた後に加算
			[EnumText("とても強力")] Very = 2,
			/// <summary> かなり強力 </summary>
			// 宝具威力に＋100、倍率を掛けた後に加算
			[EnumText("かなり強力")] Pretty = 1,
			/// <summary> 強力 </summary>
			[EnumText("強力")] Base = 0,
			/// <summary> やや強力 </summary>
			[EnumText("やや強力")] Slightly = -1,
		}

		/// <summary> 威力倍率 </summary>
		[DataMember] public Powerful_e Powerful { get; set; } = Powerful_e.Base;
		/// <summary> 防御無視宝具かどうか </summary>
		[DataMember] public bool IsIgnoredDefense { get; set; } = false;
		/// <summary> 特攻宝具かどうか </summary>
		[DataMember] public EffEnchant SpecialAttack { get; set; } = null;
		/// <summary> 自身のHPが少ないほど威力の高い宝具かどうか </summary>
		[DataMember] public EffImmediate DamageByRemainingHP { get; set; } = null;


		public override bool IsBuff => false;

		public EffAttack() : base(Type_e.NoblePhantasmAttack)
		{
			LvType = LvType_e.NP;
			SetValues(CommandCard_e.Arts, false);
		}

		/// <summary> 数値を設定する </summary>
		public void SetValues(CommandCard_e _commandCard, bool _isEnhanced)
		{
			SetupValuesSize();
			for (int i = 0; i < 5; i++)
			{
				Values[i] = (BasePower[i] + (_isEnhanced ? AddEnhanced : 0));
				switch (_commandCard)
				{
					case CommandCard_e.Quick:
						Values[i] *= 4m / 3;
						break;
					case CommandCard_e.Arts:
						break;
					case CommandCard_e.Buster:
						Values[i] *= 2m / 3;
						break;
					default:
						throw new Exception();
				}
				switch (Powerful)
				{
					case Powerful_e.Super:
						Values[i] *= 2;
						break;
					case Powerful_e.Very:
						Values[i] += 200;
						break;
					case Powerful_e.Pretty:
						Values[i] += 100;
						break;
					default:
						break;
				}
			}
		}

		public override string GetDescriptionText()
		{
			string strRemainingHP = DamageByRemainingHP == null ? "" : "自身のHPが少ないほど威力の高い";
			string strSpecial = SpecialAttack == null ? "" : $"{SpecialAttack.Fillter}特攻";
			string strIgnoredDefence = IsIgnoredDefense ? "防御力無視" : "";
			return $"{strRemainingHP}{Powerful.GetText()}な{strSpecial}{strIgnoredDefence}攻撃";
		}
	}

	/// <summary> 即時効果 </summary>
	[DataContract]
	public class EffImmediate : Effect
	{
		public override bool IsBuff => false;
		public EffImmediate(Type_e _type, DetailType_e _detailType = DetailType_e.None) : base(_type, _detailType) { }

		public override string GetDescriptionText()
		{
			switch (Type)
			{
				case Type_e.IMM_GetStar: return $"スターを獲得";
				case Type_e.IMM_GetNP: return $"NPを増やす";
				case Type_e.IMM_RecoveryHP: return $"HPを回復";
				case Type_e.IMM_CanselBuff:
				case Type_e.IMM_ReleaseDebuff:
					return $"{DetailType.GetText()}{((DetailType == DetailType_e.Attack || DetailType == DetailType_e.Debuff) ? "強化" : "")}状態を解除";
				case Type_e.IMM_AddCharge: return $"チャージを1増やす";
				case Type_e.IMM_DecreaseCharge: return $"チャージを減らす";
				case Type_e.IMM_HasteSkillCharge: return $"スキルチャージを1進める";
				case Type_e.IMM_DelaySkillCharge: return $"スキルターン増加";
				case Type_e.IMM_DamageByRemainingHP: return $"";
				default:
					throw new Exception();
			}
		}
	}
}

