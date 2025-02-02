using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectZ.InGame.Controls;
using ProjectZ.InGame.Interface;
using ProjectZ.InGame.Things;

namespace ProjectZ.InGame.Pages;

class ModsSettingsPage : InterfacePage
{
    private readonly InterfaceListLayout _bottomBar;

    public ModsSettingsPage(int width, int height)
    {
        // graphic settings layout
        var modsSettingsList = new InterfaceListLayout { Size = new Point(width, height), Selectable = true };
        var buttonWidth = 240;

        modsSettingsList.AddElement(new InterfaceLabel(Resources.GameHeaderFont, "settings_mods_header",
            new Point(buttonWidth, (int)(height * Values.MenuHeaderSize)), new Point(0, 0)));

        var contentLayout = new InterfaceListLayout { Size = new Point(width, (int)(height * Values.MenuContentSize)), Selectable = true, ContentAlignment = InterfaceElement.Gravities.Top };

        var toggleExtraDialog = InterfaceToggle.GetToggleButton(
            new Point(buttonWidth, 18),
            new Point(5, 2),
            "settings_mods_extra_dialogs",
            GameSettings.ExtraDialog,
            value =>
            {
                GameSettings.ExtraDialog = value;
                Game1.GameManager.Reload(); // Hack to reset dialogs on some items
            }
        );
        contentLayout.AddElement(toggleExtraDialog);

        var toggleBoostWalkSpeed = InterfaceToggle.GetToggleButton(
            new Point(buttonWidth, 18),
            new Point(5, 2),
            "settings_mods_boost_walk_speed",
            GameSettings.BoostWalkSpeed,
            value => GameSettings.BoostWalkSpeed = value
        );
        contentLayout.AddElement(toggleBoostWalkSpeed);

        modsSettingsList.AddElement(contentLayout);

        _bottomBar = new InterfaceListLayout() { Size = new Point(width, (int)(height * Values.MenuFooterSize)), Selectable = true, HorizontalMode = true };
        // back button
        _bottomBar.AddElement(new InterfaceButton(new Point(60, 20), new Point(2, 4), "settings_menu_back", element =>
        {
            Game1.UiPageManager.PopPage();
        }));

        modsSettingsList.AddElement(_bottomBar);

        PageLayout = modsSettingsList;
    }

    public override void Update(CButtons pressedButtons, GameTime gameTime)
    {
        base.Update(pressedButtons, gameTime);

        // close the page
        if (ControlHandler.ButtonPressed(CButtons.B))
            Game1.UiPageManager.PopPage();
    }

    public override void OnLoad(Dictionary<string, object> intent)
    {
        // the left button is always the first one selected
        _bottomBar.Deselect(false);
        _bottomBar.Select(InterfaceElement.Directions.Left, false);
        _bottomBar.Deselect(false);

        PageLayout.Deselect(false);
        PageLayout.Select(InterfaceElement.Directions.Top, false);
    }
}
