

using Nakama;

namespace Profetra
{
	public class ClanMenuUIState
	{
		public IApiGroup DisplayedClan { get; set; }
		public IApiGroup UserClan { get; set; }
		public int? UserClanRank { get; set; }
		public ClanSubMenu SubMenu { get; set; }
	}

	public enum ClanSubMenu
	{
		Search = 0,
		Details = 1
	}
}
