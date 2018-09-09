using System;
using KMUtility;

namespace FGOManager
{
	#region Enum
	/// <summary> クラス列挙体 </summary>
	public enum Class_e
	{
		[EnumText("セイバー")]
		Saber = 0,
		[EnumText("アーチャー")]
		Archer = 1,
		[EnumText("ランサー")]
		Lancer = 2,
		[EnumText("ライダー")]
		Rider = 3,
		[EnumText("キャスター")]
		Caster = 4,
		[EnumText("アサシン")]
		Assassin = 5,
		[EnumText("バーサーカー")]
		Berserker = 6,
		[EnumText("シールダー")]
		Shielder = 7,
		[EnumText("ルーラー")]
		Ruler = 8,
		[EnumText("アヴェンジャー")]
		Avenger = 9,
		[EnumText("ムーンキャンサー")]
		MoonCancer = 10,
		[EnumText("アルターエゴ")]
		Alterego = 11,
		[EnumText("フォーリナー")]
		Foreigner = 12,

		[EnumText("ビーストⅠ")]
		Beast1 = 13,
		[EnumText("ビーストⅡ")]
		Beast2 = 14,
		[EnumText("ビーストⅢ")]
		Beast3 = 15,
	}

	/// <summary> コマンドカード列挙体 </summary>
	public enum CommandCard_e
	{
		[EnumText("Q")] Quick = 1,
		[EnumText("A")] Arts = 1 << 1,
		[EnumText("B")] Buster = 1 << 2,

		[EnumText("Ex")] Extra = 1 << 3,
		[EnumText("宝具")] NoblePhantasm = 1 << 4,
	}

	/// <summary> 天地人相性列挙体 </summary>
	public enum Attributes_e
	{
		[EnumText("天")] Heaven = 0,
		[EnumText("地")] Earth = 1,
		[EnumText("人")] Man = 2,
		[EnumText("星")] Star = 3,
		[EnumText("獣")] Beast = 4,
	}

	/// <summary> 性別列挙体 </summary>
	public enum Sex_e
	{
		[EnumText("男性")] Male,
		[EnumText("女性")] Female,
		[EnumText("？")] Other,
	}

	/// <summary> 方針列挙体 </summary>
	public enum Policy_e
	{
		[EnumText("秩序")] Law,
		[EnumText("中立")] Neutral,
		[EnumText("混沌")] Chaos,
	}

	/// <summary> 成長タイプ列挙体 </summary>
	public enum StatusTrend_e
	{
		[EnumText("アルトリア〔リリィ〕")] Altria_Lily = 10,
		[EnumText("HP偏重")] HP_Heavy = 2,
		[EnumText("HP寄り")] HP = 1,
		[EnumText("バランス")] Balance = 0,
		[EnumText("ATK寄り")] ATK = -1,
		[EnumText("ATK偏重")] ATK_Heavy = -2,
	}

	#endregion

	/// <summary> ランク管理クラス </summary>
	[Serializable]
	public class Rank
	{
		public enum Type_e
		{
			EX = 2,
			A = 1,
			B = 0,
			C = -1,
			D = -2,
			E = -3,
			Other = -100,
		}

		/// <summary>  </summary>
		public Type_e Type = Type_e.B;
		/// <summary>  </summary>
		public sbyte Plus = 0;
		/// <summary>  </summary>
		public string Other = "";

		public override string ToString()
		{
			if (Type == Type_e.Other) return Other;
			string s = Type.GetText();
			if (Plus == 2) s += "++";
			else if (Plus == 1) s += "+";
			else if (Plus == -1) s += "-";
			else if (Plus != 0) s = "";
			return s;
		}
	}

	/// <summary> 素材構造体 </summary>
	[Serializable]
	public struct Material
	{
		public enum Type_e
		{
			// 銅素材
			[EnumText("英雄の証")] C_Proof = 0,
			[EnumText("凶骨")] C_Bone = 1,
			[EnumText("竜の牙")] C_Fang = 2,
			[EnumText("虚影の塵")] C_Dust = 3,
			[EnumText("愚者の鎖")] C_Chain = 4,
			[EnumText("万死の毒針")] C_PoisonNeedle = 5,
			[EnumText("魔術髄液")] C_CerebrospinalFluid = 6,
			[EnumText("宵哭きの鉄杭")] C_IronPile = 7,
			[EnumText("励振火薬")] C_Explosive = 8,

			// 銀素材
			[EnumText("世界樹の種")] S_Seed = 100,
			[EnumText("ゴーストランタン")] S_Lantern = 101,
			[EnumText("八連双晶")] S_Crystal = 102,
			[EnumText("蛇の宝玉")] S_Orb = 103,
			[EnumText("鳳凰の羽根")] S_Feather = 104,
			[EnumText("無間の歯車")] S_Gear = 105,
			[EnumText("禁断の頁")] S_Page = 106,
			[EnumText("ホムンクルスベビー")] S_HomunculusBaby = 107,
			[EnumText("隕蹄鉄")] S_Horseshoe = 108,
			[EnumText("大騎士勲章")] S_Medal = 109,
			[EnumText("追憶の貝殻")] S_Shell = 110,
			[EnumText("枯淡勾玉")] S_Jewel = 111,
			[EnumText("永遠結氷")] S_Ice = 112,
			[EnumText("巨人の指輪")] S_Ring = 113,
			[EnumText("オーロラ鋼")] S_AuroraSteel = 114,

			// 金素材
			[EnumText("混沌の爪")] G_Claw = 200,
			[EnumText("蛮神の心臓")] G_Heart = 201,
			[EnumText("竜の逆鱗")] G_Scale = 202,
			[EnumText("血の涙石")] G_TearDrop = 203,
			[EnumText("黒獣脂")] G_Tallow = 204,
			[EnumText("精霊根")] G_Root = 205,
			[EnumText("戦馬の幼角")] G_Horn = 206,
			[EnumText("封魔のランプ")] G_Lamp = 207,
			[EnumText("智慧のスカラベ")] G_Scarab = 208,
			[EnumText("呪獣胆石")] G_Gallstone = 209,
			[EnumText("原初の産毛")] G_DownyHair = 210,
			[EnumText("奇々神酒")] G_Sake = 211,

			// ピース・モニュメント
			[EnumText("ピース")] Piece = 1000,
			[EnumText("モニュメント")] Monument = 1001,
			// スキル石
			[EnumText("輝石")] Pyroxene = 1010,
			[EnumText("魔石")] Manastone = 1011,
			[EnumText("秘石")] SecretStone = 1012,

			[EnumText("伝承結晶")] TraditionalCrystal = 1020,

			// 配布キャラ再臨素材
			[EnumText("配布再臨素材")] DistributionMaterial = 2000,
		}

		/// <summary> 素材種別 </summary>
		public Type_e Type;
		/// <summary> ピース・モニュメント・スキル石の場合の対応クラス </summary>
		public Class_e Class;
		/// <summary> 配布キャラ再臨素材の場合の名称 </summary>
		public string StrDistributionMaterial;

		public Material(Type_e _type = Type_e.C_Proof) { Type = _type; Class = Class_e.Saber; StrDistributionMaterial = ""; }
		public Material(Type_e _type, Class_e _class) { Type = _type; Class = _class; StrDistributionMaterial = ""; }
		public Material(string _strDistributionMaterial)
		{ Type = Type_e.DistributionMaterial; Class = Class_e.Saber; StrDistributionMaterial = _strDistributionMaterial; }

		public override string ToString()
		{
			switch (Type)
			{
				case Type_e.Piece:
				case Type_e.Monument:
					return Class.GetText() + Type.GetText();
				case Type_e.Pyroxene:
				case Type_e.Manastone:
				case Type_e.SecretStone:
					return Class.GetShortName() + "の" + Type.GetText();
				case Type_e.DistributionMaterial:
					return StrDistributionMaterial;
				default:
					return Type.GetText();
			}
		}
	}

	public static class FGOData
	{
		/// <summary> コマンドカードの攻撃力補正値を取得 </summary>
		/// <param name="_type">カード種別</param>
		/// <param name="_order">何番目か(1番目は1)</param>
		/// <returns>補正値</returns>
		public static decimal GetCorAtk(this CommandCard_e _type, byte _order = 1)
		{
			switch (_type)
			{
				case CommandCard_e.Quick:
					return 0.8m + (_order - 1) * 0.16m;
				case CommandCard_e.Arts:
					return 1 + (_order - 1) * 0.2m;
				case CommandCard_e.Buster:
					return 1.5m + (_order - 1) * 0.3m;
				case CommandCard_e.Extra:
					return 1;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary> コマンドカードのNP補正値を取得 </summary>
		/// <param name="_type">カード種別</param>
		/// <param name="_order">何番目か(1番目は1)</param>
		/// <returns>補正値</returns>
		public static decimal GetCorNP(this CommandCard_e _type, byte _order = 1)
		{
			switch (_type)
			{
				case CommandCard_e.Quick:
					return 1 + (_order - 1) * 0.5m;
				case CommandCard_e.Arts:
					return 3 + (_order - 1) * 1.5m;
				case CommandCard_e.Buster:
					return 0;
				case CommandCard_e.Extra:
					return 1;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary> コマンドカードのStar発生補正値を取得 </summary>
		/// <param name="_type">カード種別</param>
		/// <param name="_order">何番目か(1番目は1)</param>
		/// <returns>補正値</returns>
		public static decimal GetCorStar(this CommandCard_e _type, byte _order = 1)
		{
			switch (_type)
			{
				case CommandCard_e.Quick:
					return 0.8m + (_order - 1) * 0.5m;
				case CommandCard_e.Arts:
					return 0;
				case CommandCard_e.Buster:
					return 0.1m + (_order - 1) * 0.05m;
				case CommandCard_e.Extra:
					return 1;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary> コマンドカードの攻撃力1st補正値を取得 </summary>
		/// <param name="_type">カード種別</param>
		/// <returns>補正値</returns>
		public static decimal GetFirstCorAtk(this CommandCard_e _type) => (_type == CommandCard_e.Buster) ? 0.5m : 0;

		/// <summary> コマンドカードのNP1st補正値を取得 </summary>
		/// <param name="_type">カード種別</param>
		/// <returns>補正値</returns>
		public static decimal GetFirstCorNP(this CommandCard_e _type) => (_type == CommandCard_e.Arts) ? 1 : 0;

		/// <summary> コマンドカードのStar1st補正値を取得 </summary>
		/// <param name="_type">カード種別</param>
		/// <returns>補正値</returns>
		public static decimal GetFirstCorStar(this CommandCard_e _type) => (_type == CommandCard_e.Quick) ? 0.2m : 0;

		public static decimal GetExtraBonus(bool _isCardMatch) => _isCardMatch ? 3.5m : 2;

		/// <summary> クラス毎の短縮漢字名を取得 </summary>
		/// <param name="_class">クラス</param>
		/// <returns>クラス毎の短縮漢字名</returns>
		public static string GetShortName(this Class_e _class)
		{
			switch (_class)
			{
				case Class_e.Saber:
					return "剣";
				case Class_e.Archer:
					return "弓";
				case Class_e.Lancer:
					return "槍";
				case Class_e.Rider:
					return "騎";
				case Class_e.Caster:
					return "術";
				case Class_e.Assassin:
					return "殺";
				case Class_e.Berserker:
					return "狂";
				case Class_e.Shielder:
					return "盾";
				case Class_e.Ruler:
					return "裁";
				case Class_e.Avenger:
					return "讐";
				case Class_e.MoonCancer:
					return "月";
				case Class_e.Alterego:
					return "分";
				case Class_e.Foreigner:
					return "降";
				case Class_e.Beast1:
					return "獣Ⅰ";
				case Class_e.Beast2:
					return "獣Ⅱ";
				case Class_e.Beast3:
					return "獣Ⅲ";
				default:
					return "";
			}
		}

		/// <summary> クラスの攻撃力補正値を取得 </summary>
		/// <param name="_class">クラス</param>
		/// <returns>攻撃力補正値</returns>
		public static decimal GetCorAtk(this Class_e _class)
		{
			switch (_class)
			{
				case Class_e.Archer:
					return 0.95m;
				case Class_e.Lancer:
					return 1.05m;
				case Class_e.Caster:
				case Class_e.Assassin:
					return 0.9m;
				case Class_e.Berserker:
				case Class_e.Ruler:
				case Class_e.Avenger:
					return 1.1m;
				default:
					return 1;
			}
		}

		/// <summary> クラス間相性補正値を取得する </summary>
		/// <param name="_classAtk">攻撃側クラス</param>
		/// <param name="_classDef">防御側クラス</param>
		/// <returns>クラス間相性補正値</returns>
		public static decimal GetCorConflict(this Class_e _classAtk, Class_e _classDef) => GameData.Instance.CorConflictDic[_classAtk][_classDef];

		/// <summary> 天地人間相性補正値を取得する </summary>
		/// <param name="_attriAtk">攻撃側天地人</param>
		/// <param name="_attriDef">防御側天地人</param>
		/// <returns>天地人間相性補正値</returns>
		public static decimal GetCorConflict(this Attributes_e _attriAtk, Attributes_e _attriDef)
		{
			switch (_attriAtk)
			{
				case Attributes_e.Heaven:
					if (_attriDef == Attributes_e.Earth) return 1.1m;
					if (_attriDef == Attributes_e.Man) return 0.9m;
					break;
				case Attributes_e.Earth:
					if (_attriDef == Attributes_e.Man) return 1.1m;
					if (_attriDef == Attributes_e.Heaven) return 0.9m;
					break;
				case Attributes_e.Man:
					if (_attriDef == Attributes_e.Heaven) return 1.1m;
					if (_attriDef == Attributes_e.Earth) return 0.9m;
					break;
				case Attributes_e.Star:
					if (_attriDef == Attributes_e.Beast) return 1.1m;
					break;
				case Attributes_e.Beast:
					if (_attriDef == Attributes_e.Star) return 1.1m;
					break;
				default:
					break;
			}
			return 1;
		}
	}
}
