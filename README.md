# The "Unbeatable Song Hack"

Most likely the first mod menu for the Unbeatable DEMO.

## Installation

To use, download BepInEx 5 ( https://github.com/BepInEx/BepInEx ) and put a .dll of this mod into a `BepInEx/plugins/UnbeatableSongHack` folder (you will probably have to create that folder yourself). When installed, a GUI should be accessible in-game.

## Custom Maps

To add custom maps to the game, download maps (for example off Taco's unbeatable beatmap server) and put them into a `CustomMaps` folder, which is located in the same directory as the game executable (If it's not there you might have to create the server yourself).

You can then play them in the Arcade Mode. Filter by the "local" category in the Arcade to find all local maps easily.

---

### Developing/Building this

If you want to work on/build this, you should most likely change the `<GamePath>` property in the `UnbeatableSongHack.csproj` file by changing it to your game path (Example: `Y:/.../Unbeatable Demo`).
