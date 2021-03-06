﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AthensUtility.Unity.UI
{
	public class DeciamlInputField : CustomUIInputField
	{
		[Serializable]
		public struct SetValue
		{
			public bool IsUse;
			public double m_Value;
			public decimal Value { get { return (decimal)m_Value; } set { m_Value = (double)value; } }
		}
		public SetValue Max = new SetValue { IsUse = false, Value = 0 };
		public SetValue Min = new SetValue { IsUse = false, Value = 0 };
		public SetValue Scroll = new SetValue { IsUse = false, Value = 1 };

		private void ApplyMinMaxValue() => Value = GetApplyMinMaxValue(Value);
		private decimal GetApplyMinMaxValue(decimal _value)
		{
			if (Max.IsUse && Min.IsUse) return System.Math.Min(System.Math.Max(_value, Min.Value), Max.Value);
			if (Max.IsUse) return System.Math.Min(_value, Max.Value);
			if (Min.IsUse) return System.Math.Max(_value, Min.Value);
			return _value;
		}

		public bool IsUseMax { get { return Max.IsUse; } set { Max.IsUse = value; ApplyMinMaxValue(); } }
		public decimal MaxValue { get { return Max.Value; } set { Max.Value = value; ApplyMinMaxValue(); } }
		public bool IsUseMin { get { return Min.IsUse; } set { Min.IsUse = value; ApplyMinMaxValue(); } }
		public decimal MinValue { get { return Min.Value; } set { Min.Value = value; ApplyMinMaxValue(); } }
		public bool IsScroll { get { return Scroll.IsUse; } set { Scroll.IsUse = value; } }
		public decimal ScrollValue { get { return Scroll.Value; } set { Scroll.Value = value; } }

		[SerializeField]
		private decimal m_Value = 0;
		public decimal Value
		{
			get { return m_Value; }
			set
			{
				var val = GetApplyMinMaxValue(value);
				if (m_Value == val) return;
				m_Value = val;
				Text = m_Value.ToString();
				OnEndEditValue?.Invoke(m_Value);
			}
		}

		public DecimalEvent OnEndEditValue = null;

		void OnValidate() { Value = m_Value; }

		protected override void Reset()
		{
			base.Reset();
			InputField.contentType = InputField.ContentType.DecimalNumber;
		}

		protected override void Start()
		{
			base.Start();
			InputField.contentType = InputField.ContentType.DecimalNumber;
			Value = m_Value;
			OnEndEdit.AddListener(s =>
			{
				decimal value = 0;
				if (decimal.TryParse(s, out value))
					Value = value;
				else
					Value = m_Value;
				OnEndEditValue?.Invoke(Value);
			});

			var entry = new EventTrigger.Entry { eventID = EventTriggerType.Scroll };
			entry.callback.AddListener(e =>
			{
				if (!Scroll.IsUse) return;
				Vector2? vec = (e as PointerEventData)?.scrollDelta;
				if (vec != null && vec.Value.y != 0)
					Value += Scroll.Value * (vec.Value.y > 0 ? 1 : -1);
				OnEndEdit.Invoke(Value.ToString());
			});
			Trigger.triggers.Add(entry);
		}
	}
}
