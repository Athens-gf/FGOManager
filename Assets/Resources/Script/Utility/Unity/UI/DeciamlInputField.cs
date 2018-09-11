using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KMUtility.Unity.UI
{
	public class DeciamlInputField : CustomUIInputField
	{
		[Serializable]
		public struct SetValue
		{
			public bool IsUse;
			public decimal Value;
		}
		public SetValue Max = new SetValue { IsUse = false, Value = 0 };
		public SetValue Min = new SetValue { IsUse = false, Value = 0 };
		public SetValue Scroll = new SetValue { IsUse = false, Value = 1 };

		[SerializeField]
		private decimal m_Value = 0;
		public decimal Value
		{
			get { return m_Value; }
			set
			{
				if (Max.IsUse) m_Value = System.Math.Min(value, Max.Value);
				if (Min.IsUse) m_Value = System.Math.Max(value, Min.Value);
				Text = m_Value.ToString();
				OnEndEditValue?.Invoke(m_Value);
			}
		}

		[Serializable]
		public class SubmitEvent : UnityEvent<decimal> { }

		public SubmitEvent OnEndEditValue = null;

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
