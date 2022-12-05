using Nakama;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Profetra
{
	public class Scene01MainMenuController : MonoBehaviour
	{
		[SerializeField] BattleMenuUI _battleMenuUI;
		[SerializeField] ClansMenuUI _clansMenuUI;
		[SerializeField] FriendsMenuUI _friendsMenuUI;
		[SerializeField] LeaderboardsMenuUI _leaderboardMenuUI;
		[SerializeField] ProfileMenuUI _profileMenuUI;
		[SerializeField] ClanCreationPanel _clanCreationPanel;
		[SerializeField] LoadingMenu _loadingMenu;
		[SerializeField] ProfilePopup _profilePopup;
		[SerializeField] ProfileUpdatePanel _profileUpdatePanel;
		[SerializeField] NotificationPopup _notificationPopup;

		[SerializeField] Button _battleButton;
		[SerializeField] Button _cardsButton;
		[SerializeField] Button _clansButton;
		[SerializeField] Button _friendsButton;
		[SerializeField] Button _leaderboardsButton;
		[SerializeField] Button _profileButton;
		[SerializeField] private GameConnection _connection;

		private void Awake()
		{
			//_battleButton.onClick.AddListener(() => _battleMenuUI.Show());
			_clansButton.onClick.AddListener(() => _clansMenuUI.Show());
			_friendsButton.onClick.AddListener(() => _friendsMenuUI.Show());
			_leaderboardsButton.onClick.AddListener(() => _leaderboardMenuUI.Show());
			_profileButton.onClick.AddListener(() => _profileMenuUI.Show());
		}

		private async void Start()
		{

			_loadingMenu.Show(true);

			if (_connection.Session == null)
			{
				string deviceId = GetDeviceId();

				if (!string.IsNullOrEmpty(deviceId))
				{
					PlayerPrefs.SetString(GameConstants.DeviceIdKey, deviceId);
				}

				await InitializeGame(deviceId);
			}

//			_battleMenuUI.Init(_connection);
			_loadingMenu.Init(_connection);
			_notificationPopup.Init(_connection);
			_clanCreationPanel.Init(_connection);
//			_profilePopup.Init(_connection, _profileUpdatePanel);
//			_profileUpdatePanel.Init(_connection, GetDeviceId());
			_clansMenuUI.Init(_connection, _profilePopup);
			_friendsMenuUI.Init(_connection);
			_leaderboardMenuUI.Init(_connection, _profilePopup);
//			_profileMenuUI.Init(_connection, _profileUpdatePanel);

			_loadingMenu.Hide(true);
		}

		private async Task InitializeGame(string deviceId)
		{
			var client = new Client(_connection.Protocol, _connection.IpAddress, _connection.Port, _connection.ServerKey, UnityWebRequestAdapter.Instance);
			client.Timeout = 5;

			var socket = client.NewSocket(useMainThread: true);

			string authToken = PlayerPrefs.GetString(GameConstants.AuthTokenKey, null);
			bool isAuthToken = !string.IsNullOrEmpty(authToken);

			string refreshToken = PlayerPrefs.GetString(GameConstants.RefreshTokenKey, null);

			ISession session = null;
			if (isAuthToken)
			{
				session = Session.Restore(authToken, refreshToken);

				if (session.HasExpired(DateTime.UtcNow.AddDays(1)))
				{
					try
					{
						session = await client.SessionRefreshAsync(session);
					}
					catch (ApiResponseException)
					{
						session = await client.AuthenticateDeviceAsync(deviceId);
						PlayerPrefs.SetString(GameConstants.RefreshTokenKey, session.RefreshToken);
					}

					PlayerPrefs.SetString(GameConstants.AuthTokenKey, session.AuthToken);
				}
			}
			else
			{
				session = await client.AuthenticateDeviceAsync(deviceId);
				PlayerPrefs.SetString(GameConstants.AuthTokenKey, session.AuthToken);
				PlayerPrefs.SetString(GameConstants.RefreshTokenKey, session.RefreshToken);
			}

			try
			{
				await socket.ConnectAsync(session);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Error connecting socket: " + e.Message);
			}

			IApiAccount account = null;

			try
			{
				account = await client.GetAccountAsync(session);
			}
			catch (ApiResponseException e)
			{
				Debug.LogError("Error getting user account: " + e.Message);
			}

			_connection.Init(client, socket, account, session);
		}

		private string GetDeviceId()
		{
			string deviceId = "";

			deviceId = PlayerPrefs.GetString(GameConstants.DeviceIdKey);

			if (string.IsNullOrWhiteSpace(deviceId))
			{
				deviceId = Guid.NewGuid().ToString();
			}

			return deviceId;
		}

		private async void OnApplicationQuit()
		{
			await _connection.Socket.CloseAsync();
		}
	}
}
