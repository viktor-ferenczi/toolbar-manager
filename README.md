# Toolbar Manager

Toolbar Manager for the Space Engineers game.

For support please [join the SE Mods Discord](https://discord.gg/PYPFPGf3Ca).

Please consider [supporting my work on Patreon](https://www.patreon.com/semods).

## Features

This plugin allows for saving, loading and managing character and block
toolbars. For example you can  quickly load a "standard" toolbar for
your character into any game (including multiplayer ones), which can
greatly speed up your builds by using toolbar slots you have already
memorized. It can also be used to fix the broken toolbars of your
ships in survival based on the profiles you saved in your creative
design world earlier.

This plugin works for both offline and online multiplayer games without
the need for any server side support. Toolbar profiles are saved locally,
nothing is stored on any server.

### Toolbars

The **G menu** has a new TM buttons at the bottom left to manage saved
toolbar profiles.

**New** saves the currently open toolbar as a new profile.

**Load** overwrites your currently open toolbar from the selected profile. 

**Merge** loads only the used slots from the profile and leaves the rest of them as is.

You can also **Rename** and **Delete** toolbar profiles.

### Profile folders

Saved toolbars are stored as XML files in folders under:
`%AppData%\Roaming\SpaceEngineers\ToolbarManager`

There are subdirectories for each toolbar type. Toolbars within the same type are compatible,
however they may not have the same amount of slots available in-game. For example, it is
possible to save the toolbar from any Cockpit and load the profile into a Remote Control.

Up to 6 backups of each profile is kept, even after deleting the profile. Renaming profiles
also renames the backup files. Orphaned backup files (with the main profile deleted) are
cleaned up after 90 days, but only when a profile is deleted the next time.

### Quick block selection

Press the `BACKSLASH` or `PIPE` key (on English keyboard) to open the quick 
block search menu. It works the same way as the original menu, but the search 
rules are optimized to find blocks primarily by capital letters and digits:

- `AB` **Armor Blocks**
- `PB` **Programmable Block**
- `SK` **Survival Kit**
- `AT` **Atmospheric Thrusters**
- `LHT` **Large Hydrogen Thrusters**
- `211` **All 2x1x1 armor blocks**

Adding the subsequent lower case letters after an upper case one allows 
for narrowing down in case of ambiguity:

- `PH` **Parachute Hatch** and **Point Hand**
- `PHat` **Parachute Hatch** only

The order of characters must match exactly. Lower case characters and
space are skipped after a matching upper chase character or digit. 

Separating multiple search patterns by space is not supported, currently.

### Planned features
- Multiplying the available toolbar pages using new hotkeys and saved toolbars
- Restricting the load/merge operations to certain toolbar pages
- Moving and swapping toolbar items using mouse drag&drop
- Moving toolbar items between toolbar pages
- Swapping toolbar pages

Please join the Discord (see below) and tell me which one you would use, 
so they can be fast-tracked.  

## Installation
1. Exit from Space Engineers
2. Install the [Plugin Loader](https://github.com/sepluginloader/SpaceEngineersLauncher)
3. Start Space Engineers
4. Open the Plugins menu (should be in the Main Menu)
5. Enable the Toolbar Manager plugin
6. Save and let the game restart

After enabling the plugin it will be active for all single- and multiplayer worlds.

*Enjoy!*

## Want to know more?
- [SE Mods Discord](https://discord.gg/PYPFPGf3Ca)
- [Plugin Loader Discord](https://discord.gg/6ETGRU3CzR)
- [YouTube Channel](https://www.youtube.com/channel/UCc5ar3cW9qoOgdBb1FM_rxQ)
- [Source code](https://github.com/viktor-ferenczi/toolbar-manager)
- [Bug reports](https://discord.gg/x3Z8Ug5YkQ)

## Credits

### Patreon Supporters

#### Admiral level
- BetaMark
- Mordith - Guardians SE
- Robot10
- Casinost
- wafoxxx

#### Captain level
- Diggz
- lazul
- jiringgot
- Kam Solastor
- NeonDrip
- NeVaR
- opesoorry
- NeVaR
- Jimbo
- Lotan

#### Testers
- Avaness
- mkaito

### Creators
- avaness - Plugin Loader, Racing Display
- Mordith - Guardians SE
- Mike Dude - Guardians SE
- SwiftyTech - Stargate Dimensions
- Fred XVI - Racing maps
- Kamikaze - M&M mod
- LTP

**Thank you very much for all your support and hard work on testing!**