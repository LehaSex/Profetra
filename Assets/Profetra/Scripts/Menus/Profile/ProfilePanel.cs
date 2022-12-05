

using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Profetra
{

	public class ProfilePanel : MonoBehaviour
	{
		[SerializeField] private Button _befriendButton = null;


		[SerializeField] private Text _usernameText = null;


		[SerializeField] private Text _clanNameText = null;


		[SerializeField] private Text _statsText = null;


		[SerializeField] private Image _avatarImage = null;


		[SerializeField] private Button _profileUpdateButton = null;


		[SerializeField] private AvatarSprites _avatarSprites = null;

		private GameConnection _connection;

		public void Init(GameConnection connection, ProfileUpdatePanel updatePanel)
		{
			_connection = connection;
//			_profileUpdateButton.onClick.AddListener(() => updatePanel.Show());
		}


		public async void ShowAsync(string userId)
		{
			try
			{
				IApiUsers results = await _connection.Client.GetUsersAsync(_connection.Session, new string[] { userId });
				if (results.Users.Count() != 0)
				{
					Show(results.Users.ElementAt(0));
				}
				else
				{
					Debug.LogWarning("Couldn't find user with id: " + userId);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogWarning("An error has occured while retrieving user info: " + e);
			}
		}

		public async void Show(IApiUser user)
		{
			PopulateDataAsync(user);
			await SetUIAccessAsync(user);
		}

		private async Task SetUIAccessAsync(IApiUser user)
		{
			IApiUser localUser = _connection.Account.User;
			bool isFriend = await CanBeFriendAsync(user);
			_avatarImage.sprite = _avatarSprites.GetSpriteByName(user.AvatarUrl);
			_profileUpdateButton.gameObject.SetActive(user.Id == localUser.Id);
			_befriendButton.gameObject.SetActive(isFriend);
		}

		private async Task<bool> CanBeFriendAsync(IApiUser user)
		{
			if (user.Id == _connection.Session.UserId)
			{
				return false;
			}

			var friends = await _connection.Client.ListFriendsAsync(_connection.Session);

			if (friends == null)
			{
				Debug.LogError("Couldn't retrieve friends list");
				return false;
			}
			foreach (IApiFriend friend in friends.Friends)
			{
				if (friend.User.Id == user.Id)
				{
					return false;
				}
			}
			return true;
		}


		private void OnAccountUpdated()
		{
			IApiUser user = _connection.Account.User;
			_usernameText.text = user.Username;
			_avatarImage.sprite = _avatarSprites.GetSpriteByName(user.AvatarUrl);
		}

		private async void PopulateDataAsync(IApiUser user)
		{
			StorageObjectId personalStorageId = new StorageObjectId();
			personalStorageId.Collection = "personal";
			personalStorageId.UserId = _connection.Session.UserId;
			personalStorageId.Key = "player_data";

			IApiStorageObjects personalStorageObjects = await _connection.Client.ReadStorageObjectsAsync(_connection.Session, personalStorageId);

			PlayerData playerData = new PlayerData();
			IUserGroupListUserGroup clan = null;

			try
			{
				IApiUserGroupList clanList = await _connection.Client.ListUserGroupsAsync(_connection.Session);
				// user should only be in one clan.
				clan = clanList.UserGroups.Any() ? clanList.UserGroups.First() : null;
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Error fetching user clans " + e.Message);
			}

			//CardCollection cardCollection = null;

			try
			{
				var response = await _connection.Client.RpcAsync(_connection.Session, "load_user_cards", "");
				//cardCollection = response.Payload.FromJson<CardCollection>();
			}
			catch (ApiResponseException e)
			{
				throw e;
			}

			_usernameText.text = user.Username;
			_statsText.text = GenerateStats(playerData).TrimEnd();

			_clanNameText.text = clan == null ?
			"<i><color=#b0b0b0>[Not a clan member yet]</color></i>" :
			clan.Group.Name;

/* 			List<string> deckIds = cardCollection.GetDeckList();
			for (int i = 0; i < deckIds.Count; i++)
			{
				Card card = cardCollection.GetDeckCard(deckIds[i]);
				_cardSlots[i].SetCard(card);
			} */
		}

		private string GenerateStats(PlayerData data)
		{
			return
				"Level: \t" + data.level + System.Environment.NewLine +
				"Wins: \t" + data.wins + System.Environment.NewLine +
				"Games:\t " + data.gamesPlayed + System.Environment.NewLine;
		}
	}
}
