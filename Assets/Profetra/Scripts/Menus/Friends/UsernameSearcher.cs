using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Profetra
{
	public class UsernameSearcher : MonoBehaviour
	{
		private struct SearchResult
		{
			public string username;
		}
		public event Action OnSubmit = delegate { };
		public string InputFieldValue
		{
			get
			{
				return _inputField.text;
			}
		}
		[SerializeField] private GameObject _usernameTipPrefab = null;
		[Header("UI elements")]
		[SerializeField] private InputField _inputField = null;
		[SerializeField] private RectTransform _usernameTipsParent = null;
		private List<UsernameTip> _tips = new List<UsernameTip>();
		private GameConnection _connection;
		private void Start()
		{
			_inputField.onValueChanged.AddListener(SearchUsers);
			_inputField.onEndEdit.AddListener(SearchEnded);
			_inputField.onEndEdit.AddListener(Submit);
		}
		public void Init(GameConnection connection)
		{
			_connection = connection;
		}
		public void SetSearcherText(string username)
		{
			_inputField.onValueChanged.RemoveListener(SearchUsers);
			_inputField.text = username;
			_inputField.onValueChanged.AddListener(SearchUsers);
			DeleteAllTips();
		}
		public void Submit(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (Input.GetKeyDown(KeyCode.Return))
				{
					OnSubmit();
				}
			}
		}
		private async void SearchUsers(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			var searchRequest = new Dictionary<string, string>() { { "username", text } };
			string payload = searchRequest.ToJson();
			string rpcid = "search_username";
			Task<IApiRpc> searchTask;
			try
			{
				searchTask = _connection.Client.RpcAsync(_connection.Session, rpcid, payload);
			}
			catch (Exception e)
			{
				Debug.LogError("Could not search users (" + e.Message + ")");
				return;
			}
			IApiRpc searchResult = await searchTask;
			SearchResult[] usernames = searchResult.Payload.FromJson<SearchResult[]>();
			DeleteAllTips();
			for (int i = 0; i < 5 && i < usernames.Length; i++)
			{
				if (!string.IsNullOrEmpty(usernames[i].username))
				{
					CreateTip(usernames[i].username);
				}
			}
		}
		private void CreateTip(string username)
		{
			GameObject go = Instantiate(_usernameTipPrefab, _usernameTipsParent);
			UsernameTip tip = go.GetComponent<UsernameTip>();
			if (tip)
			{
				tip.Init(username, this);
				_tips.Add(tip);
			}
			else
			{
				Debug.LogError("Invalid username tip prefab!");
				Destroy(go);
			}
		}
		private void SearchEnded(string value)
		{
			Rect tipParentRect = _usernameTipsParent.rect;
			tipParentRect.position += (Vector2)_usernameTipsParent.position;
			if (!tipParentRect.Contains(Input.mousePosition))
			{
				DeleteAllTips();
			}
		}
		private void DeleteAllTips()
		{
			for (int i = 0; i < _tips.Count; i++)
			{
				Destroy(_tips[i].gameObject, 0.1f);
			}
			_tips.Clear();
		}
	}
}
