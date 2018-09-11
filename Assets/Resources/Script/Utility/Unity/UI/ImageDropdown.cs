using UnityEngine;
using UnityEngine.UI;

namespace KMUtility.Unity.UI
{
	public class ImageDropdown : CustomDropdown
	{
		[SerializeField]
		private RectTransform m_Item = null;

		void OnValidate()
		{
			if (m_Item)
			{
				m_Item.sizeDelta = Rect.sizeDelta;
				LayoutElement layoutElement = m_Item.GetComponent<LayoutElement>();
				if (layoutElement)
					layoutElement.preferredHeight = Rect.sizeDelta.y;
			}
		}
	}
}
