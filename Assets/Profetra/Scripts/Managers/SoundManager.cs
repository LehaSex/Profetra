using System.Collections.Generic;
using UnityEngine;

namespace Profetra.Managers
{

	public static class SoundConstants
	{
		public const string ButtonClick01 = "ButtonClick01";
		public const string CardDrop01 = "CardDrop01";
	}


	public class SoundManager : Singleton<SoundManager>
	{


		[SerializeField]
		private GameConfiguration _gameConfiguration = null;

		[SerializeField]
		private List<AudioClip> _audioClips = new List<AudioClip>();

		[SerializeField]
		private List<AudioSource> _audioSources = new List<AudioSource>();

		public void PlayAudioClip(string audioClipName)
		{
			foreach (AudioClip audioClip in _audioClips)
			{
				if (audioClip.name == audioClipName)
				{
					PlayAudioClip(audioClip);
					return;
				}
			}
		}

		public void PlayAudioClip(AudioClip audioClip)
		{
			if (!_gameConfiguration.IsAudioEnabled)
			{
				return;
			}

			foreach (AudioSource audioSource in _audioSources)
			{
				if (!audioSource.isPlaying)
				{
					audioSource.volume = _gameConfiguration.AudioVolume;
					audioSource.clip = audioClip;
					audioSource.Play();
					return;
				}
			}
		}

		public void PlayButtonClick()
		{
			PlayAudioClip(SoundConstants.ButtonClick01);
		}

/* 		protected void Start()
		{
			GameObject.DontDestroyOnLoad(gameObject);
		} */
	}
}
