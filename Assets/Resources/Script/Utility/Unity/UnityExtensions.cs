using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace KMUtility.Unity
{
	public static class ExUnity
	{
		/// <summary> 子要素をソートする </summary>
		/// <param name="_parent">親</param>
		/// <param name="_sortFunc">ソート順決定関数</param>
		public static void OrderSort(this Transform _parent, Func<Transform, Transform, int> _sortFunc)
		{
			List<Transform> objList = new List<Transform>();
			var childCount = _parent.childCount;
			for (int i = 0; i < childCount; i++)
				objList.Add(_parent.GetChild(i));
			objList.Sort((obj1, obj2) => _sortFunc(obj1, obj2));
			foreach (var obj in objList)
				obj.SetSiblingIndex(_parent.childCount - 1);
		}
	}
}
