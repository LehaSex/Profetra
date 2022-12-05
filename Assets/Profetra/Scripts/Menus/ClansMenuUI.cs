using System;
using System.Collections.Generic;
using System.Linq;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ClansMenuUI : Menu
	{
		[Space]
		[SerializeField] private Button _createClanButton = null;
		[SerializeField] private ClanCreationPanel _clanCreationPrefab = null;
		[SerializeField] private Button _removeClanButton = null;
		[SerializeField] private Button _refreshClanButton = null;
		[Space]
		[SerializeField] private Button _joinClanButton = null;
		[SerializeField] private Button _leaveClanButton = null;
		[SerializeField] private Button _chatButton = null;
		private List<ClanUserEntry> _clanMembers = new List<ClanUserEntry>();
		private ClanUserEntry _selectedMember = null;
		[Space]
		[SerializeField] private InputField _clanSearchInput = null;
		[SerializeField] private Button _clanSearchButton = null;
		[SerializeField] private ClanSearchResult _clanSearchResultPrefab = null;
		[SerializeField] private RectTransform _clanSearchList = null;
		[Space]
		[SerializeField] private ClanUserEntry _clanUserEntryPrefab = null;
		[SerializeField] private RectTransform _clanUserList = null;
		[Space]
		[SerializeField] private Text _clanDisplayName = null;
		[SerializeField] private CanvasGroup _searchTab = null;
		[SerializeField] private Button _searchTabButton = null;
		[SerializeField] private CanvasGroup _detailsTab = null;
		[SerializeField] private Button _detailsTabButton = null;
		[SerializeField] private ChatChannelClanUI _chatChannelClanUI = null;
		private GameConnection _connection;
		private ProfilePopup _profilePopup;
		private readonly ClanMenuUIState _state = new ClanMenuUIState();
		public void Init(GameConnection connection, ProfilePopup profilePopup)
		{
			_connection = connection;
			_profilePopup = profilePopup;
			_chatChannelClanUI.Init(_connection);
		}
		private void Awake()
		{
			_clanCreationPrefab.OnClanCreated += clan =>
			{
				_state.UserClan = clan;
				_state.SubMenu = ClanSubMenu.Details;
				RefreshUI(_state);
			};
			_backButton.onClick.AddListener(() => Hide());
			_createClanButton.onClick.AddListener(() =>
			{
				_clanCreationPrefab.Show();
			});
			_removeClanButton.onClick.AddListener(DeleteClan);
			_clanSearchButton.onClick.AddListener(SearchClan);
			_leaveClanButton.onClick.AddListener(LeaveClan);
			_joinClanButton.onClick.AddListener(JoinDisplayedClan);
			_searchTabButton.onClick.AddListener(() =>
			{
				_state.DisplayedClan = null;
				_state.SubMenu = ClanSubMenu.Search;
				RefreshUI(_state);
			});
			_detailsTabButton.onClick.AddListener(() =>
			{
				_state.SubMenu = ClanSubMenu.Details;
				RefreshUI(_state);
			});
			_chatButton.onClick.AddListener(() => StartChat(_state));
			_refreshClanButton.onClick.AddListener(SearchClan);
			_clanSearchInput.onEndEdit.AddListener(SearchClanOnReturnClicked);
		}
		private async void DeleteClan()
		{
			try
			{
				await _connection.Client.DeleteGroupAsync(_connection.Session, _state.UserClan.Id);
			}
			catch (ApiResponseException e)
			{
				Debug.LogError("Nevozmozhno udalit klan: " + e.Message);
			}
			_state.DisplayedClan = null;
			_state.UserClan = null;
			_state.UserClanRank = null;
			RefreshUI(_state);
		}
		private async void JoinDisplayedClan()
		{
			try
			{
				await _connection.Client.JoinGroupAsync(_connection.Session, _state.DisplayedClan.Id);
				_state.UserClan = _state.DisplayedClan;
				RefreshUI(_state);
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka s vhodom v klan s kodom " + e.StatusCode + ": " + e.Message);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Oshibka s interfeisom klanov " + e.Message);
			}
		}
		private async void LeaveClan()
		{
			try
			{
				await _connection.Client.LeaveGroupAsync(_connection.Session, _state.DisplayedClan.Id);
				_state.UserClan = null;
				_state.SubMenu = ClanSubMenu.Search;
				RefreshUI(_state);
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka API. Code: " + e.StatusCode + ", Message: " + e.Message);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Oshibka vo vremya vihoda iz klana: " + e.Message);
			}
		}
		private async void SearchClan()
		{
			string name = _clanSearchInput.text;
			try
			{
				IApiGroupList clanList = await _connection.Client.ListGroupsAsync(_connection.Session, name);
				OnClanListFound(clanList);
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka vo vremya poiska klana: " + e);
			}
		}
		private void SearchClanOnReturnClicked(string text)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				SearchClan();
			}
		}
		public override async void Show(bool isMuteButtonClick = false)
		{
			base.Show(isMuteButtonClick);
			try
			{
				IApiUserGroupList groupList = await _connection.Client.ListUserGroupsAsync(_connection.Session);
				foreach (var group in groupList.UserGroups)
				{
					_state.UserClan = group.Group;
					_state.UserClanRank = group.State;
					_state.DisplayedClan = _state.UserClan;
					_state.SubMenu = ClanSubMenu.Details;
					break;
				}
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka s ustanovleniem klana usera: " + e.Message);
			}
			RefreshUI(_state);
			_connection.Socket.ReceivedNotification += NotificationReceived;
		}
		private void OnClanListFound(IApiGroupList clans)
		{
			foreach (Transform transform in _clanSearchList)
			{
				Destroy(transform.gameObject);
			}
			if (clans.Groups.Any())
			{
				foreach (IApiGroup clan in clans.Groups)
				{
					ClanSearchResult result = Instantiate(_clanSearchResultPrefab, _clanSearchList);
					result.SetClan(clan, clickedClan =>
					{
						_state.DisplayedClan = clickedClan;
						_state.SubMenu = ClanSubMenu.Details;
						RefreshUI(_state);
					});
				}
			}
		}
		private void OnClanUserListReceived(IEnumerable<IGroupUserListGroupUser> userList)
		{
			_clanMembers.Clear();
			foreach (Transform child in _clanUserList)
			{
				Destroy(child.gameObject);
			}
			foreach (IGroupUserListGroupUser user in userList)
			{
				if (user.User.Id == _connection.Account.User.Id)
				{
					_state.UserClanRank = user.State;
					break;
				}
			}
			foreach (IGroupUserListGroupUser user in userList)
			{
				ClanUserEntry userEntry = Instantiate(_clanUserEntryPrefab, _clanUserList);
				userEntry.Init(_connection.Session.UserId);
				userEntry.SetUser(user.User, user.State, _state.UserClanRank.Value, OnUserSelected, OnUserKick, OnUserPromote, OnUserShowProfile);
				_clanMembers.Add(userEntry);
			}
			RefreshUI(_state);
		}
		private void OnUserSelected(ClanUserEntry sender)
		{
			if (_selectedMember == sender)
			{
				_selectedMember.HideInteractionPanel();
				_selectedMember = null;
			}
			else
			{
				if (_selectedMember != null)
				{
					_selectedMember.HideInteractionPanel();
				}
				_selectedMember = sender;
				_selectedMember.ShowInteractionPanel();
			}
		}
		private async void OnUserKick(IApiUser kickedUser)
		{
			try
			{
				await _connection.Client.KickGroupUsersAsync(_connection.Session, _state.UserClan.Id, new string[] { kickedUser.Id });
				var userEnumeration = await _connection.Client.ListGroupUsersAsync(_connection.Session, _state.UserClan.Id, null, 1, null);
				OnClanUserListReceived(userEnumeration.GroupUsers);
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka s kikom usera " + e.Message);
			}
		}
		private async void OnUserPromote(IApiUser user)
		{
			try
			{
				await _connection.Client.PromoteGroupUsersAsync(_connection.Session, _state.UserClan.Id, new string[] { user.Id });
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka s priglasheniem usera " + user.Username
						+ " v clan " + _state.UserClan.Name + ": " + e);
			}
		}
		private void OnUserShowProfile(IApiUser user)
		{
			_profilePopup.Show(user);
		}
		private void NotificationReceived(IApiNotification notification)
		{
			if (notification.Code == (int)NotificationCode.Clan_RefreshMembers)
			{
				SearchClan();
			}
			if (notification.Code == (int)NotificationCode.Clan_Delete)
			{
				_state.DisplayedClan = null;
				_state.UserClan = null;
				_state.UserClanRank = null;
				RefreshUI(_state);
			}
		}
		private void HideSubMenu(CanvasGroup tab)
		{
			tab.alpha = 0;
			tab.blocksRaycasts = false;
		}
		private void ShowSubMenu(CanvasGroup tab)
		{
			tab.alpha = 1;
			tab.blocksRaycasts = true;
		}
		private void RefreshUI(ClanMenuUIState state)
		{
			_clanDisplayName.text = state.UserClan?.Name ?? state.DisplayedClan?.Name ?? "Без клана";
			if (state.SubMenu == ClanSubMenu.Search)
			{
				ShowSubMenu(_searchTab);
				HideSubMenu(_detailsTab);
				_joinClanButton.gameObject.SetActive(true);
				_leaveClanButton.gameObject.SetActive(false);
				_removeClanButton.gameObject.SetActive(false);
				_chatButton.gameObject.SetActive(false);
			}
			else
			{
				ShowSubMenu(_detailsTab);
				HideSubMenu(_searchTab);
				_joinClanButton.gameObject.SetActive(state.UserClan == null && state.DisplayedClan != null);
				_leaveClanButton.gameObject.SetActive(state.UserClan != null);
				_removeClanButton.gameObject.SetActive(state.UserClan != null && state.UserClanRank == 0);
				_chatButton.gameObject.SetActive(state.UserClan != null);
			}
		}
		private async void StartChat(ClanMenuUIState state)
		{
			IChannel channel;
			try
			{
				channel = await _connection.Socket.JoinChatAsync(state.UserClan.Id, ChannelType.Group, persistence: true, hidden: true);
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Couldn't join chat " + e.Message);
				return;
			}
			_chatChannelClanUI.SetChatChannel(channel);
			_chatChannelClanUI.gameObject.SetActive(true);
		}
	}
}
