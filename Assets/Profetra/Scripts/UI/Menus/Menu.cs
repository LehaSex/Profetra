using Profetra.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class Menu : MonoBehaviour, IMenu
	{
		[SerializeField] private CanvasGroup _canvasGroup = null;
		[SerializeField] protected Button _backButton = null;
		public bool IsShown { get; protected set; }
		[ContextMenu("Show")]
		public virtual void Show(bool isMuteButtonClick = false)
		{
			if (!isMuteButtonClick)
			{
				SoundManager.Instance.PlayButtonClick();
			}
			_canvasGroup.alpha = 1;
			_canvasGroup.blocksRaycasts = true;
			IsShown = true;
		}
		[ContextMenu("Hide")]
		public virtual void Hide(bool isMuteButtonClick = false)
		{
			if (!isMuteButtonClick)
			{
				SoundManager.Instance.PlayButtonClick();
			}
			_canvasGroup.alpha = 0;
			_canvasGroup.blocksRaycasts = false;
			IsShown = false;
		}
		public virtual void SetBackButtonHandler(Action onBack)
		{
			_backButton?.onClick.AddListener(() =>
			{
				SoundManager.Instance.PlayButtonClick();
				onBack();
			});
		}
	}
}
