

using System;
using Nakama;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Profetra
{


	public class BattleMenuUI : Menu
	{

		[SerializeField] private RectTransform _rotatingSymbol = null;
		[SerializeField] private float _degreesPerSecond = 90;

		private IMatchmakerTicket _ticket;
		private GameConnection _connection;

		public void Init(GameConnection connection)
		{
			_connection = connection;
			_backButton.onClick.AddListener(() => Hide());
		}

		private void Update()
		{
			if (gameObject.activeInHierarchy)
			{
				_rotatingSymbol.Rotate(Vector3.forward, -_degreesPerSecond * Time.deltaTime);
			}
		}

		public async override void Show(bool isMuteButtonClick = false)
		{
			_connection.Socket.ReceivedMatchmakerMatched += OnMatchmakerMatched;

			try
			{

				_ticket = await _connection.Socket.AddMatchmakerAsync(
					query: "*",
					minCount: 2,
					maxCount: 2,
					stringProperties: null,
					numericProperties: null);

			}
			catch (Exception e)
			{
				Debug.LogWarning("An error has occured while joining the matchmaker: " + e);
			}

			base.Show(isMuteButtonClick);
		}

		public async override void Hide(bool isMuteSoundManager = false)
		{
			try
			{
				await _connection.Socket.RemoveMatchmakerAsync(_ticket);
			}
			catch (Exception e)
			{
				Debug.LogWarning("An error has occured while removing from matchmaker: " + e);
			}

			_connection.Socket.ReceivedMatchmakerMatched -= OnMatchmakerMatched;
			_ticket = null;
			base.Hide(isMuteSoundManager);
		}

		private void OnMatchmakerMatched(IMatchmakerMatched matched)
		{
			//_connection.BattleConnection = new BattleConnection(matched);
			_connection.Socket.ReceivedMatchmakerMatched -= OnMatchmakerMatched;

			Debug.Log("matchmaker matched called");

			SceneManager.LoadScene(GameConfigurationManager.Instance.GameConfiguration.SceneNameBattle);
		}
	}
}
