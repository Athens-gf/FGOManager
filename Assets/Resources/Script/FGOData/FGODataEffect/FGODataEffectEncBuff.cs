using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
using KMUtility.Math;
using KMUtility.Unity;
using System.Runtime.Serialization;

namespace FGOManager
{
	public static partial class FGOData
	{
	}

	/// <summary> バフ効果(付与系) </summary>
	[DataContract]
	public abstract class EffBuffEnc : Effect { public EffBuffEnc(Type_e _type) : base(_type) { } }

	/// <summary> 特攻付与 </summary>
	[DataContract]
	public class EffSpecialAttack : EffBuffEnc
	{
		public override bool IsBuff => true;
		public EffSpecialAttack() : base(Type_e.ENC_SpecialAttack) { }
		public override string GetDescriptionText() => $"{Fillter}特攻状態を付与";
	}

}

