using System;
using UnityEngine;
namespace Profetra
{
	public class ProfileMenuUI : Menu
	{
		[SerializeField] private ProfilePanel _profilePanel = null;
		private GameConnection _connection;
		private void Awake()
		{
			_backButton.onClick.AddListener(() => Hide());
		}
		public void Init(GameConnection connection, ProfileUpdatePanel updatePanel)
		{
			_connection = connection;
			_profilePanel.Init(connection, updatePanel);
		}
		public override void Show(bool isMuteButtonClick = false)
		{
			try
			{
				_profilePanel.Show(_connection.Account.User);
				base.Show(isMuteButtonClick);
			}
			catch (Exception e)
			{
				Debug.LogError("Невозможно показать profile menu UI: " + e.Message);
			}
		}
	}
}
