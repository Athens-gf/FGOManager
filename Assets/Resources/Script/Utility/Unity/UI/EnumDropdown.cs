using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	public class EnumDropdown<T> : Dropdown where T : struct
	{

		public virtual Dictionary<T, Sprite> SpriteDic { get; protected set; } = null;
		public virtual Func<T, int> FuncOrder { get; protected set; } = null;
		public virtual List<T> EnumList => Enum.GetValues(typeof(T)).Cast<T>().ToList();
		public List<T> ReturnList { get; protected set; } = null;
		public Dictionary<T, int> IndexDic { get; protected set; } = null;

		public virtual T Value
		{
			get { return ReturnList[value]; }
			set
			{
				this.value = IndexDic[value];
				onValueChanged?.Invoke(this.value);
			}
		}

		[ContextMenu("SetOptions")]
		protected virtual void SetOptions()
		{
			if (SpriteDic == null || FuncOrder == null) return;
			ReturnList = EnumList.Where(t => SpriteDic.ContainsKey(t)).OrderBy(t => FuncOrder(t)).ToList();
			ClearOptions();
			AddOptions(ReturnList.Select(t => new OptionData(t.ToString(), SpriteDic[t])).ToList());
		}

		protected override void Awake()
		{
			base.Awake();
			SetOptions();
			IndexDic = EnumList.Where(t => SpriteDic.ContainsKey(t)).OrderBy(t => FuncOrder(t))
				.Select((t, i) => new { Type = t, Index = i }).ToDictionary(x => x.Type, x => x.Index);
		}
	}
}