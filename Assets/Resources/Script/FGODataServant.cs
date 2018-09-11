using System;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
using KMUtility.Math;

namespace FGOManager
{
	/// <summary> 性格 </summary>
	[Serializable]
	public class Personality
	{
		public enum Type_e
		{
			[EnumText("善")] Right,
			[EnumText("中庸")] Moderate,
			[EnumText("悪")] Evil,

			[EnumText("その他")] Other,
		}

		/// <summary> 性格種別 </summary>
		public Type_e Type = Type_e.Moderate;
		/// <summary> 性格が「Other」だったときの文字列 </summary>
		public string OtherStr = "";

		public override string ToString() => Type == Type_e.Other ? OtherStr : Type.GetText();
	}

	/// <summary> コマンドカード </summary>
	[Serializable]
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
		[Serializable]
		public class Data
		{
			/// <summary> ヒット数 </summary>
			public byte Hit = 5;

			/// <summary> 対応種類 </summary>
			public CommandCard_e Type { get; private set; }

			public Data(CommandCard_e _type) { Type = _type; }
		}

		[SerializeField]
		private Data m_DataQuick = new Data(CommandCard_e.Quick), m_DataArts = new Data(CommandCard_e.Arts), m_DataBuster = new Data(CommandCard_e.Buster);
		[SerializeField]
		private Data m_DataExtra = new Data(CommandCard_e.Extra), m_DataNoblePhantasm = new Data(CommandCard_e.NoblePhantasm);

		/// <summary> コマンドカード種別 </summary>
		public Type_e Type = Type_e.Q3A1B1;

		/// <summary> コマンドカードデータ取得インデクサ </summary>
		public Data this[CommandCard_e _key]
		{
			get
			{
				switch (_key)
				{
					case CommandCard_e.Quick:
						return m_DataQuick;
					case CommandCard_e.Arts:
						return m_DataArts;
					case CommandCard_e.Buster:
						return m_DataBuster;
					case CommandCard_e.Extra:
						return m_DataExtra;
					case CommandCard_e.NoblePhantasm:
						return m_DataNoblePhantasm;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary> カード枚数取得 </summary>
		/// <param name="_card">カード種別取得</param>
		/// <returns>カード枚数</returns>
		public int GetCardNum(CommandCard_e _card)
		{
			bool isQ = ((int)Type & (int)CommandCard_e.Quick) != 0;
			bool isA = ((int)Type & (int)CommandCard_e.Arts) != 0;
			bool isB = ((int)Type & (int)CommandCard_e.Buster) != 0;
			switch (_card)
			{
				case CommandCard_e.Quick:
					return !isQ ? 1 : (isA || isB) ? 2 : 3;
				case CommandCard_e.Arts:
					return !isA ? 1 : (isQ || isB) ? 2 : 3;
				case CommandCard_e.Buster:
					return !isB ? 1 : (isQ || isA) ? 2 : 3;
				default:
					return 0;
			}
		}
	}

	/// <summary> パラメーター </summary>
	[Serializable]
	public class Parameter
	{
		public enum Type_e
		{
			[EnumText("筋力")] Physics,
			[EnumText("耐久")] Toughness,
			[EnumText("敏捷")] Agility,
			[EnumText("魔力")] Magic,
			[EnumText("幸運")] Luck,
			[EnumText("宝具")] NoblePhantasm,
		}

		/// <summary> ランクペア（表示値と実値） </summary>
		[Serializable]
		public class RankPair
		{
			[SerializeField]
			private Rank m_Display = new Rank();

			/// <summary> ランクペア実値 </summary>
			public Rank Real = new Rank();

			/// <summary> ランクペア表示値 </summary>
			public Rank Display { get { return IsUseDisplay ? m_Display : Real; } }

			public bool IsUseDisplay;
		}

		[SerializeField]
		private RankPair m_RankPhysics = new RankPair(), m_RankToughness = new RankPair(), m_RankAgility = new RankPair();
		[SerializeField]
		private RankPair m_RankMagic = new RankPair(), m_RankLuck = new RankPair(), m_RankNoblePhantasm = new RankPair();

		/// <summary> インデクサ </summary>
		public RankPair this[Type_e _key]
		{
			get
			{
				switch (_key)
				{
					case Type_e.Physics:
						return m_RankPhysics;
					case Type_e.Toughness:
						return m_RankToughness;
					case Type_e.Agility:
						return m_RankAgility;
					case Type_e.Magic:
						return m_RankMagic;
					case Type_e.Luck:
						return m_RankLuck;
					case Type_e.NoblePhantasm:
						return m_RankNoblePhantasm;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary> DR補正値 </summary>
		public decimal CorrectionDR => 0.7m - (decimal)this[Type_e.Magic].Real.Type / 10m - this[Type_e.Magic].Real.Plus * 0.025m;


		/// <summary> パラメーター補正値を取得する </summary>
		/// <param name="_type">パラメーター種類</param>
		/// <returns>パラメーター補正値</returns>
		public decimal GetCorrection(Type_e _type)
		{
			Rank rank = this[_type].Real;
			if (rank.Type == Rank.Type_e.Other) return 0;
			decimal cor = (decimal)rank.Type / 100 + rank.Plus * 0.0025m;
			if (rank.Type >= Rank.Type_e.B) cor *= 2;
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
	}

	/// <summary> 宝具 </summary>
	[Serializable]
	public class NoblePhantasm
	{
		/// <summary> 宝具名称 </summary>
		public string Name = "";
		/// <summary> 宝具名称ふりがな </summary>
		public string Phonetic = "";

		/// <summary> 宝具種別 </summary>
		public string Kind = "";

		/// <summary> 宝具色 </summary>
		public CommandCard_e CardType = CommandCard_e.Buster;

		/// <summary> 宝具ランク </summary>
		public Rank Rank = new Rank();

		/// <summary> 強化前効果 </summary>
		public List<Effect> BaseEffects = new List<Effect>();
		/// <summary> 強化後効果 </summary>
		public List<Effect> EnhancedEffects = new List<Effect>();

		/// <summary> ヒット数取得Func </summary>
		public Func<byte> GetHit { get; set; } = null;

		/// <summary> ヒット数 </summary>
		public byte HitNum { get { return GetHit == null ? (byte)0 : GetHit(); } }


		/// <summary> 強化状態に応じた効果を取得する </summary>
		/// <param name="_isEnhanced">強化されているかどうか</param>
		/// <returns>宝具効果</returns>
		public List<Effect> GetEffects(bool _isEnhanced) => !_isEnhanced ? BaseEffects : EnhancedEffects;
	}

	/// <summary> スキル </summary>
	[Serializable]
	public class Skill
	{
		/// <summary> スキル名称 </summary>
		public string Name = "";

		/// <summary> スキルランク </summary>
		public Rank Rank = new Rank();

		/// <summary> CT </summary>
		public byte CT = 7;

		/// <summary> 強化前効果 </summary>
		public List<Effect> BaseEffects = new List<Effect>();
		/// <summary> 強化後効果 </summary>
		public List<Effect> EnhancedEffects = new List<Effect>();


		/// <summary> 強化状態に応じた効果を取得する </summary>
		/// <param name="_isEnhanced">強化されているかどうか</param>
		/// <returns>スキル効果</returns>
		public List<Effect> GetEffects(bool _isEnhanced) => !_isEnhanced ? BaseEffects : EnhancedEffects;
	}

	/// <summary> クラススキル </summary>
	[Serializable]
	public class ClassSkill
	{
		/// <summary> スキル名称 </summary>
		public string Name = "";

		/// <summary> スキルランク </summary>
		public Rank Rank = new Rank();

		/// <summary> 効果 </summary>
		public List<Effect> Effects = new List<Effect>();
	}

	/// <summary> サーヴァント基本データクラス </summary>
	[Serializable]
	public class ServantBase
	{
		#region Data
		/// <summary> サーヴァントNo. </summary>
		public int No = 0;

		/// <summary> サーヴァント名称 </summary>
		public string Name = "";

		/// <summary> クラス </summary>
		public Class_e Class = Class_e.Saber;

		/// <summary> レア度(☆0～5) </summary>
		public byte Rare = 5;

		/// <summary> 初期ATK </summary>
		public int FirstATK = 0;
		/// <summary> 最終ATK </summary>
		public int MaxATK = 0;

		/// <summary> コマンドカード </summary>
		public CommandCard CommandCard = new CommandCard();

		/// <summary> N/A(攻撃時のNP上昇基礎値) </summary>
		public decimal NA = 0;
		/// <summary> N/D(攻撃を受けた際のNP上昇基礎値) </summary>
		public decimal ND = 0;

		/// <summary> SR計算が初期補正かどうか </summary>
		public bool IsInitialSR = false;
		/// <summary> ATK算出が魔術タイプかどうか </summary>
		public bool IsMagicalType = false;
		// キャスターのうち、メフィストフェレス、エリザベート〔ハロウィン〕、バベッジ、エジソン、ジェロニモ、ジークを除くすべて
		// マリー（騎）、マルタ（騎）、ステンノ、マタ・ハリ、カーミラ、清姫（狂）、ニコラ・テスラ、アルジュナ、オジマンディアス、イシュタル（弓）、BB（SR）、殺生院キアラ、ホームズ、ニトクリス（殺）、刑部姫、アビゲイル、葛飾北斎、セミラミス、浅上藤乃、イヴァン雷帝、BB（SSR）

		/// <summary> 天地人相性 </summary>
		public Attributes_e Attributes = Attributes_e.Man;

		/// <summary> 方針 </summary>
		public Policy_e Policy = Policy_e.Neutral;

		/// <summary> 性格 </summary>
		public Personality Personality = new Personality();

		/// <summary> 性別 </summary>
		public Sex_e Sex = Sex_e.Female;

		/// <summary> 成長タイプ </summary>
		public StatusTrend_e StatusTrend = StatusTrend_e.ATK;

		/// <summary> パラメーター </summary>
		public Parameter Parameter = new Parameter();
		//		巌窟王の幸運はB～A++相当（正確なランクは不明）
		//		エジソンの耐久・魔力EXはE相当
		//		エルキドゥのパラメーター？はB相当
		//		“山の翁”のパラメーターは表記よりも1ランクずつ高い（筋力A耐久EX敏捷A魔力D幸運D相当）

		/// <summary> 特性 </summary>
		public List<string> Characteristic = new List<string>();

		/// <summary> 宝具データ </summary>
		public NoblePhantasm NoblePhantasm = new NoblePhantasm();

		/// <summary> 保有スキルデータ </summary>
		public Skill[] Skills = new Skill[3];

		/// <summary> クラススキルデータ </summary>
		public List<ClassSkill> ClassSkills = new List<ClassSkill>();

		/// <summary> 霊基再臨素材 </summary>
		public MaterialNumber[] SecondComingMaterials = new MaterialNumber[4];
		/// <summary> スキル育成素材 </summary>
		public MaterialNumber[] SkillMaterials = new MaterialNumber[9];

		/// <summary> 絆礼装 </summary>
		public ConceptDress BondDress = new ConceptDress();

		/// <summary> イラストレーター </summary>
		public string Illustrator = "";
		/// <summary> 声優 </summary>
		public string CV = "";

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
				int artsCardNum = CommandCard.GetCardNum(CommandCard_e.Arts);
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
