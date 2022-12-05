namespace Profetra
{
	/// <summary>
	/// Значения, используемые в проекте.
	/// </summary>
	public static class GameConstants
	{

		// CreateAssetMenu
		private const string CreateAssetMenu = "Profetra/";
		public const string CreateAssetMenu_AvatarSprites = CreateAssetMenu + "AvatarSprites";
		public const string CreateAssetMenu_Card = CreateAssetMenu + "Card";
		public const string CreateAssetMenu_CardList = CreateAssetMenu + "Card List";
		public const string CreateAssetMenu_GameConfiguration = CreateAssetMenu + "GameConfiguration";
		public const string CreateAssetMenu_GameConnection = CreateAssetMenu + "GameConnection";

		// MenuItem
		private const string MenuItem = "Window/Profetra/";
		public const string MenuItem_OpenWebsite = MenuItem + "Open MrVester's Website";
		public const string MenuItem_OpenDeveloperConsole = MenuItem + "Open Developer Console";

		// PlayerPrefs
		public const string DeviceIdKey = "nakama.deviceId";
		public static string AuthTokenKey = "nakama.authToken";
		public static string RefreshTokenKey = "nakama.refreshToken";

		// Urls
		public const string m0kar_website = "https://mrvester.games/";
		public const string DeveloperConsoleUrl = "http://192.168.1.10:7351/";


	}
}
