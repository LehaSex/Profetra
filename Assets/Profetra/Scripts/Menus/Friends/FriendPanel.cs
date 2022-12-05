using UnityEngine.UI;
using UnityEngine;
using Nakama;
using System;
namespace Profetra
{
	public class FriendPanel : MonoBehaviour
	{
		public event Action<FriendPanel> OnSelected = delegate { };
		public event Action OnDataChanged = delegate { };
		public event Action<string, string> OnChatStartButtonClicked = delegate { };
		[Header("UI elements")]
		[Header("Buttons")]
		[SerializeField] private Button _friendButton = null;
		[SerializeField] private Button _removeFriendButton = null;
		[SerializeField] private Button _blockFriendButton = null;
		[SerializeField] private Button _startChatButton = null;
		[SerializeField] private Button _acceptInviteButton = null;
		[Header("Buttons texts and Icons")]
		[SerializeField] private Text _nicknameText = null;
		[SerializeField] private Text _blockFriendButtonText = null;
		[SerializeField] private Text _removeFriendButtonText = null;
		[Header("Animations")]
		[SerializeField] private Animator _animator = null;
		[SerializeField] private RectTransform _bottomPanel = null;
		private IApiFriend _friend;
		private bool _panelOpened = false;
		private bool _blocked;
		private GameConnection _connection;
		public void Init(GameConnection connection, IApiFriend friend)
		{
			_connection = connection;
			_friend = friend;
			_nicknameText.text = friend.User.Username;
			_friendButton.onClick.AddListener(TogglePanel);
			_friendButton.onClick.AddListener(OnSelected_Handler);
			_removeFriendButton.onClick.AddListener(RemoveThisFriend);
			_blockFriendButton.onClick.AddListener(BlockOrUnblockThisFriend);
			_acceptInviteButton.onClick.AddListener(AcceptInviteFromThisFriend);
			_startChatButton.onClick.AddListener(StartChatWithThisFriend);
			ActualizeFriendState();
		}
		public void Deselect(bool closeImmediately = false)
		{
			if (_panelOpened)
			{
				if (closeImmediately)
				{
					ClosePanelImmediately();
				}
				ClosePanel();
			}
		}
		private void ActualizeFriendState()
		{
			switch (_friend.State)
			{
				case 0: break;
				case 1: SetInvitedState(); break;
				case 2: SetInvitingState(); break;
				case 3: SetBlockedState(); break;
			}
		}
		private void OnSelected_Handler()
		{
			if (OnSelected != null)
			{
				OnSelected(this);
			}
		}
		private async void AcceptInviteFromThisFriend()
		{
			try
			{
				string[] ids = new[] { _friend.User.Id };
				await _connection.Client.AddFriendsAsync(_connection.Session, ids);
				OnDataChanged();
			}
			catch (Exception e) 
			{
				Debug.LogError("Adding friend failed (" + e.Message + ")");
			}
		}
		private async void RemoveThisFriend()
		{
			try
			{
				string[] ids = new[] { _friend.User.Id };
				await _connection.Client.DeleteFriendsAsync(_connection.Session, ids);
				OnDataChanged();
			}
			catch (Exception e) 
			{
				Debug.LogError("Removing friend failed (" + e.Message + ")");
			}
		}
		private async void BlockOrUnblockThisFriend()
		{
			if (!_blocked)
			{
				string[] ids = new[] { _friend.User.Id };
				try
				{
					await _connection.Client.BlockFriendsAsync(_connection.Session, ids);
					OnDataChanged();
				}
				catch (Exception e)
				{
					Debug.LogError("Blocking friend failed (" + e.Message + ")");
				}
			}
			else
			{
				await _connection.Client.DeleteFriendsAsync(_connection.Session, new[] { _friend.User.Id });
			}
		}
		private void StartChatWithThisFriend()
		{
			OnChatStartButtonClicked(_friend.User.Id, _friend.User.Username);
		}
		private void SetBlockedState()
		{
			_removeFriendButton.gameObject.SetActive(true);
			_removeFriendButtonText.text = "разблок";
			_blockFriendButtonText.text = "Разблокировать";
			_blocked = true;
		}
		private void SetInvitedState()
		{
			_removeFriendButton.gameObject.SetActive(true);
			_removeFriendButtonText.text = "убрать";
		}
		private void SetInvitingState()
		{
			_removeFriendButton.gameObject.SetActive(true);
			_removeFriendButtonText.text = "отказаться";
			_acceptInviteButton.gameObject.SetActive(true);
		}
		private void TogglePanel()
		{
			if (_panelOpened)
			{
				ClosePanel();
			}
			else
			{
				OpenPanel();
			}
		}
		private void OpenPanel()
		{
			_animator.SetTrigger("Open");
			_panelOpened = true;
		}
		private void ClosePanel()
		{
			_animator.SetTrigger("Close");
			_panelOpened = false;
		}
		public void ClosePanelImmediately()
		{
			_bottomPanel.sizeDelta = new Vector2(_bottomPanel.sizeDelta.x, 0);
			_panelOpened = false;
		}
	}
}
