using System;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ClanCreationPanel : Menu
	{
		public event Action<IApiGroup> OnClanCreated;
		[SerializeField] private Text _clanName = null;
		[SerializeField] private Button _avatarButton = null;
		[SerializeField] private Image _avatarImage = null;
		[SerializeField] private Button _doneButton = null;
		[SerializeField] private AvatarSprites _avatarSprites = null;
		private int _currentAvatarIndex;
		private GameConnection _connection;
		public void Init(GameConnection connection)
		{
			_connection = connection;
			_doneButton.onClick.AddListener(() =>
			{
				Hide();
			});
		}
		private void Awake()
		{
			base.Hide(true);
			_backButton.onClick.AddListener(() =>
			{
				Hide();
			});
			_avatarButton.onClick.AddListener(ChangeAvatar);
			ChangeAvatar();
		}
		private void ChangeAvatar()
		{
			int nextIndex = _currentAvatarIndex = (_currentAvatarIndex + 1) % _avatarSprites.Sprites.Length;
			_avatarImage.sprite = _avatarSprites.Sprites[nextIndex];
		}
		private async void CreateClan()
		{
			string name = _clanName.text;
			try
			{
				IApiGroup group = await _connection.Client.CreateGroupAsync(_connection.Session, name, "A super great clan.", _avatarImage.name);
				if (OnClanCreated != null)
				{
					OnClanCreated(group);
				}
			}
			catch (ApiResponseException e)
			{
				Debug.LogError("Error creating clan: " + e.Message);
			}
		}
		public override void Hide(bool isMuteSoundManager = false)
		{
			CreateClan();
			base.Hide(isMuteSoundManager);
		}
	}
}
