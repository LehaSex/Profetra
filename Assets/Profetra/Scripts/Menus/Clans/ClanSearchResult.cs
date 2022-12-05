using System;
using Nakama;
using UnityEngine;
using UnityEngine.UI;
namespace Profetra
{
	public class ClanSearchResult : MonoBehaviour
	{
		[SerializeField] private Text _clanName = null;
		[SerializeField] private Image _clanAvatar = null;
		[SerializeField] private Button _joinClanButton = null;
		[SerializeField] private AvatarSprites _avatarSprites = null;
		public void SetClan(IApiGroup clan, Action<IApiGroup> onJoin)
		{
			_clanName.text = clan.Name;
			_joinClanButton.onClick.AddListener(() => onJoin(clan));
			Sprite avatar = _avatarSprites.GetSpriteByName(clan.AvatarUrl);
			_clanAvatar.sprite = avatar;
		}
	}
}
