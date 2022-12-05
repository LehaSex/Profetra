using UnityEditor;
using UnityEngine;

namespace Profetra
{
	public class MenuItems
	{
		[MenuItem (GameConstants.MenuItem_OpenWebsite)]
		public static void MenuItem_OpenWebsite()
		{
			Application.OpenURL(GameConstants.m0kar_website);
		}


		[MenuItem(GameConstants.MenuItem_OpenDeveloperConsole)]
		public static void MenuItem_OpenDeveloperConsole()
		{
			Application.OpenURL(GameConstants.DeveloperConsoleUrl);
		}
		
	}
}
