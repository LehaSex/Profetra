# 🎮Profetra
```php

   ▄███████▄    ▄████████  ▄██████▄     ▄████████    ▄████████     ███        ▄████████    ▄████████ 
  ███    ███   ███    ███ ███    ███   ███    ███   ███    ███ ▀█████████▄   ███    ███   ███    ███ 
  ███    ███   ███    ███ ███    ███   ███    █▀    ███    █▀     ▀███▀▀██   ███    ███   ███    ███ 
  ███    ███  ▄███▄▄▄▄██▀ ███    ███  ▄███▄▄▄      ▄███▄▄▄         ███   ▀  ▄███▄▄▄▄██▀   ███    ███ 
▀█████████▀  ▀▀███▀▀▀▀▀   ███    ███ ▀▀███▀▀▀     ▀▀███▀▀▀         ███     ▀▀███▀▀▀▀▀   ▀███████████ 
  ███        ▀███████████ ███    ███   ███          ███    █▄      ███     ▀███████████   ███    ███ 
  ███          ███    ███ ███    ███   ███          ███    ███     ███       ███    ███   ███    ███ 
 ▄████▀        ███    ███  ▀██████▀    ███          ██████████    ▄████▀     ███    ███   ███    █▀  
               ███    ███                                                    ███    ███              
```
## 📋Introduction
This is the task for the module "Software project management".
## ✅Requirements
- ☑️ Install [Nakama](https://heroiclabs.com/docs/install-docker-quickstart/)
- ☑️ Install [Unity](https://unity3d.com/get-unity/download)
- ☑️ Install [Visual Studio Code](https://code.visualstudio.com/download)

## 🔧Installation
1. Clone the repository
```bash
git clone https://github.com/LehaSex/Profetra.git
```
2. Change parameters in the file /Scripts/GameConnection.cs
```csharp
		[SerializeField] private string _protocol = "http";
		[SerializeField] private string _ipAddress = "192.168.1.10";
		[SerializeField] private int _port = 7350;
		[SerializeField] private string _serverKey = "defaultkey";
```
3. Run game

## 📝Description of scripts
- GameConfiguration.cs - script for storing game settings
- GameConnection.cs - script for connecting to the server
- GameConstants.cs - script for storing constants (Mainly for editor)
- Scene01MainMenuController.cs - script for the main menu (This script controls the scene with the main menu, and also connects to the server when entering the game)
- /Editor/Nakama/Profetra/MenuItems.cs - script for adding menu items to the editor
- /Managers/GameConfigurationManager.cs - script for managing game settings (Singleton)
- /Managers/SoundManager.cs - script for managing sounds (Singleton)
- /Menus/* - scripts for managing actions in menus
- /UI/* - scripts for managing UI elements
- /Utils/Singleton.cs - script for creating a singleton
- /Utils/TerrainBob.cs - script for animate terrain (This script is used to animate the terrain in the main menu)