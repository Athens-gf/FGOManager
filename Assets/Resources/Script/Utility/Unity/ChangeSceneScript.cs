using UnityEngine;
using UnityEngine.SceneManagement;

namespace AthensUtility.Unity
{
	/// <summary>
	/// シーン変更用スクリプト
	/// </summary>
	public class ChangeSceneScript : MonoBehaviour
	{
		[SerializeField]
		private string m_SceneName = "";
		public string SceneName { get { return m_SceneName; } set { m_SceneName = value; } }

		/// <summary>
		/// シーンの変更を行う
		/// </summary>
		public void ChangeScene()
		{
			if (m_SceneName != "")
				SceneManager.LoadScene(m_SceneName);
		}
	}
}
