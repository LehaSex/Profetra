using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System;
using Nakama.TinyJson;
namespace Profetra
{
	public class ChatChannelUI : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField] private GameObject _thisUserMessagePrefab = null;
		[SerializeField] private GameObject _otherUserMessagePrefab = null;
		[SerializeField] private GameObject _serverMessagePrefab = null;
		[Space()]
		[Header("UI Elements")]
		[SerializeField] protected Text _chatNameText = null;
		[SerializeField] private Button _closeButton = null;
		[SerializeField] private Button _sendMessageButton = null;
		[SerializeField] private InputField _chatInputField = null;
		[SerializeField] private RectTransform _content = null;
		private Dictionary<string, ChatMessageUI> _messages = new Dictionary<string, ChatMessageUI>();
		private string _lastMessageUsername;
		protected GameConnection _connection;
		protected IChannel _chatChannel;
		public void Init(GameConnection connection)
		{
			_connection = connection;
		}
		private void OnEnable()
		{
			_sendMessageButton.onClick.AddListener(SendMessage);
			_closeButton.onClick.AddListener(CloseChannelUI);
			_chatInputField.onEndEdit.AddListener(SendMessageIfReturnButton);
		}
		private void OnDisable()
		{
			_sendMessageButton.onClick.RemoveListener(SendMessage);
			_closeButton.onClick.RemoveListener(CloseChannelUI);
		}
		public virtual void SetChatChannel(IChannel channel)
		{
			if (_chatChannel != null)
			{
				if (_chatChannel == channel)
				{
					return;
				}
				_connection.Socket.ReceivedChannelMessage -= AddMessage;
			}
			_chatChannel = channel;
			_connection.Socket.ReceivedChannelMessage += AddMessage;
			_chatNameText.text = channel.RoomName;
			_lastMessageUsername = "";
		}
		private void AddMessage(IApiChannelMessage message)
		{
			GameObject messagePrefab;
			if (message.SenderId == _connection.Account.User.Id)
			{
				messagePrefab = _thisUserMessagePrefab;
			}
			else
			{
				messagePrefab = _otherUserMessagePrefab;
			}
			GameObject messageGO = Instantiate(messagePrefab, _content) as GameObject;
			ChatMessageUI messageUI = messageGO.GetComponent<ChatMessageUI>();
			if (messageUI)
			{
				bool hideUsername = (message.Username == _lastMessageUsername) && !message.Persistent;
				messageUI.InitMessage(message.MessageId, message.Username, message.Content.FromJson<Dictionary<string, string>>()["content"], message.CreateTime, hideUsername);
				_messages.Add(message.MessageId, messageUI);
				_lastMessageUsername = message.Username;
				if (message.Persistent)
				{
					messageUI.transform.SetSiblingIndex(1);
				}
			}
			else
			{
				Debug.LogError("Invalid _thisUserMessagePrefab or _otherUserMessagePrefab! It should contains ChatMessageUI script.");
				Destroy(messageGO);
				return;
			}
		}
		private void AddServerMessage(string messageId, string content, bool historical)
		{
			GameObject messageGO = Instantiate(_serverMessagePrefab, _content) as GameObject;
			ChatServerMessageUI message = messageGO.GetComponent<ChatServerMessageUI>();
			if (message)
			{
				message.Init(content);
				_lastMessageUsername = "";
				if (historical)
				{
					message.transform.SetSiblingIndex(1);
				}
			}
			else
			{
				Debug.LogError("Invalid _serverMessagePrefab! It should contains ChatServerMessageUI script.");
				Destroy(messageGO);
				return;
			}
		}
		private void ClearMessages()
		{
			List<ChatMessageUI> messages = new List<ChatMessageUI>(_messages.Values);
			for (int i = 0; i < messages.Count; i++)
			{
				Destroy(messages[i].gameObject);
			}
			_messages.Clear();
		}
		private void SendMessage()
		{
			if (string.IsNullOrEmpty(_chatInputField.text))
			{
				return;
			}
			try
			{
				var content = new Dictionary<string, string>() { { "content", _chatInputField.text } }.ToJson();
				_connection.Socket.WriteChatMessageAsync(_chatChannel.Id, content);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Error writing chat message: " + e.Message);
			}
			_chatInputField.text = "";
		}
		private void SendMessageIfReturnButton(string value)
		{
			if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(value))
			{
				SendMessage();
			}
		}
		private async void CloseChannelUI()
		{
			try
			{
				await _connection.Socket.LeaveChatAsync(_chatChannel.Id);
			}
			catch (Exception e)
			{
				Debug.LogError("Error leaving chat : " + e.Message);
			}
			ClearMessages();
			gameObject.SetActive(false);
		}
	}
}
