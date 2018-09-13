using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using KMUtility.Unity;
using KMUtility.Unity.UI;

namespace FGOManager.Register
{
	public class MaterialNode : CustomUIToggle
	{
		[SerializeField]
		private Image m_Image = null;

		[SerializeField]
		private IntEvent m_SetupValue = null;
		public IntEvent OnChangeValue = null;

		private Material_e m_Material = Material_e.C_Bone;
		public Material_e Material
		{
			get { return m_Material; }
			set
			{
				m_Material = value;
				m_Image.sprite = GameData.Instance.MaterialSprites[value];
			}
		}

		private int m_Count = 0;
		public int Count
		{
			get { return m_Count; }
			set
			{
				if (m_Count == value) return;
				m_Count = value;
				OnChangeValue?.Invoke(value);
				m_SetupValue?.Invoke(value);
			}
		}

		public void Select() => OnSelect?.Invoke();
	}
}
