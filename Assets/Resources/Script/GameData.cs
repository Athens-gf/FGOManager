using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMUtility.Unity;

namespace FGOManager
{
	/// <summary>
	/// Singlton class of Game Data
	/// </summary>
	public class GameData : SingletonMonoBehaviour<GameData>
	{
		public ServantBase Servant;

		private void Start()
		{
		}
	}

}