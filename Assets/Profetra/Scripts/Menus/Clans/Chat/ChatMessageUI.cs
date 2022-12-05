using System;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ChatMessageUI : MonoBehaviour
	{
		public string ContentTextValue
		{
			get
			{
				return _contentText.text;
			}
		}
		public string Id
		{
			get
			{
				return _messageId;
			}
		}
		[Header("UI Elements")]
		[SerializeField] private Text _usernameText = null;
		[SerializeField] private Text _dateText = null;
		[SerializeField] private Text _contentText = null;
		private string _messageId;
		private const string DATE_FORMAT = "M.d.yy H:mm";
		public void InitMessage(string messageId, string username, string content, string date, bool hideUsername)
		{
			_messageId = messageId;
			if (hideUsername)
			{
				_usernameText.enabled = false;
			}
			else
			{
				_usernameText.text = username;
			}
			DateTime dateTime;
			if (DateTime.TryParse(date, out dateTime))
			{
				_dateText.text = dateTime.ToString(DATE_FORMAT);
			}
			else
			{
				_dateText.text = "?.?.?? ?:??";
			}
			_contentText.text = content;
		}
	}
}
