using System;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class NotificationPopup : Menu
	{
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _descriptionText = null;
		[SerializeField] private Button _dismissButton = null;
		[Serializable]
		private class Reward
		{
			public int reward = 0;
		}
		public void Init(GameConnection connection)
		{
			connection.Socket.ReceivedNotification += NotificationReceived;
		}
		private void Awake()
		{
			_dismissButton.onClick.AddListener(() => base.Hide());
		}
		private void NotificationReceived(IApiNotification e)
		{
			if (e.Code == (int)NotificationCode.Quest_NewFriend)
			{
				Reward reward = JsonUtility.FromJson<Reward>(e.Content);
				base.Show();
				_titleText.text = e.Subject;
				_descriptionText.text = "Получена награда: " + reward.reward;
			}
		}
		private void NotifyQuestComplete(IApiNotification e)
		{
			Reward reward = JsonUtility.FromJson<Reward>(e.Content);
			base.Show();
			_titleText.text = e.Subject;
			_descriptionText.text = "Получена награда: " + reward.reward;
		}
	}
}
