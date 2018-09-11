using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KMUtility;
using KMUtility.Unity.UI;
using System.Collections.Generic;

namespace FGOManager.Register
{
	public class ClassDropdown : EnumDropdown<Class_e>
	{
		public override Dictionary<Class_e, Sprite> SpriteDic { get { return GameData.Instance.ClassSprites.Table; } }
		public override Func<Class_e, int> FuncOrder { get { return c => (int)c; } }
		public override List<Class_e> EnumList => base.EnumList.Where(c => c < Class_e.Beast1).ToList();

		[ContextMenu("SetOptions")]
		protected override void SetOptions() { base.SetOptions(); }

	}
}