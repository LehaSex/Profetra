using System.Collections.Generic;
using System.Linq;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ChatChannelClanUI : ChatChannelUI
	{
		[Space()]
		[Header("Prefabs")]
		[Space()]
		[Header("ChatChannelClanUI")]
		[SerializeField] private GameObject _activeUserTextPrefab = null;
		[Space()]
		[Header("UI elements")]
		[SerializeField] private RectTransform _activeUsersListPanel = null;
		private Dictionary<string, Text> _userTexts = new Dictionary<string, Text>();
		public override void SetChatChannel(IChannel chatChannel)
		{
			if (_chatChannel != null && _chatChannel == chatChannel)
			{
				return;
			}
			base.SetChatChannel(chatChannel);
			ClearAllActiveUsersTexts();
			PopulateActiveUsersList(chatChannel.Presences.Select(presence => presence.Username));
		}
		private void AddUserToList(string username)
		{
			_userTexts.Add(username, InstantiateUsernameText(username));
		}
		private void DeleteUserFromList(string username)
		{
			Destroy(_userTexts[username].gameObject);
			_userTexts.Remove(username);
		}
		private void PopulateActiveUsersList(IEnumerable<string> usernames)
		{
			foreach (string username in usernames)
			{
				_userTexts.Add(username, InstantiateUsernameText(username));
			}
		}
		private void ClearAllActiveUsersTexts()
		{
			List<Text> texts = new List<Text>(_userTexts.Values);
			foreach (Text text in texts)
			{
				Destroy(text.gameObject);
			}
			_userTexts.Clear();
		}
		private Text InstantiateUsernameText(string username)
		{
			GameObject textGO = Instantiate(_activeUserTextPrefab, _activeUsersListPanel) as GameObject;
			Text text = textGO.GetComponent<Text>();
			text.text = username;
			return text;
		}
	}
}
