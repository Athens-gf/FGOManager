using System;
using KMUtility;
using KMUtility.Unity;
using UnityEngine.Events;

namespace FGOManager
{
	#region Enum
	/// <summary> クラス列挙型 </summary>
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

	/// <summary> コマンドカード列挙型 </summary>
	public enum CommandCard_e
	{
		[EnumText("Q")] Quick = 1,
		[EnumText("A")] Arts = 1 << 1,
		[EnumText("B")] Buster = 1 << 2,

		[EnumText("Ex")] Extra = 1 << 3,
		[EnumText("宝具")] NoblePhantasm = 1 << 4,
	}

	/// <summary> 天地人相性列挙型 </summary>
	public enum Attribute_e
	{
		[EnumText("天")] Heaven = 0,
		[EnumText("地")] Earth = 1,
		[EnumText("人")] Man = 2,
		[EnumText("星")] Star = 3,
		[EnumText("獣")] Beast = 4,
	}

	/// <summary> 性別列挙型 </summary>
	public enum Sex_e
	{
		[EnumText("男性")] Male = 0,
		[EnumText("女性")] Female = 1,
		[EnumText("？")] Other = 2,
	}

	/// <summary> 成長タイプ列挙型 </summary>
	public enum StatusTrend_e
	{
		[EnumText("アルトリア〔リリィ〕")] Altria_Lily = 10,
		[EnumText("HP偏重")] HP_Heavy = 2,
		[EnumText("HP寄り")] HP = 1,
		[EnumText("バランス")] Balance = 0,
		[EnumText("ATK寄り")] ATK = -1,
		[EnumText("ATK偏重")] ATK_Heavy = -2,
	}

	/// <summary> 素材列挙型 </summary>
	public enum Material_e
	{
		#region 銅素材
		[EnumText("英雄の証")] C_Proof = 0,
		[EnumText("凶骨")] C_Bone = 1,
		[EnumText("竜の牙")] C_Fang = 2,
		[EnumText("虚影の塵")] C_Dust = 3,
		[EnumText("愚者の鎖")] C_Chain = 4,
		[EnumText("万死の毒針")] C_PoisonNeedle = 5,
		[EnumText("魔術髄液")] C_CerebrospinalFluid = 6,
		[EnumText("宵哭きの鉄杭")] C_IronPile = 7,
		[EnumText("励振火薬")] C_Explosive = 8,
		C_Max,
		#endregion

		#region 銀素材
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
		S_Max,
		#endregion

		#region 金素材
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
		G_Max,
		#endregion

		#region ピース・モニュメント
		[EnumText("セイバーピース")] P_Saber = 1000,
		[EnumText("セイバーモニュメント")] M_Saber = 1010,
		[EnumText("アーチャーピース")] P_Archer = 1001,
		[EnumText("アーチャーモニュメント")] M_Archer = 1011,
		[EnumText("ランサーピース")] P_Lancer = 1002,
		[EnumText("ランサーモニュメント")] M_Lancer = 1012,
		[EnumText("ライダーピース")] P_Rider = 1003,
		[EnumText("ライダーモニュメント")] M_Rider = 1013,
		[EnumText("キャスターピース")] P_Caster = 1004,
		[EnumText("キャスターモニュメント")] M_Caster = 1014,
		[EnumText("アサシンピース")] P_Assassin = 1005,
		[EnumText("アサシンモニュメント")] M_Assassin = 1015,
		[EnumText("バーサーカーピース")] P_Berserker = 1006,
		P_Max,
		[EnumText("バーサーカーモニュメント")] M_Berserker = 1016,
		M_Max,
		PM_Max,
		#endregion

		#region スキル石
		[EnumText("剣の輝石")] SP_Saber = 1050,
		[EnumText("剣の魔石")] SM_Saber = 1060,
		[EnumText("剣の秘石")] SS_Saber = 1070,
		[EnumText("弓の輝石")] SP_Archer = 1051,
		[EnumText("弓の魔石")] SM_Archer = 1061,
		[EnumText("弓の秘石")] SS_Archer = 1071,
		[EnumText("槍の輝石")] SP_Lancer = 1052,
		[EnumText("槍の魔石")] SM_Lancer = 1062,
		[EnumText("槍の秘石")] SS_Lancer = 1072,
		[EnumText("騎の輝石")] SP_Rider = 1053,
		[EnumText("騎の魔石")] SM_Rider = 1063,
		[EnumText("騎の秘石")] SS_Rider = 1073,
		[EnumText("術の輝石")] SP_Caster = 1054,
		[EnumText("術の魔石")] SM_Caster = 1064,
		[EnumText("術の秘石")] SS_Caster = 1074,
		[EnumText("殺の輝石")] SP_Assassin = 1055,
		[EnumText("殺の魔石")] SM_Assassin = 1065,
		[EnumText("殺の秘石")] SS_Assassin = 1075,
		[EnumText("狂の輝石")] SP_Berserker = 1056,
		SP_Max,
		[EnumText("狂の魔石")] SM_Berserker = 1066,
		SM_Max,
		[EnumText("狂の秘石")] SS_Berserker = 1076,
		SS_Max,
		Stone_Max,
		#endregion

		#region その他
		Other,
		[EnumText("伝承結晶")] TraditionalCrystal = 2000,

		// 配布キャラ再臨素材
		[EnumText("配布再臨素材")] DistributionMaterial = 2010,
		#endregion
	}
	[Serializable] public class MaterialEvent : UnityEvent<Material_e> { }

	public enum MaterialType_e
	{
		Copper = 0,
		Silver = 100,
		Gold = 200,

		Piece = 1000,
		Monument = 1010,

		Pyroxene = 1050,
		Manastone = 1060,
		SecretStone = 1070,

		Other = 2000,
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
		}

		/// <summary>  </summary>
		public Type_e Type = Type_e.B;
		/// <summary>  </summary>
		public sbyte Plus = 0;
		/// <summary>  </summary>
		public string Other = "";

		public override string ToString() => (Other != "") ? Other : (Type.GetText() + ((Plus >= 0) ? new string('+', Plus) : "-"));
	}
	[Serializable] public class RankEvent : UnityEvent<Rank> { }

	[Serializable]
	public class MaterialNumber : UnityDictionary<Material_e, int, KV_MaterialNumber> { public MaterialNumber() : base(0) { } }
	[Serializable]
	public class KV_MaterialNumber : KeyAndValue<Material_e, int> { }

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
		public static decimal GetCorConflict(this Attribute_e _attriAtk, Attribute_e _attriDef)
		{
			switch (_attriAtk)
			{
				case Attribute_e.Heaven:
					if (_attriDef == Attribute_e.Earth) return 1.1m;
					if (_attriDef == Attribute_e.Man) return 0.9m;
					break;
				case Attribute_e.Earth:
					if (_attriDef == Attribute_e.Man) return 1.1m;
					if (_attriDef == Attribute_e.Heaven) return 0.9m;
					break;
				case Attribute_e.Man:
					if (_attriDef == Attribute_e.Heaven) return 1.1m;
					if (_attriDef == Attribute_e.Earth) return 0.9m;
					break;
				case Attribute_e.Star:
					if (_attriDef == Attribute_e.Beast) return 1.1m;
					break;
				case Attribute_e.Beast:
					if (_attriDef == Attribute_e.Star) return 1.1m;
					break;
				default:
					break;
			}
			return 1;
		}

		/// <summary> 素材の種別を判別する </summary>
		/// <param name="_material">素材</param>
		/// <param name="_type">素材の種類</param>
		public static bool IsType(this Material_e _material, MaterialType_e _type)
		{
			if ((int)_material < (int)_type)
				return false;
			switch (_type)
			{
				case MaterialType_e.Copper:
					return _material < Material_e.C_Max;
				case MaterialType_e.Silver:
					return _material < Material_e.S_Max;
				case MaterialType_e.Gold:
					return _material < Material_e.G_Max;
				case MaterialType_e.Piece:
					return _material < Material_e.P_Max;
				case MaterialType_e.Monument:
					return _material < Material_e.M_Max;
				case MaterialType_e.Pyroxene:
					return _material < Material_e.SP_Max;
				case MaterialType_e.Manastone:
					return _material < Material_e.SM_Max;
				case MaterialType_e.SecretStone:
					return _material < Material_e.SS_Max;
				case MaterialType_e.Other:
					return true;
				default:
					return false;
			}
		}

		/// <summary> 素材の種別を取得する </summary>
		/// <param name="_material">素材</param>
		public static MaterialType_e GetMaterialType(this Material_e _material)
		{
			foreach (var type in ExEnum.GetEnumIter<MaterialType_e>())
				if (_material.IsType(type))
					return type;
			throw new Exception();
		}
		/// <summary> 素材がピース・モニュメントかどうかを判別する </summary>
		/// <param name="_material">素材</param>
		public static bool IsPieceMonument(this Material_e _material)
			=> _material.IsType(MaterialType_e.Piece) || _material.IsType(MaterialType_e.Monument);

		/// <summary> 素材がスキル石かどうかを判別する </summary>
		/// <param name="_material">素材</param>
		public static bool IsSkillStone(this Material_e _material)
			=> _material.IsType(MaterialType_e.Pyroxene) || _material.IsType(MaterialType_e.Manastone) || _material.IsType(MaterialType_e.SecretStone);

		/// <summary> 素材の対応クラスを取得する </summary>
		/// <param name="_material">素材</param>
		public static Class_e GetClass(this Material_e _material)
		{
			if (!_material.IsPieceMonument() && !_material.IsSkillStone())
				throw new Exception();
			return (Class_e)((int)_material % 10);
		}

		/// <summary> クラスと素材種類から素材を取得する </summary>
		/// <param name="_class">クラス</param>
		/// <param name="_type">素材の種類</param>
		public static Material_e GetMaterial(this Class_e _class, MaterialType_e _type)
		{
			switch (_type)
			{
				case MaterialType_e.Piece:
				case MaterialType_e.Monument:
				case MaterialType_e.Pyroxene:
				case MaterialType_e.Manastone:
				case MaterialType_e.SecretStone:
					return (Material_e)((int)_class + (int)_type);
				default:
					throw new Exception();
			}
		}

		/// <summary> カード枚数取得 </summary>
		/// <param name="_card">カード種別取得</param>
		/// <returns>カード枚数</returns>
		public static int GetCount(this CommandCard.Type_e _cct, CommandCard_e _card)
		{
			bool isQ = ((int)_cct & (int)CommandCard_e.Quick) != 0;
			bool isA = ((int)_cct & (int)CommandCard_e.Arts) != 0;
			bool isB = ((int)_cct & (int)CommandCard_e.Buster) != 0;
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
}
