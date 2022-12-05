using UnityEngine;

namespace Profetra
{
	/// <summary>
	/// Часто используемые значения
	/// </summary>
	[CreateAssetMenu(
	   menuName = GameConstants.CreateAssetMenu_GameConfiguration)]
	public class GameConfiguration : ScriptableObject
	{
		//  Параметры ------------------------------------
		public string SceneNameMainMenu { get { return _sceneNameMainMenu; } }
		public string SceneNameBattle { get { return _sceneNameBattle; } }

		// Gameplay - Local Player
		public int StartingGold { get { return _startingGold; } }
		public int MaxGoldCount { get { return _maxGoldCount; } }
		public float GoldPerSecond { get { return _goldPerSecond; } }

		// Audio
		public bool IsAudioEnabled { get { return _isAudioEnabled; } }
		public float AudioVolume { get { return _audioVolume; } }

		//  Fields ----------------------------------------
		[Header("Scenes")]
		[SerializeField] private string _sceneNameMainMenu = "Scene01MainMenu";
		[SerializeField] private string _sceneNameBattle = "Scene02Battle";

		[Header("Gameplay - Local Player")]
		/// <summary>
		/// start coins 
		/// </summary>
		[Range(1, 3)]
		[SerializeField] private int _startingGold = 3;

		/// <summary>
		/// Макс кол-во голды в катке
		/// </summary>
		[Range(3, 10)]
		[SerializeField] private int _maxGoldCount = 10;

		/// <summary>
		/// coins per second
		/// </summary>
		[Range(0.1f, 2f)]
		[SerializeField] private float _goldPerSecond = 0.5f;

		[Header("Audio")]
		[SerializeField] private bool _isAudioEnabled = true;

		[Range(0, 1f)]
		[SerializeField] float _audioVolume = 1;
	}
}
