using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class LoadingMenu : Menu
	{
		[Space]
		[SerializeField] private GameObject _connectingPanel = null;
		[SerializeField] private GameObject _loadingIcon = null;
		[SerializeField] private float rotationSpeed = -90;
		[Space]
		[SerializeField] private GameObject _retryPanel = null;
		[SerializeField] private Button _retryButton = null;
		private GameConnection _connection;
		public void Init(GameConnection connection)
		{
			_connection = connection;
		}
		private void Update()
		{
			if (base.IsShown)
			{
				_loadingIcon.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
			}
		}
		public override void Show(bool isMuteButtonClick = false)
		{
			base.Show(isMuteButtonClick);
			_retryButton.onClick.AddListener(Retry);
			AwaitConnection();
		}
		public void AwaitConnection()
		{
			_connectingPanel.SetActive(true);
			_retryPanel.SetActive(false);
		}
		private void ConnectionFailed()
		{
			_connectingPanel.SetActive(false);
			_retryPanel.SetActive(true);
		}
		private void Retry()
		{
			_connectingPanel.SetActive(true);
			_retryPanel.SetActive(false);
		}
	}
}
