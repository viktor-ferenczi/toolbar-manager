# Toolbar Manager

Toolbar Manager plugin for [Space Engineers 1](https://www.spaceengineersgame.com/).

Please consider supporting my work on [Patreon](https://www.patreon.com/semods) or one time via [PayPal](https://www.paypal.com/paypalme/vferenczi/).

- [SE Mods Discord](https://discord.gg/PYPFPGf3Ca) FAQ, Troubleshooting, Support, Bug Reports, Discussion
- [Pulsar Discord](https://discord.gg/z8ZczP2YZY) Everything about plugins
- [YouTube Channel](https://www.youtube.com/@couldntfindafreename)
- [Source code](https://github.com/viktor-ferenczi/se-toolbar-manager)
- [Bug reports](https://discord.gg/KWzDu683zs)

## Installation

1. Exit from Space Engineers
2. Install [Pulsar](https://github.com/SpaceGT/Pulsar)
3. Start Space Engineers
4. Open the **Plugins** menu (it should show up in the Main Menu)
5. Add the **Toolbar Manager** plugin to your list (`+` icon)
6. Apply the change and let the game restart

After enabling the plugin it will be active for all single- and multiplayer worlds.

*Enjoy!*

## Features

This plugin allows for the saving, loading and convenient editing of character and block
toolbars. For example, you can quickly load a "standard" toolbar for your character into 
any game (including multiplayer ones), which can greatly speed up your builds by using 
toolbar slots you have already memorized. It can also be used to fix the broken toolbars of
your ships in survival based on the profiles you saved in your creative design world earlier.

This plugin works for both offline and online multiplayer games without
the need for any server side support. Toolbar profiles are saved locally,
nothing is stored on any server.

### Editing toolbars

This plugin adds a new "Staging" area in the G menu, which allows for the convenient reordering
of toolbar items, including moving them between toolbar pages.

The staging areas are preserved for the character and each block with a toolbar in-memory during
gameplay, but **not saved** over sessions (world loads) and game restarts. Please save your
toolbars into profiles after editing.

You can disable the staging area in the plugin's configuration, should it cause any issues.

### Saving/loading toolbars (profiles)

The **G menu** has a new TM buttons at the bottom left to manage saved toolbar profiles.

**New** saves the currently open toolbar as a new profile.

**Load** overwrites your currently open toolbar from the selected profile.

**Merge** loads only the used slots from the profile and leaves the rest of them as is.

You can also **Rename** and **Delete** toolbar profiles.

#### Profile folders (for backup)

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

You can change the hotkey in the plugin's configuration.

### Configuration

All key features of this plugin are configurable. You can change any of the configuration options without having to restart or reload anything. Please see below how to access the plugin's configuration dialog.

#### From the main menu

Double-click on the **Toolbar Manager** plugin in Pulsar's **Plugins** dialog, which will open the plugin's detailed information dialog. Use the **Settings** button in that dialog.

#### During game play

If you are in a menu (Terminal, etc.), then close that. Press `Ctrl-Alt-/` to open the **Configure a plugin** dialog. Choose the **Toolbar Manager** plugin.

## Credits

### Patreon Supporters

_in alphabetical order_

#### Admiral level

- BetaMark
- Casinost
- Mordith - Guardians SE
- Robot10
- wafoxxx

#### Captain level

- Diggz
- jiringgot
- Jimbo
- Kam Solastor
- lazul
- Linux123123
- Lotan
- Lurking StarCpt
- NeonDrip
- NeVaR
- opesoorry

### Testers

- Avaness
- mkaito

### Creators

- avaness - Plugin Loader
- Fred XVI - Racing maps
- Kamikaze - M&M mod
- LTP
- Mordith - Guardians SE
- Mike Dude - Guardians SE
- SwiftyTech - Stargate Dimensions

**Thank you very much for all your support and testing effort!**