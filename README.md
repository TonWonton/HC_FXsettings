## Prerequisites
BepisPlugins for HC

https://github.com/IllusionMods/BepisPlugins


BepInEx Unity Il2Cpp (plugin only tested on 6.0.0-be674 and be680)

https://github.com/BepInEx/BepInEx


BepInEx.ConfigurationManager Il2Cpp

https://github.com/BepInEx/BepInEx.ConfigurationManager

BepInEx.ConfigurationManager Il2Cpp v18.1 required to avoid crashes and problems with clicking

Delete old configuration manager from plugin folder if you have older version installed


All included in HF-patch

https://github.com/ManlyMarco/HC-HF_Patch

## Known issues
- MSAA quality set to 0 by default in plugin instead of 2 (game default), if your game has aliasing or looks pixelated, set MSAA quality to 2 or higher
- Increasing shadow res higher than the default (4096) might make clothes and shadows on characters flicker
- v1.3.0 by default has a bit too much bloom/flares due to settings that were added, and those settings were not "active" before, reduce bloom settings to fix
- Some settings might not work in both DigitalCraft and the plugin menu since they both edit the same values. Check both settings if something doesn't work, and try to enable/change settings there.
- If some settings don't have effect in main game, check in game settings if it is enabled there too

## Installation
1. Install correct versions of BepInEx Unity IL2CPP and BepInEx.ConfigurationManager
2. Download from releases
3. Extract into game folder

## Description
Originally cloned from BepInEx Utility IL2CPP GraphicsSettingsIL2CPP_net6

Edit Unity and Beautify post processing settings in HoneyCome through BepInEx.ConfigurationManager
