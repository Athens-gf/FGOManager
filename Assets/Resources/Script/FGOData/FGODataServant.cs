using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KMUtility;
using KMUtility.Math;
using System.Runtime.Serialization;

namespace FGOManager
{
	#region 要素
	/// <summary> 方針 </summary>
	public class Policy
	{
		private Type_e m_Type = Type_e.Neutral;
		private string m_OtherStr = "";

		public enum Type_e
		{
			[EnumText("秩序")] Law = 0,
			[EnumText("中立")] Neutral = 1,
			[EnumText("混沌")] Chaos = 2,

			[EnumText("その他")] Other = 3,
		}

		/// <summary> 方針種別 </summary>
		public Type_e Type
		{
			get { return m_Type; }
			set
			{
				m_Type = value;
				if (value != Type_e.Other) m_OtherStr = "";
			}
		}
		/// <summary> 方針が「Other」だったときの文字列 </summary>
		public string OtherStr
		{
			get { return m_OtherStr; }
			set
			{
				m_OtherStr = value;
				if (value != "") m_Type = Type_e.Other;
			}
		}
		public override string ToString() => Type == Type_e.Other ? OtherStr : Type.GetText();
	}

	/// <summary> 性格 </summary>
	public class Personality
	{
		private Type_e m_Type = Type_e.Moderate;
		private string m_OtherStr = "";

		public enum Type_e
		{
			[EnumText("善")] Right = 0,
			[EnumText("中庸")] Moderate = 1,
			[EnumText("悪")] Evil = 2,

			[EnumText("その他")] Other = 3,
		}

		/// <summary> 性格種別 </summary>
		public Type_e Type
		{
			get { return m_Type; }
			set
			{
				m_Type = value;
				if (value != Type_e.Other) m_OtherStr = "";
			}
		}
		/// <summary> 性格が「Other」だったときの文字列 </summary>
		public string OtherStr
		{
			get { return m_OtherStr; }
			set
			{
				m_OtherStr = value;
				if (value != "") m_Type = Type_e.Other;
			}
		}
		public override string ToString() => Type == Type_e.Other ? OtherStr : Type.GetText();
	}

	/// <summary> コマンドカード </summary>
	[DataContract]
	public class CommandCard
	{
		public enum Type_e
		{
			Q3A1B1 = CommandCard_e.Quick,
			Q1A3B1 = CommandCard_e.Arts,
			Q1A1B3 = CommandCard_e.Buster,
			Q2A2B1 = CommandCard_e.Quick | CommandCard_e.Arts,
			Q2A1B2 = CommandCard_e.Quick | CommandCard_e.Buster,
			Q1A2B2 = CommandCard_e.Arts | CommandCard_e.Buster,
		}

		/// <summary> コマンドカードデータ構造体 </summary>
		[DataContract]
		public class Data
		{
			[DataMember] private sbyte m_Hit = 5;

			/// <summary> 対応種類 </summary>
			[DataMember] public CommandCard_e Type { get; private set; }

			/// <summary> ヒット数 </summary>
			public sbyte Hit
			{
				get { return m_Hit; }
				set { if (m_Hit > 0) m_Hit = value; }
			}

			public Data(CommandCard_e _type) { Type = _type; }

			public override string ToString() => $"{Type.ToString()} Hit;{Hit}";
		}

		[DataMember] private Dictionary<CommandCard_e, Data> m_Data = null;

		/// <summary> コマンドカード種別 </summary>
		[DataMember] public Type_e Type { get; set; } = Type_e.Q3A1B1;

		/// <summary> コマンドカードデータ取得インデクサ </summary>
		public Data this[CommandCard_e _key] { get { return m_Data[_key]; } }

		public CommandCard()
		{
			m_Data = ExEnum.GetEnumIter<CommandCard_e>().ToDictionary(cc => cc, cc => new Data(cc));
		}

		public override string ToString()
			=> $"{Type.ToString()}, {this[CommandCard_e.Quick]} {this[CommandCard_e.Arts]} {this[CommandCard_e.Buster]} {this[CommandCard_e.Extra]} {this[CommandCard_e.NoblePhantasm]}";
	}

	[Serializable] public class CommandCardEvent : UnityEvent<CommandCard> { }

	/// <summary> パラメーター </summary>
	[DataContract]
	public class Parameter
	{
		public enum Type_e
		{
			[EnumText("筋力")] Physics = 0,
			[EnumText("耐久")] Toughness = 1,
			[EnumText("敏捷")] Agility = 2,
			[EnumText("魔力")] Magic = 3,
			[EnumText("幸運")] Luck = 4,
			[EnumText("宝具")] NoblePhantasm = 5,
		}

		[DataMember] private Dictionary<Type_e, Rank> m_Ranklist = null;

		/// <summary> インデクサ </summary>
		public Rank this[Type_e _key] { get { return m_Ranklist[_key]; } set { m_Ranklist[_key] = value; } }

		/// <summary> DR補正値 </summary>
		public decimal CorrectionDR => 0.7m - (decimal)this[Type_e.Magic].Type / 10m - this[Type_e.Magic].Plus * 0.025m;

		public Parameter()
		{
			m_Ranklist = ExEnum.GetEnumIter<Type_e>().ToDictionary(t => t, _ => new Rank());
		}

		public override string ToString() => m_Ranklist.Select(rl => $"{rl.Key.GetText()}:{rl.Value}").Aggregate((s0, s1) => $"{s0} {s1}");

		/// <summary> パラメーター補正値を取得する </summary>
		/// <param name="_type">パラメーター種類</param>
		/// <returns>パラメーター補正値</returns>
		public decimal GetCorrection(Type_e _type)
		{
			decimal cor = (decimal)this[_type].Type / 100 + this[_type].Plus * 0.0025m;
			if (this[_type].Type >= Rank.Type_e.B) cor *= 2;
			return 1 + cor;
		}

		/// <summary> SRのパラメーター補正値を取得する </summary>
		/// <param name="_isInitialSR">SRは初期補正かどうか</param>
		/// <returns>SRのパラメーター補正値</returns>
		public decimal GetCorrectionInitialSR(bool _isInitialSR)
		{
			var cor = GetCorrection(Type_e.Agility) - 1;
			if (_isInitialSR) cor /= 2;
			return 1 + cor;
		}

		public List<string> Tags => ExEnum.GetEnumIter<Type_e>().Select(t => $"{t.GetText()}:{this[t]}").ToList();
	}

	/// <summary> 宝具 </summary>
	[DataContract]
	public class NoblePhantasm
	{
		[DataMember] private CommandCard_e m_CardType = CommandCard_e.Buster;

		/// <summary> 宝具名称 </summary>
		[DataMember] public string Name { get; set; } = "";
		/// <summary> 宝具名称ふりがな </summary>
		[DataMember] public string Phonetic { get; set; } = "";

		/// <summary> 宝具種別 </summary>
		[DataMember] public string Kind { get; set; } = "";

		/// <summary> 宝具色 </summary>
		public CommandCard_e CardType
		{
			get { return m_CardType; }
			set { if (value <= CommandCard_e.Buster) m_CardType = value; }
		}
		/// <summary> 宝具ランク </summary>
		[DataMember] public Rank Rank { get; private set; } = new Rank();

		/// <summary> 強化前効果 </summary>
		[DataMember] public List<Effect> BaseEffects { get; private set; } = new List<Effect>();
		/// <summary> 強化後効果 </summary>
		[DataMember] public List<Effect> EnhancedEffects { get; private set; } = new List<Effect>();

		/// <summary> ヒット数取得Func </summary>
		[IgnoreDataMember] public Func<sbyte> GetHit { get; set; } = null;

		/// <summary> ヒット数 </summary>
		public sbyte HitNum { get { return GetHit == null ? (sbyte)0 : GetHit(); } }

		public override string ToString() => Name;

		/// <summary> 強化状態に応じた効果を取得する </summary>
		/// <param name="_isEnhanced">強化されているかどうか</param>
		/// <returns>宝具効果</returns>
		public List<Effect> GetEffects(bool _isEnhanced) => !_isEnhanced ? BaseEffects : EnhancedEffects;
	}

	/// <summary> スキル </summary>
	[DataContract]
	public class Skill
	{
		[DataMember] private byte m_CT = 7;

		/// <summary> スキル名称 </summary>
		[DataMember] public string Name { get; set; } = "";
		/// <summary> スキル名称ふりがな </summary>
		[DataMember] public string Phonetic { get; set; } = "";

		/// <summary> スキルランク </summary>
		[DataMember] public Rank Rank { get; private set; } = new Rank();

		/// <summary> CT </summary>
		public byte CT
		{
			get { return m_CT; }
			set { if (value > 3) m_CT = value; }
		}
		/// <summary> 強化前効果 </summary>
		[DataMember] public List<Effect> BaseEffects { get; private set; } = new List<Effect>();
		/// <summary> 強化後効果 </summary>
		[DataMember] public List<Effect> EnhancedEffects { get; private set; } = new List<Effect>();

		public override string ToString() => Name;


		/// <summary> 強化状態に応じた効果を取得する </summary>
		/// <param name="_isEnhanced">強化されているかどうか</param>
		/// <returns>スキル効果</returns>
		public List<Effect> GetEffects(bool _isEnhanced) => !_isEnhanced ? BaseEffects : EnhancedEffects;
	}

	/// <summary> クラススキル </summary>
	[DataContract]
	public class ClassSkill
	{
		/// <summary> スキル名称 </summary>
		[DataMember] public string Name { get; set; } = "";
		/// <summary> スキル名称ふりがな </summary>
		[DataMember] public string Phonetic { get; set; } = "";

		/// <summary> スキルランク </summary>
		[DataMember] public Rank Rank { get; private set; } = new Rank();

		/// <summary> 効果 </summary>
		[DataMember] public List<Effect> Effects { get; private set; } = new List<Effect>();
	}
	#endregion

	[Serializable] public class ServantBaseEvent : UnityEvent<ServantBase> { }
	/// <summary> サーヴァント基本データクラス </summary>
	[DataContract]
	public class ServantBase : ICharacter
	{
		#region Data
		[DataMember] private int m_No = 0;
		[DataMember] private int m_Rare = 5;
		[DataMember] private decimal m_NA = 0;
		[DataMember] private decimal m_ND = 3;

		/// <summary> サーヴァントNo. </summary>
		public int No { get { return m_No; } set { if (value > 0) m_No = value; } }
		/// <summary> 名称 </summary>
		[DataMember] public string Name { get; set; } = "";
		/// <summary> クラス </summary>
		[DataMember] public Class_e Class { get; set; } = Class_e.Saber;
		/// <summary> レア度(☆0～5) </summary>
		public int Rare { get { return m_Rare; } set { if (value >= 0 && value <= 5) m_Rare = value; } }
		/// <summary> 初期ATK </summary>
		[DataMember] public int FirstATK { get; set; } = 0;
		/// <summary> 最終ATK </summary>
		[DataMember] public int MaxATK { get; set; } = 0;
		/// <summary> コマンドカード </summary>
		[DataMember] public CommandCard CommandCard { get; set; } = new CommandCard();
		/// <summary> N/A(攻撃時のNP上昇基礎値) </summary>
		public decimal NA { get { return m_NA; } set { if (value > 0) m_NA = value; } }
		/// <summary> N/D(攻撃を受けた際のNP上昇基礎値) </summary>
		public decimal ND { get { return m_ND; } set { if (value > 0) m_ND = value; } }
		/// <summary> SR計算が初期補正かどうか </summary>
		[DataMember] public bool IsInitialSR { get; set; } = false;
		/// <summary> ATK算出が魔術タイプかどうか </summary>
		[DataMember] public bool IsMagicalType { get; set; } = false;
		// キャスターのうち、メフィストフェレス、エリザベート〔ハロウィン〕、バベッジ、エジソン、ジェロニモ、ジークを除くすべて
		// マリー（騎）、マルタ（騎）、ステンノ、マタ・ハリ、カーミラ、清姫（狂）、ニコラ・テスラ、アルジュナ、オジマンディアス、イシュタル（弓）、BB（SR）、殺生院キアラ、ホームズ、ニトクリス（殺）、刑部姫、アビゲイル、葛飾北斎、セミラミス、浅上藤乃、イヴァン雷帝、BB（SSR）

		/// <summary> 天地人相性 </summary>
		[DataMember] public Attribute_e Attribute { get; set; } = Attribute_e.Man;

		/// <summary> 方針 </summary>
		[DataMember] public Policy Policy { get; set; } = new Policy();

		/// <summary> 性格 </summary>
		[DataMember] public Personality Personality { get; set; } = new Personality();

		/// <summary> 性別 </summary>
		[DataMember] public Sex_e Sex { get; set; } = Sex_e.Female;

		/// <summary> 成長タイプ </summary>
		[DataMember] public StatusTrend_e StatusTrend { get; set; } = StatusTrend_e.ATK;

		/// <summary> パラメーター </summary>
		[DataMember] public Parameter Parameter = new Parameter();

		//		巌窟王の幸運はB～A++相当（正確なランクは不明）
		//		エジソンの耐久・魔力EXはE相当
		//		エルキドゥのパラメーター？はB相当
		//		“山の翁”のパラメーターは表記よりも1ランクずつ高い（筋力A耐久EX敏捷A魔力D幸運D相当）

		/// <summary> 特性 </summary>
		[DataMember] public List<string> Characteristic { get; private set; } = new List<string> { "人型", "エヌマ・エリシュ" };

		/// <summary> 宝具データ </summary>
		[DataMember] public NoblePhantasm NoblePhantasm { get; private set; } = new NoblePhantasm();

		/// <summary> 保有スキルデータ </summary>
		[DataMember] public Skill[] Skills { get; private set; } = new Skill[3];

		/// <summary> クラススキルデータ </summary>
		[DataMember] public List<ClassSkill> ClassSkills { get; private set; } = new List<ClassSkill>();

		/// <summary> 霊基再臨素材 </summary>
		[DataMember] public MaterialNumber[] SecondComingMaterials { get; private set; } = new MaterialNumber[4];
		/// <summary> スキル育成素材 </summary>
		[DataMember] public MaterialNumber[] SkillMaterials { get; private set; } = new MaterialNumber[9];

		/// <summary> 絆礼装 </summary>
		[DataMember] public ConceptDress BondDress { get; private set; } = new ConceptDress();

		/// <summary> イラストレーター </summary>
		[DataMember] public string Illustrator { get; set; } = "";
		/// <summary> 声優 </summary>
		[DataMember] public string CV { get; set; } = "";

		#endregion

		#region Property
		/// <summary> 編成コスト </summary>
		public byte Cost
		{
			get
			{
				if (Class == Class_e.Shielder) return 0;
				if (Rare == 5) return 16;
				if (Rare == 4) return 12;
				if (Rare == 3) return 7;
				if (Rare == 2 || Rare == 0) return 4;
				if (Rare == 1) return 3;
				return 100;
			}
		}

		/// <summary> 聖杯転輪しない場合の最大成長レベル </summary>
		public byte MaxLevel
		{
			get
			{
				if (Rare == 5) return 90;
				if (Rare == 4 || Class == Class_e.Shielder) return 80;
				if (Rare == 3) return 70;
				if (Rare == 2 || Rare == 0) return 65;
				if (Rare == 1) return 60;
				return 0;
			}
		}

		/// <summary> 基本算出N/A(攻撃時のNP上昇基礎値) </summary>
		public decimal BaseNA
		{
			get
			{
				decimal baseByClass;
				switch (Class)
				{
					case Class_e.Berserker:
						baseByClass = 1.4m; break;
					case Class_e.Lancer:
					case Class_e.Assassin:
					case Class_e.Avenger:
						baseByClass = 1.45m; break;
					case Class_e.Saber:
					case Class_e.Shielder:
					case Class_e.Ruler:
					case Class_e.Foreigner:
						baseByClass = 1.5m; break;
					case Class_e.Archer:
					case Class_e.Rider:
					case Class_e.Alterego:
						baseByClass = 1.55m; break;
					case Class_e.Caster:
					case Class_e.MoonCancer:
						baseByClass = 1.6m; break;
					default: baseByClass = 0; break;
				}
				decimal arts = 0;
				int artsCardNum = CommandCard.Type.GetCount(CommandCard_e.Arts);
				if (artsCardNum == 1) arts = 1.5m;
				else if (artsCardNum == 2) arts = 1.125m;
				else if (artsCardNum == 3) arts = 1.0m;
				return KMMath.ToRoundDown(baseByClass * arts * Parameter.GetCorrection(Parameter.Type_e.Magic) / CommandCard[CommandCard_e.Arts].Hit, 2);
			}
		}

		/// <summary> スター発生率の基本値 </summary>
		public decimal SR
		{
			get
			{
				decimal baseByClass;
				switch (Class)
				{
					case Class_e.Saber:
						baseByClass = 10; break;
					case Class_e.Archer:
						baseByClass = 8; break;
					case Class_e.Lancer:
						baseByClass = 12; break;
					case Class_e.Rider:
						baseByClass = 9; break;
					case Class_e.Caster:
						baseByClass = 11; break;
					case Class_e.Assassin:
						baseByClass = 25; break;
					case Class_e.Berserker:
						baseByClass = 5; break;
					case Class_e.Shielder:
					case Class_e.Ruler:
					case Class_e.Alterego:
						baseByClass = 10; break;
					case Class_e.Avenger:
						baseByClass = 6; break;
					case Class_e.MoonCancer:
					case Class_e.Foreigner:
						baseByClass = 15; break;
					default: baseByClass = 0; break;
				}
				return KMMath.ToRoundDown(baseByClass * Parameter.GetCorrectionInitialSR(IsInitialSR), 1);
			}
		}

		/// <summary> スター集中度 </summary>
		public int SW
		{
			get
			{
				int baseByClass;
				switch (Class)
				{
					case Class_e.Berserker:
						baseByClass = 10; break;
					case Class_e.Avenger:
						baseByClass = 30; break;
					case Class_e.Caster:
					case Class_e.MoonCancer:
						baseByClass = 50; break;
					case Class_e.Lancer:
						baseByClass = 90; break;
					case Class_e.Saber:
					case Class_e.Assassin:
					case Class_e.Shielder:
					case Class_e.Ruler:
					case Class_e.Alterego:
						baseByClass = 100; break;
					case Class_e.Archer:
					case Class_e.Foreigner:
						baseByClass = 150; break;
					case Class_e.Rider:
						baseByClass = 200; break;
					default: baseByClass = 0; break;
				}
				return (int)(baseByClass * Parameter.GetCorrection(Parameter.Type_e.Luck));
			}
		}

		/// <summary> 即死攻撃を受けた時の補正値 数値が低いほど死に難い </summary>
		public decimal DR
		{
			get
			{
				int baseByClass;
				switch (Class)
				{
					case Class_e.MoonCancer:
						baseByClass = 1; break;
					case Class_e.Avenger:
					case Class_e.Foreigner:
						baseByClass = 10; break;
					case Class_e.Saber:
					case Class_e.Shielder:
					case Class_e.Ruler:
						baseByClass = 35; break;
					case Class_e.Lancer:
						baseByClass = 40; break;
					case Class_e.Archer:
						baseByClass = 45; break;
					case Class_e.Rider:
					case Class_e.Alterego:
						baseByClass = 50; break;
					case Class_e.Assassin:
						baseByClass = 55; break;
					case Class_e.Caster:
						baseByClass = 60; break;
					case Class_e.Berserker:
						baseByClass = 65; break;
					default: baseByClass = 0; break;
				}
				return KMMath.ToRoundDown(baseByClass * Parameter.CorrectionDR, 1);
			}
		}

		/// <summary> HP計算補正値 </summary>
		private decimal CorHP
		{
			get
			{
				decimal baseByClass;
				switch (Class)
				{
					case Class_e.Avenger:
						baseByClass = 0.88m; break;
					case Class_e.Berserker:
						baseByClass = 0.9m; break;
					case Class_e.Assassin:
					case Class_e.Alterego:
						baseByClass = 0.95m; break;
					case Class_e.Rider:
						baseByClass = 0.96m; break;
					case Class_e.Archer:
					case Class_e.Caster:
						baseByClass = 0.98m; break;
					case Class_e.Ruler:
					case Class_e.Foreigner:
						baseByClass = 1; break;
					case Class_e.Saber:
					case Class_e.Shielder:
						baseByClass = 1.01m; break;
					case Class_e.Lancer:
						baseByClass = 1.02m; break;
					case Class_e.MoonCancer:
						baseByClass = 1.05m; break;
					default: baseByClass = 0; break;
				}
				decimal trend = StatusTrend == StatusTrend_e.Altria_Lily ? 0.85m : 1 + (decimal)StatusTrend * 0.05m;
				return baseByClass * trend * Parameter.GetCorrection(Parameter.Type_e.Toughness);
			}
		}

		/// <summary> 最大HP </summary>
		public int MaxHP
		{
			get
			{
				int baseHP = 0;
				if (Rare == 1) baseHP = 7500;
				else if (Rare == 2 || Rare == 0) baseHP = 8500;
				else if (Rare == 3) baseHP = 10000;
				else if (Rare == 4) baseHP = 12500;
				else if (Rare == 5) baseHP = 15000;
				return (int)(baseHP * CorHP);
			}
		}

		/// <summary> 初期HP </summary>
		public int FirstHP
		{
			get
			{
				int baseHP = 0;
				if (Rare == 1) baseHP = 1500;
				else if (Rare == 2 || Rare == 0) baseHP = 1600;
				else if (Rare == 3) baseHP = 1800;
				else if (Rare == 4) baseHP = 2000;
				else if (Rare == 5) baseHP = 2200;
				return (int)(baseHP * CorHP);
			}
		}

		/// <summary> ATK計算補正値 </summary>
		private decimal CorAtk
		{
			get
			{
				decimal baseByClass;
				switch (Class)
				{
					case Class_e.Caster:
					case Class_e.MoonCancer:
						baseByClass = 0.94m; break;
					case Class_e.Ruler:
						baseByClass = 0.95m; break;
					case Class_e.Assassin:
						baseByClass = 0.96m; break;
					case Class_e.Rider:
						baseByClass = 0.97m; break;
					case Class_e.Lancer:
						baseByClass = 0.98m; break;
					case Class_e.Shielder:
						baseByClass = 0.99m; break;
					case Class_e.Foreigner:
						baseByClass = 1; break;
					case Class_e.Saber:
						baseByClass = 1.01m; break;
					case Class_e.Archer:
					case Class_e.Alterego:
						baseByClass = 1.02m; break;
					case Class_e.Berserker:
						baseByClass = 1.03m; break;
					case Class_e.Avenger:
						baseByClass = 1.05m; break;
					default: baseByClass = 0; break;
				}
				decimal trend = StatusTrend == StatusTrend_e.Altria_Lily ? 0.85m : 1 - (int)StatusTrend * 0.05m;
				decimal corMag = Parameter.GetCorrection(Parameter.Type_e.Magic);
				if (IsMagicalType)
					return baseByClass * trend * corMag;
				decimal corAgiMag = (Parameter.GetCorrection(Parameter.Type_e.Agility) + corMag) / 2;
				decimal corPhy = Parameter.GetCorrection(Parameter.Type_e.Physics);
				return baseByClass * trend * corPhy * corAgiMag * (1 - (corPhy - 1) * (corAgiMag - 1));
			}
		}

		/// <summary> 基本算出最大ATK </summary>
		public int BaseMaxATK
		{
			get
			{
				int baseATK = 0;
				if (Rare == 1) baseATK = 5500;
				else if (Rare == 2 || Rare == 0) baseATK = 6200;
				else if (Rare == 3) baseATK = 7000;
				else if (Rare == 4) baseATK = 9000;
				else if (Rare == 5) baseATK = 11000;
				return (int)(baseATK * CorAtk);
			}
		}

		/// <summary> 基本算出初期ATK </summary>
		public int BaseFirstATK
		{
			get
			{
				int baseATK = 0;
				if (Rare == 1) baseATK = 1000;
				else if (Rare == 2 || Rare == 0) baseATK = 1100;
				else if (Rare == 3) baseATK = 1300;
				else if (Rare == 4) baseATK = 1500;
				else if (Rare == 5) baseATK = 1700;
				return (int)(baseATK * CorAtk);
			}
		}

		public bool IsServant => true;

		public virtual List<string> Tags => Characteristic
			.AddRetern(Name)
			.AddRetern(Class.GetText())
			.AddRetern(Attribute.GetText())
			.AddRetern(Policy.ToString())
			.AddRetern(Personality.ToString())
			.AddRetern(Sex.GetText())
			.AddRetern(Parameter.Tags)
			.ToList();



		#endregion

		public ServantBase()
		{
			NoblePhantasm.GetHit = () => CommandCard[CommandCard_e.NoblePhantasm].Hit;
			for (int i = 0; i < SecondComingMaterials.Length; i++)
				SecondComingMaterials[i] = new MaterialNumber();
			for (int i = 0; i < SkillMaterials.Length; i++)
				SkillMaterials[i] = new MaterialNumber();
			SkillMaterials[8][Material_e.TraditionalCrystal] = 1;
		}

		#region Method
		/// <summary> 聖杯転輪の現在のレベルによる補正値を取得する </summary>
		/// <param name="_level">レベル</param>
		/// <returns>聖杯転臨の補正値</returns>
		private decimal GetCorHolyGrail(int _level) => KMMath.ToRoundDown((decimal)(_level - 1) / (MaxLevel - 1), 3);

		/// <summary> 聖杯転臨後のHPを取得する </summary>
		/// <param name="_level">レベル</param>
		/// <returns>聖杯転臨後のHP</returns>
		public int GetHolyGrailHP(int _level) => FirstHP + (int)((MaxHP - FirstHP) * GetCorHolyGrail(_level));

		/// <summary> 聖杯転臨後のATKを取得する </summary>
		/// <param name="_level">レベル</param>
		/// <returns>聖杯転臨後のATK</returns>
		public int GetHolyGrailATK(int _level) => FirstATK + (int)((MaxATK - FirstATK) * GetCorHolyGrail(_level));
		#endregion
	}

}
