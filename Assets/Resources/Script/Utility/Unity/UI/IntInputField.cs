using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AthensUtility.Unity.UI
{
	public class IntInputField : CustomUIInputField
	{
		[Serializable]
		public struct SetValue
		{
			public bool IsUse;
			public int Value;
		}
		public SetValue Max = new SetValue { IsUse = false, Value = 0 };
		public SetValue Min = new SetValue { IsUse = false, Value = 0 };
		public SetValue Scroll = new SetValue { IsUse = false, Value = 1 };

		private void ApplyMinMaxValue() => Value = GetApplyMinMaxValue(Value);
		private int GetApplyMinMaxValue(int _value)
		{
			if (Max.IsUse && Min.IsUse) return Mathf.Clamp(_value, Min.Value, Max.Value);
			if (Max.IsUse) return Mathf.Min(_value, Max.Value);
			if (Min.IsUse) return Mathf.Max(_value, Min.Value);
			return _value;
		}

		public bool IsUseMax { get { return Max.IsUse; } set { Max.IsUse = value; ApplyMinMaxValue(); } }
		public int MaxValue { get { return Max.Value; } set { Max.Value = value; ApplyMinMaxValue(); } }
		public bool IsUseMin { get { return Min.IsUse; } set { Min.IsUse = value; ApplyMinMaxValue(); } }
		public int MinValue { get { return Min.Value; } set { Min.Value = value; ApplyMinMaxValue(); } }
		public bool IsScroll { get { return Scroll.IsUse; } set { Scroll.IsUse = value; } }
		public int ScrollValue { get { return Scroll.Value; } set { Scroll.Value = value; } }

		[SerializeField]
		private int m_Value = 0;
		public int Value
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

		public IntEvent OnEndEditValue = null;

		void OnValidate() { Value = m_Value; }

		protected override void Reset()
		{
			base.Reset();
			InputField.contentType = InputField.ContentType.IntegerNumber;
		}

		protected override void Start()
		{
			base.Start();
			InputField.contentType = InputField.ContentType.IntegerNumber;
			Value = m_Value;
			Text = m_Value.ToString();
			OnEndEdit.AddListener(s =>
			{
				int value = 0;
				if (int.TryParse(s, out value))
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
