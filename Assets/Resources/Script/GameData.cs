using System;
using System.Linq;
using System.Collections.Generic;
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
		[Serializable]
		public class ClassSpriteDictionary : UnityDictionary<Class_e, Sprite, KP_ClassSprite> { }
		[Serializable]
		public class KP_ClassSprite : KeyAndValue<Class_e, Sprite> { }

		public static readonly string SavePath = "Data/sevPath.png";

		[SerializeField]
		private ClassSpriteDictionary m_ClassSprites = null;
		public ClassSpriteDictionary ClassSprites { get { return m_ClassSprites; } }

		public List<ServantBase> Servants { get; private set; } = new List<ServantBase>();
		public Dictionary<Class_e, Dictionary<Class_e, decimal>> CorConflictDic { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			// クラス間相性データ読み込み
			CorConflictDic = new Dictionary<Class_e, Dictionary<Class_e, decimal>>();
			var csv = new CSVReader(@"Data/CorConflict");
			foreach (Class_e cAtk in Enum.GetValues(typeof(Class_e)))
			{
				CorConflictDic[cAtk] = new Dictionary<Class_e, decimal>();
				foreach (Class_e cDef in Enum.GetValues(typeof(Class_e)))
					CorConflictDic[cAtk][cDef] = csv.GetDecimal((int)cDef, (int)cAtk, -1);
			}

			// サーヴァントデータ読み込み
			if (File.Exists(SavePath))
			{
				List<string> savePath = SaveJsonPng.LoadList<string>(SavePath);
				foreach (string path in savePath)
				{
					List<ServantBase> newSev = SaveJsonPng.LoadList<ServantBase>(path);
					Servants.AddRange(newSev);
				}
				Servants = Servants.OrderBy(s => s.No).ToList();
			}
		}

		private void Update()
		{
		}
	}

}