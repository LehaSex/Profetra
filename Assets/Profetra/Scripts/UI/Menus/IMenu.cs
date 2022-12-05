using System;
namespace Profetra
{
	public interface IMenu
	{
		bool IsShown { get; }
		void Show(bool isMuteButtonClick = false);
		void Hide(bool isMuteSoundManager = false);
		void SetBackButtonHandler(Action onBack);
	}
}
