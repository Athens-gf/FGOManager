using UnityEngine;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	[RequireComponent(typeof(Toggle))]
	public class CustomUIToggle : CustomUI
	{
		public Toggle Toggle { get { return GetComponent<Toggle>(); } }
	}
}
