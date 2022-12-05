using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ChatServerMessageUI : MonoBehaviour
	{
		[SerializeField] private Text _contentText = null;
		public void Init(string content)
		{
			_contentText.text = content;
		}
	}
}
