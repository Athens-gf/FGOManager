using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
using KMUtility.Math;
using KMUtility.Unity;

namespace FGOManager
{
	/// <summary> 効果抽象クラス </summary>
	[Serializable]
	public abstract class Effect
	{
		public enum Type_e
		{
			// 宝具攻撃
			Attack,
			// 即時発動効果
			Immediate,
			// 強化付与
			Buff,
			// 弱体付与
			Debuff,
		}

		/// <summary> 効果の種別 </summary>
		public Type_e Type;


	}

	[Serializable]
	public class EffAttack : Effect
	{

	}
}
