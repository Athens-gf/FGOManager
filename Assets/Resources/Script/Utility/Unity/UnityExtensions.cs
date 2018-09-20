using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AthensUtility.Unity
{
	[Serializable] public class StringEvent : UnityEvent<string> { }
	[Serializable] public class IntEvent : UnityEvent<int> { }
	[Serializable] public class DoubleEvent : UnityEvent<double> { }
	[Serializable] public class DecimalEvent : UnityEvent<decimal> { }
	[Serializable] public class BoolEvent : UnityEvent<bool> { }
	[Serializable] public class OptionDatasEvent : UnityEvent<List<Dropdown.OptionData>> { }

	public static class ExUnity
	{
		/// <summary> 子要素をソートする </summary>
		/// <param name="_parent">親</param>
		/// <param name="_sortFunc">ソート順決定関数</param>
		public static void OrderSort<T>(this Transform _parent, Func<Transform, T> _keySelector) where T : IComparable
		{
			List<Transform> objList = new List<Transform>();
			var childCount = _parent.childCount;
			for (int i = 0; i < childCount; i++)
				objList.Add(_parent.GetChild(i));
			objList.OrderBy(obj => _keySelector(obj)).ToList().ForEach(obj => obj.SetSiblingIndex(_parent.childCount - 1));
		}

	}
}
