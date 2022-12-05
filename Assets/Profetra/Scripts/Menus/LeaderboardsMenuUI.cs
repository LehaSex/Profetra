using System;
using System.Collections.Generic;
using System.Linq;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class LeaderboardsMenuUI : Menu
	{
		[SerializeField] private RectTransform _userList = null;
		[SerializeField] private LeaderboardEntry _leaderboardEntryPrefab = null;
		[SerializeField] private int _recordsPerPage = 100;
		[SerializeField] private Button _showGlobal = null;
		[SerializeField] private Button _showClan = null;
		[SerializeField] private Button _showFriends = null;
		[SerializeField] private Button _nextPageButton = null;
		[SerializeField] private Button _prevPageButton = null;
		private Action _nextPage;
		private Action _prevPage;
		private GameConnection _connection;
		private ProfilePopup _profilePopup;
		private IUserGroupListUserGroup _userClan;
		private void Awake()
		{
			_showClan.onClick.AddListener(() => ShowClanLeaderboards(null));
			_showGlobal.onClick.AddListener(() => ShowGlobalLeaderboards(null));
			_showFriends.onClick.AddListener(() => ShowFriendsLeaderboards(null));
			_backButton.onClick.AddListener(() => Hide());
		}
		public void Init(GameConnection connection, ProfilePopup profilePopup)
		{
			_connection = connection;
			_profilePopup = profilePopup;
		}
		public async void ShowGlobalLeaderboards(string cursor = null)
		{
			IApiLeaderboardRecordList records = await _connection.Client.ListLeaderboardRecordsAsync(_connection.Session, "global", ownerIds: null, expiry: null, _recordsPerPage, cursor);
			SetLeaderboardsCursor(records, ShowGlobalLeaderboards);
			FillLeaderboard(records.Records);
			_showFriends.interactable = true;
			_showClan.interactable = true;
			_showGlobal.interactable = false;
		}
		public async void ShowClanLeaderboards(string cursor)
		{
			if (_userClan == null)
			{
				return;
			}
			var users = await _connection.Client.ListGroupUsersAsync(_connection.Session, _userClan.Group.Id, null, 1, null);
			IEnumerable<string> ids = users.GroupUsers.Select(x => x.User.Id);
			IApiLeaderboardRecordList list = await _connection.Client.ListLeaderboardRecordsAsync(_connection.Session, "global", ids, null, 1, cursor);
			if (list.Records != null)
			{
				SetLeaderboardsCursor(list, ShowClanLeaderboards);
				FillLeaderboard(list.OwnerRecords);
				_showFriends.interactable = true;
				_showClan.interactable = false;
				_showGlobal.interactable = true;
			}
		}
		public async void ShowFriendsLeaderboards(string cursor)
		{
			try
			{
				var friends = await _connection.Client.ListFriendsAsync(_connection.Session);
				List<string> ids = friends.Friends.Select(x => x.User.Id).ToList();
				ids.Add(_connection.Session.UserId);
				IApiLeaderboardRecordList records = await _connection.Client.ListLeaderboardRecordsAsync(_connection.Session, "global", ids, null, 1, cursor);
				SetLeaderboardsCursor(records, ShowFriendsLeaderboards);
				FillLeaderboard(records.OwnerRecords);
				_showFriends.interactable = false;
				_showClan.interactable = true;
				_showGlobal.interactable = true;
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka liderbord druzia: " + e.Message);
			}
		}
		private void SetLeaderboardsCursor(IApiLeaderboardRecordList records, Action<string> caller)
		{
			if (records.PrevCursor != null)
			{
				_prevPageButton.interactable = true;
				_prevPageButton.onClick.RemoveAllListeners();
				_prevPageButton.onClick.AddListener(() => caller(records.PrevCursor));
			}
			else
			{
				_prevPageButton.interactable = false;
			}
			if (records.NextCursor != null)
			{
				_nextPageButton.interactable = true;
				_nextPageButton.onClick.RemoveAllListeners();
				_nextPageButton.onClick.AddListener(() => caller(records.NextCursor));
			}
			else
			{
				_nextPageButton.interactable = false;
			}
		}
		private void FillLeaderboard(IEnumerable<IApiLeaderboardRecord> recordList)
		{
			foreach (Transform entry in _userList)
			{
				Destroy(entry.gameObject);
			}
			int rank = 1;
			string localId = _connection.Account.User.Id;
			foreach (IApiLeaderboardRecord record in recordList)
			{
				LeaderboardEntry entry = Instantiate(_leaderboardEntryPrefab, _userList);
				string username = record.Username;
				if (localId == record.OwnerId)
				{
					username += " (You)";
				}
				entry.SetPlayer(username, rank, record.Score, () => OnProfileClicked(record.OwnerId));
				rank += 1;
			}
		}
		private void OnProfileClicked(string userId)
		{
			_profilePopup.Show(userId);
		}
		public async override void Show(bool isMuteButtonClick = false)
		{
			IApiUserGroupList clanList = null;
			try
			{
				clanList = await _connection.Client.ListUserGroupsAsync(_connection.Session);
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Oshibka s liderbordom klana: " + e.Message);
				return;
			}
			_userClan = clanList.UserGroups.FirstOrDefault();
			if (_userClan != null)
			{
				_showClan.gameObject.SetActive(true);
			}
			else
			{
				if (_showClan.interactable)
				{
					ShowGlobalLeaderboards();
				}
				_showClan.gameObject.SetActive(false);
			}
			base.Show(isMuteButtonClick);
		}
	}
}
