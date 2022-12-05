using System.Linq;
using UnityEngine;
namespace Profetra
{
	[CreateAssetMenu(menuName = GameConstants.CreateAssetMenu_AvatarSprites)]
	public class AvatarSprites : ScriptableObject
	{
		public Sprite[] Sprites => _sprites;
		[SerializeField] private Sprite[] _sprites;
		public Sprite GetSpriteByName(string name)
		{
			return _sprites.FirstOrDefault<Sprite>(sprite => sprite.name == name) ?? _sprites.First(); ;
		}
	}
}
