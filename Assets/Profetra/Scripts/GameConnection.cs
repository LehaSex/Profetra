using Nakama;
using UnityEngine;

namespace Profetra
{
	[CreateAssetMenu(fileName = "GameConnection",
	menuName = GameConstants.CreateAssetMenu_GameConnection)]
	public class GameConnection : ScriptableObject
	{
		/// <summary>
		/// Используется для подключения между клиентом и сервером.
		/// Содержит в себе полезные методы для работы с сервером.
		///
		/// Не использовать напрямую, использовать заместо этого <see cref="Client"/>
		///
		/// </summary>
		private IClient _client;

		/// <summary>
		/// Используется для отправки сообщений серваку
		///
		/// Чтобы пользователь мог получать сообщения от серва,
		/// <see cref="Session"/> не должна быть просрочена.
		///
		/// Для инициализации сессии вызывать <see cref="AuthenticateDeviceIdAsync"/>
		///
		/// Для повторой аутентификации чек <see cref="Reauthenticate"/> 
		/// </summary>
		public ISession Session { get; set; }
		public IApiAccount Account { get; set; }

		private ISocket _socket;

		public IClient Client => _client;

		public ISocket Socket => _socket;
		// Connection
		public string IpAddress { get { return _ipAddress; } }
		public string Protocol { get { return _protocol; } }
		public int Port { get { return _port; } }
		public string ServerKey { get { return _serverKey; } }
		//public BattleConnection BattleConnection { get; set; }

		[Header("Connection")]
		[SerializeField] private string _protocol = "http";
		[SerializeField] private string _ipAddress = "192.168.1.10";
		[SerializeField] private int _port = 7350;
		[SerializeField] private string _serverKey = "defaultkey";

		public void Init(IClient client, ISocket socket, IApiAccount account, ISession session)
		{
			_client = client;
			_socket = socket;
			Account = account;
			Session = session;
		}
	}
}
