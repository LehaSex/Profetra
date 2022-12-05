using System;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ClanUserEntry : MonoBehaviour
	{
		[SerializeField] private Text _usernameText = null;
		[SerializeField] private Image _rankImage = null;
		[Space]
		[SerializeField] private Animator _animator = null;
		[SerializeField] private Button _panelButton = null;
		[SerializeField] private Button _kickButton = null;
		[SerializeField] private Button _promoteButton = null;
		[SerializeField] private Button _profileButton = null;
		private bool _isShown;
		private string _userId;
		[Space]
		[SerializeField] private Sprite _adminRankSprite = null;
		[SerializeField] private Sprite _superadminRankSprite = null;
		public void Init(string userId)
		{
			_userId = userId;
		}
		public void SetUser(IApiUser user, int userState, int localState, Action<ClanUserEntry> onSelected,
			Action<IApiUser> onKick, Action<IApiUser> onPromote, Action<IApiUser> onShowProfile)
		{
			_usernameText.text = user.Username;
			_rankImage.sprite = GetRankSprite(userState);
			_rankImage.gameObject.SetActive(_rankImage.sprite != null);
			if (user.Id == _userId)
			{
				_usernameText.color = Color.green;
				_kickButton.gameObject.SetActive(false);
				_promoteButton.gameObject.SetActive(false);
			}

			else if (!CanManageUser(localState, userState))
			{
				_kickButton.gameObject.SetActive(false);
				_promoteButton.gameObject.SetActive(false);
			}
			else
			{
				_kickButton.onClick.AddListener(() => onKick?.Invoke(user));
				_promoteButton.onClick.AddListener(() => onPromote?.Invoke(user));
			}
			_profileButton.onClick.AddListener(() => onShowProfile?.Invoke(user));
			_panelButton.onClick.AddListener(() => onSelected(this));
		}
		public void ShowInteractionPanel()
		{
			if (!_isShown)
			{
				_isShown = true;
				_animator.SetTrigger("Open");
			}
		}
		public void HideInteractionPanel()
		{
			if (_isShown)
			{
				_isShown = false;
				_animator.SetTrigger("Close");
			}
		}
		private Sprite GetRankSprite(int userState)
		{
			switch (userState)
			{
				case 0: return _superadminRankSprite;
				case 1: return _adminRankSprite;
				default: return null;
			}
		}
		private bool CanManageUser(int localState, int managedUser)
		{
			switch (localState)
			{
				case 0: // superadmin
					return true;
				case 1: // admin
					return
						managedUser == 2 ||
						managedUser == 3;
				case 2: // member
					return false;
				case 3: // invite
					return false;
				default:
					return false;
			}
		}
	}
}
