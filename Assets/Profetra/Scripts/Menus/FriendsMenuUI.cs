using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System;
namespace Profetra
{
	public class FriendsMenuUI : Menu
	{
		[Space]
		[SerializeField] private GameObject _friendPanelPrefab = null;
		[SerializeField] private ScrollRect _scrollRect = null;
		[SerializeField] private Button _refreshButton = null;
		[SerializeField] private Button _addFriendButton = null;
		[SerializeField] private ChatChannelUI _chatChannelUI = null;
		[SerializeField] private UsernameSearcher _usernameSearcher = null;
		[Header("Tabs buttons")]
		[SerializeField] private Button _friendsTabButton = null;
		[SerializeField] private Button _sentInvitesTabButton = null;
		[SerializeField] private Button _receivedInvitesTabButton = null;
		[SerializeField] private Button _bannedUsersTabButton = null;
		[Header("Content panels")]
		[SerializeField] private RectTransform _friendsContent = null;
		[SerializeField] private RectTransform _sentInvitesContent = null;
		[SerializeField] private RectTransform _receivedInvitesContent = null;
		[SerializeField] private RectTransform _bannedUsersContent = null;
		private RectTransform _currentTabContent;
		private Button _currentTabButton;
		private FriendPanel _selectedFriendPanel;
		private GameConnection _connection;
		private void Awake()
		{
			_backButton.onClick.AddListener(() => Hide());
			//vibor taba s druziami
			FriendsTabButtonClicked();
			_friendsTabButton.onClick.AddListener(FriendsTabButtonClicked);
			_sentInvitesTabButton.onClick.AddListener(SentInvitesTabButtonClicked);
			_receivedInvitesTabButton.onClick.AddListener(ReceivedInvitesTabButtonClicked);
			_bannedUsersTabButton.onClick.AddListener(BannedUsersTabButtonClicked);
		}
		public void Init(GameConnection connection)
		{
			_connection = connection;
			_usernameSearcher.Init(connection);
			_chatChannelUI.Init(_connection);
			_addFriendButton.onClick.AddListener(AddFriend);
			_usernameSearcher.OnSubmit += AddFriend;
			_refreshButton.onClick.AddListener(ActualizeFriendsList);
		}
		public async override void Show(bool isMuteButtonClick = false)
		{
			base.Show(isMuteButtonClick);
			var friends = await _connection.Client.ListFriendsAsync(_connection.Session);
			if (friends != null)
			{
				RefreshFriendsListUI(friends);
			}
		}
		private void RefreshFriendsListUI(IApiFriendList friends)
		{
			ClearLists();
			foreach (IApiFriend friend in friends.Friends)
			{
				RectTransform content;
				switch (friend.State)
				{
					case 0: content = _friendsContent; break;
					case 1: content = _sentInvitesContent; break;
					case 2: content = _receivedInvitesContent; break;
					case 3: content = _bannedUsersContent; break;
					default: Debug.LogError("Huevoe sostoyanie druzei: \"" + friend.State + "\" in " + friend.User.Username + "!"); return;
				}
				GameObject panelGO = Instantiate(_friendPanelPrefab, content) as GameObject;
				FriendPanel panel = panelGO.GetComponent<FriendPanel>();
				if (panel)
				{
					panel.Init(_connection, friend);
					panel.OnSelected += SelectedPanelChange;
					panel.OnDataChanged += ActualizeFriendsList;
					panel.OnChatStartButtonClicked += StartChatWithUser;
				}
				else
				{
					Debug.LogError("Hueviy friend panel prefab!");
					Destroy(panelGO);
				}
			}
		}
		private async void ActualizeFriendsList()
		{
			var friends = await _connection.Client.ListFriendsAsync(_connection.Session);
			if (friends != null)
			{
				RefreshFriendsListUI(friends);
			}
		}
		private void ClearLists()
		{
			ClearList(_friendsContent);
			ClearList(_sentInvitesContent);
			ClearList(_receivedInvitesContent);
			ClearList(_bannedUsersContent);
		}
		private void ClearList(RectTransform content)
		{
			FriendPanel[] friendPanels = content.GetComponentsInChildren<FriendPanel>();
			for (int i = 0; i < friendPanels.Length; i++)
			{
				Destroy(friendPanels[i].gameObject);
			}
		}
		public async void AddFriend()
		{
			try
			{
				string[] usernames = new[] { _usernameSearcher.InputFieldValue };
				await _connection.Client.AddFriendsAsync(_connection.Session, new string[] { }, usernames);
				ActualizeFriendsList();
			}
			catch (Exception e) 
			{
				Debug.LogError("Adding friend failed (" + e.Message + ")");
			}
		}
		private void SelectedPanelChange(FriendPanel friendPanel)
		{
			if (_selectedFriendPanel == friendPanel)
			{
				return;
			}
			DeselectCurrentPanel();
			_selectedFriendPanel = friendPanel;
		}
		private void DeselectCurrentPanel(bool closeOldPanelImmediately = false)
		{
			if (_selectedFriendPanel)
			{
				_selectedFriendPanel.Deselect(true);
			}
			_selectedFriendPanel = null;
		}
		private async void StartChatWithUser(string userId, string username)
		{
			IChannel chatChannel;
			try
			{
				chatChannel = await _connection.Socket.JoinChatAsync(userId, ChannelType.DirectMessage, persistence: true, hidden: true);
			}
			catch (ApiResponseException e)
			{
				Debug.LogError("Huynya s chatom: " + e.Message);
				return;
			}
			_chatChannelUI.SetChatChannel(chatChannel);
			_chatChannelUI.gameObject.SetActive(true);
		}
		private void SelectTab(RectTransform content, Button button)
		{
			//return if tab already selected
			if (_currentTabContent == content)
			{
				return;
			}
			DeselectCurrentPanel(true);
			//deselecting current tab
			if (_currentTabContent)
			{
				_currentTabContent.gameObject.SetActive(false);
			}
			if (_currentTabButton)
			{
				_currentTabButton.interactable = true;
			}
			//selecting new tab
			content.gameObject.SetActive(true);
			_currentTabContent = content;
			button.interactable = false;
			_currentTabButton = button;
			_scrollRect.content = _currentTabContent;
		}
		private void FriendsTabButtonClicked()
		{
			SelectTab(_friendsContent, _friendsTabButton);
		}
		private void SentInvitesTabButtonClicked()
		{
			SelectTab(_sentInvitesContent, _sentInvitesTabButton);
		}
		private void ReceivedInvitesTabButtonClicked()
		{
			SelectTab(_receivedInvitesContent, _receivedInvitesTabButton);
		}
		private void BannedUsersTabButtonClicked()
		{
			SelectTab(_bannedUsersContent, _bannedUsersTabButton);
		}
	}
}
