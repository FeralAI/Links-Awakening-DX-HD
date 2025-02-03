# Links Awakening DX HD

This is a personal fork of [Link's Awakening DX HD](https://linksawakeningdxhd.itch.io/links-awakening-dx-hd).

Check the [wiki](https://github.com/ihm-tswow/Links-Awakening-DX-HD/wiki) for a brief documentation of the in-game tools.

## Build

**Prerequisites**

- (Very) Basic knowledge of C# and Visual Studio
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
    - Make sure to select `.NET desktop development` components in the visual studio installer.
- .NET 6.0 SDK

**Build Instructions**

- Clone this repository
- Open ProjectZ.sln
- Build/run like any normal C# program
    - To create an optimized release build, run the `publish.bat`, or the Publish action in Visual Studio, to output a `Publish` folder.

## Changes

### New Mods menu!

<img src="Assets/menu_settings_mods.png" style="width: 400px" alt="Modifications Menu" title="Modifications Menu">

#### Show Extra Dialog

Show or hide the Piece of Power / Guardian Acorn / Heavy Object dialogs that persistently pop up.

Inspired by the the [QoL Patch](https://www.romhacking.net/hacks/3597/) for the GBC DX version.

#### Boost Walk Speed

Set Link's base speed to 1.25x normal. Makes map traversal feel better without overdoing it.

### Font Selection!

<img src="Assets/menu_settings_game.png" style="width: 400px" alt="Game Settings Menu" title="Game Settings Menu">

Available in the Game Settings menu, choose between the original font or a monospace version for more clarity in menus and dialogs.

The monospace version was in the original release, but went unused.

### Various Other Tweaks

Several other enchancements and fixes are included:

* Enable UI scaling option and allow higher manual max value (same value as "Auto" setting)

## Credits

TODO
