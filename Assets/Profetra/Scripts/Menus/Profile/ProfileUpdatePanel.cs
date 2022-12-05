using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ProfileUpdatePanel : Menu
	{
		[Space]
		[SerializeField] private Button _doneButton = null;
		[SerializeField] private InputField _usernameText = null;
		[SerializeField] private Image _avatarImage = null;
		[SerializeField] private Button _avatarButton = null;
		[SerializeField] private AvatarSprites _avatarSprites;
		private int _currentAvatarIndex;
		private GameConnection _connection;
		private string _deviceId;
		public void Init(GameConnection connection, string deviceId)
		{
			_deviceId = deviceId;
			_connection = connection;
			_backButton.onClick.AddListener(() =>
			{
				Hide();
			});
			_doneButton.onClick.AddListener(Done);
			IApiUser user = _connection.Account.User;
			_backButton.gameObject.SetActive(true);
		}
		private void Start()
		{
			_avatarButton.onClick.AddListener(ChangeAvatar);
		}
		public override void Show(bool isMuteButtonClick = false)
		{
			base.Show(isMuteButtonClick);
			_usernameText.text = _connection.Account.User.Username;
			_avatarImage.sprite = _avatarSprites.GetSpriteByName(_connection.Account.User.AvatarUrl);
		}
		private void ChangeAvatar()
		{
			int nextIndex = _currentAvatarIndex = (_currentAvatarIndex + 1) % _avatarSprites.Sprites.Length;
			_avatarImage.sprite = _avatarSprites.Sprites[nextIndex];
		}
		private async void Done()
		{
			try
			{
				PlayerPrefs.SetString(GameConstants.AuthTokenKey, _connection.Session.AuthToken);
				await _connection.Client.UpdateAccountAsync(_connection.Session, _usernameText.text, null);
				var account = await _connection.Client.GetAccountAsync(_connection.Session);
				_connection.Account = account;
			}
			catch (ApiResponseException e)
			{
				Debug.LogWarning("Error updating username: " + e.Message);
			}
			Hide();
		}
	}
}
