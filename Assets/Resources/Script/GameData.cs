using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMUtility.Unity;

namespace FGOManager
{
	/// <summary>
	/// Singlton class of Game Data
	/// </summary>
	public class GameData : SingletonMonoBehaviour<GameData>
	{

		public Dictionary<Class_e, Dictionary<Class_e, decimal>> CorConflictDic { get; private set; }

		private void Start()
		{
			/*
			ServantBase sv = new ServantBase { Name = "test" };
			SaveJsonPng.Save("a.png", sv);
			var s = SaveJsonPng.Load<ServantBase>("a.png");
			Debug.Log(s.Name);
			*/

			// クラス間相性データ読み込み
			CorConflictDic = new Dictionary<Class_e, Dictionary<Class_e, decimal>>();
			var csv = new CSVReader(@"Data/CorConflict");
			foreach (Class_e cAtk in Enum.GetValues(typeof(Class_e)))
			{
				CorConflictDic[cAtk] = new Dictionary<Class_e, decimal>();
				foreach (Class_e cDef in Enum.GetValues(typeof(Class_e)))
					CorConflictDic[cAtk][cDef] = csv.GetDecimal((int)cDef, (int)cAtk, -1);
			}
		}

		private void Update()
		{
		}
	}

}