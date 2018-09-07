using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
using KMUtility.Unity;

namespace FGOManager
{
    /// <summary>
    /// Singlton class of Game Data
    /// </summary>
    public class GameData : SingletonMonoBehaviour<GameData>
    {
		private void Start()
		{
			var bc = new CommandCardNumberDict { };
			bc[CommandCard_e.Quick] = 1;
			bc[CommandCard_e.Arts] = 3;
			bc[CommandCard_e.Buster] = 1;
			string s = JsonUtility.ToJson(bc);
			var jc = JsonUtility.FromJson<CommandCardNumberDict>(s);
			Debug.Log(jc);
			Debug.Log(jc[CommandCard_e.Quick]);
			Debug.Log(jc[CommandCard_e.Arts]);
			Debug.Log(jc[CommandCard_e.Buster]);
		}
	}

	#region Enum
	public enum Class_e
	{
		[EnumText("セイバー")]
		Saber,
		[EnumText("アーチャー")]
		Archer,
		[EnumText("ランサー")]
		Lancer,
		[EnumText("ライダー")]
		Rider,
		[EnumText("キャスター")]
		Caster,
		[EnumText("アサシン")]
		Assassin,
		[EnumText("バーサーカー")]
		Berserker,
		[EnumText("シールダー")]
		Shielder,
		[EnumText("ルーラー")]
		Ruler,
		[EnumText("アヴェンジャー")]
		Avenger,
		[EnumText("ムーンキャンサー")]
		MoonCancer,
		[EnumText("アルターエゴ")]
		Alterego,
		[EnumText("フォーリナー")]
		Foreigner,
	}

	public enum CommandCard_e
	{
		Quick,
		Arts,
		Buster,
	}
	#endregion

	[Serializable]
	public class CommandCardNumberDict : UnityDictionary<CommandCard_e, byte, CommandCardNumber> { }

	[Serializable]
	public class CommandCardNumber : KeyAndValue<CommandCard_e, byte> { }

	[Serializable]
	public struct Servant
	{
		public Class_e Class;
		public byte Rare;
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


	}
}