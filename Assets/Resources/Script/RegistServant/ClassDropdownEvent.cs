using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FGOManager.Register
{
	[RequireComponent(typeof(ClassDropdown))]
	public class ClassDropdownEvent : MonoBehaviour
	{
		[Serializable]
		public class ValueChangeEvent : UnityEvent<Class_e> { }
		public ValueChangeEvent OnClassChanged = null;

		public ClassDropdown ClassDropdown { get { return GetComponent<ClassDropdown>(); } }

		protected void Start()
		{
			ClassDropdown.onValueChanged.AddListener(i =>
			{
				var a = ClassDropdown.ReturnList;
				OnClassChanged?.Invoke(ClassDropdown.ReturnList[i]);
			});
		}
	}
}