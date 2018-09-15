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

	/// <summary> デバフ効果 </summary>
	[DataContract]
	public abstract class EffDebuff : Effect
	{
		public EffDebuff(Type_e _type) : base(_type) { }
	}
}

