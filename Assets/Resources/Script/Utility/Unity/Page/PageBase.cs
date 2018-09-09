using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KMUtility.Unity.Page
{
	public abstract class PageBase : MonoBehaviour
	{
		public Button ButtonBack = null;

		/// <summary> 終了時の破棄を行う </summary>
		public virtual void Destroy() => Destroy(gameObject);
	}
}