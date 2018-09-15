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

	/// <summary> 即時効果 </summary>
	[DataContract]
	public abstract class EffImmediate : Effect
	{
		public EffImmediate(Type_e _type) : base(_type) { }
	}
}

