using UnityEngine;

namespace Profetra
{
	public class GameConfigurationManager : Singleton<GameConfigurationManager>
	{
		public GameConfiguration GameConfiguration { get { return gameConfiguration; } }

		[SerializeField] private GameConfiguration gameConfiguration = null;
	}
}
