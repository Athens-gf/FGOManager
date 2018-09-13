using System;
using System.Linq;
using System.Collections.Generic;
using KMUtility;
using KMUtility.Unity;
using System.IO;
using UnityEngine;

namespace FGOManager
{
	/// <summary>
	/// Singlton class of Game Data
	/// </summary>
	public class GameData : SingletonMonoBehaviour<GameData>
	{
		[Serializable] public class ClassSpriteDictionary : UnityDictionary<Class_e, Sprite, KP_ClassSprite> { }
		[Serializable] public class KP_ClassSprite : KeyAndValue<Class_e, Sprite> { }
		[Serializable] public class MaterialSpriteDictionary : UnityDictionary<Material_e, Sprite, KP_MaterialSprite> { }
		[Serializable] public class KP_MaterialSprite : KeyAndValue<Material_e, Sprite> { }

		public static readonly string SavePath = "Data/sevPath.png";

		[SerializeField]
		private ClassSpriteDictionary m_ClassSprites = null;
		public ClassSpriteDictionary ClassSprites { get { return m_ClassSprites; } }

		[SerializeField]
		private MaterialSpriteDictionary m_MaterialSprites = null;
		public MaterialSpriteDictionary MaterialSprites { get { return m_MaterialSprites; } }


		[ContextMenu("Setup Sprite")]
		private void SetupSprites()
		{
			m_ClassSprites = new ClassSpriteDictionary();
			Dictionary<string, Class_e> dicClass = ExEnum.GetEnumIter<Class_e>().Where(c => c <= Class_e.Foreigner)
				.Select((c, i) => new { Class = c, Index = i }).ToDictionary(ci => $"Class_{ci.Index}", ci => ci.Class);
			m_ClassSprites = new ClassSpriteDictionary();
			m_ClassSprites.Table = Resources.LoadAll<Sprite>("Texture/Class").ToDictionary(s => dicClass[s.name], s => s);

			Func<MaterialType_e, string> getPrefix = type =>
			{
				switch (type)
				{
					case MaterialType_e.Copper:
						return "C";
					case MaterialType_e.Silver:
						return "S";
					case MaterialType_e.Gold:
						return "G";
					case MaterialType_e.Piece:
						return "P";
					case MaterialType_e.Monument:
						return "M";
					case MaterialType_e.Pyroxene:
						return "SP";
					case MaterialType_e.Manastone:
						return "SM";
					case MaterialType_e.SecretStone:
						return "SS";
					case MaterialType_e.Other:
						return "Ot";
					default:
						throw new Exception();
				}
			};
			Dictionary<string, Material_e> dicMat = new Dictionary<string, Material_e>();
			ExEnum.GetEnumIter<MaterialType_e>().ToList().ForEach(type =>
			dicMat = dicMat.AppendDictionary(ExEnum.GetEnumIter<Material_e>().Where(mat => mat.IsType(type)).Select((mat, i) => new { Mat = mat, Index = i })
			.ToDictionary(mi => $"{getPrefix(type)}{mi.Index:D2}", mi => mi.Mat)));
			m_MaterialSprites = new MaterialSpriteDictionary();
			m_MaterialSprites.Table = Resources.LoadAll<Sprite>("Texture/Materials0")
				.ReternAppend(Resources.LoadAll<Sprite>("Texture/Materials1"))
				.ToDictionary(s => dicMat[s.name], s => s);
		}

		public List<ServantBase> Servants { get; private set; } = new List<ServantBase>();
		public Dictionary<Class_e, Dictionary<Class_e, decimal>> CorConflictDic { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			// クラス間相性データ読み込み
			CorConflictDic = new Dictionary<Class_e, Dictionary<Class_e, decimal>>();
			var csv = new CSVReader(@"Data/CorConflict");
			foreach (Class_e cAtk in ExEnum.GetEnumIter<Class_e>())
			{
				CorConflictDic[cAtk] = new Dictionary<Class_e, decimal>();
				ExEnum.GetEnumIter<Class_e>().ToList().ForEach(cDef => CorConflictDic[cAtk][cDef] = csv.GetDecimal((int)cDef, (int)cAtk, -1));
			}

			// サーヴァントデータ読み込み
			if (File.Exists(SavePath))
			{
				List<string> savePath = SaveJsonPng.LoadList<string>(SavePath);
				savePath.ForEach(path => Servants.AddRange(SaveJsonPng.LoadList<ServantBase>(path)));
				Servants = Servants.OrderBy(s => s.No).ToList();
			}

			return;
		}

		private void Update()
		{
		}
	}

}