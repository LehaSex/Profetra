using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class UsernameTip : MonoBehaviour
	{
		[SerializeField] private Button _tipButton = null;
		[SerializeField] private Text _usernameText = null;
		private UsernameSearcher _searcher;
		public void Init(string username, UsernameSearcher searcher)
		{
			_tipButton.onClick.AddListener(Select);
			_searcher = searcher;
			_usernameText.text = username;
		}
		private void Select()
		{
			_searcher.SetSearcherText(_usernameText.text);
		}
	}
}
