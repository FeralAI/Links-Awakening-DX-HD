﻿using Microsoft.Xna.Framework;
using ProjectZ.Content.Fonts;
using ProjectZ.InGame.Controls;
using ProjectZ.InGame.Things;
using System;

namespace ProjectZ.InGame.SaveLoad;

class SettingsSaveLoad
{
    private static readonly string SettingsFileName = "settings";

    public static void LoadSettings()
    {
        var saveManager = new SaveManager();

        // error loading file
        if (!saveManager.LoadFile(SettingsFileName))
            return;

        Values.PathContentFolder = saveManager.GetString("ContentPath", Values.PathContentFolder);
        Values.PathSaveFolder = saveManager.GetString("SavePath", Values.PathSaveFolder);

        GameSettings.GameScale = saveManager.GetInt("GameScale", GameSettings.GameScale);
		GameSettings.GameScale = MathHelper.Clamp(GameSettings.GameScale, -1, 11);
        GameSettings.UiScale = saveManager.GetInt("UIScale", GameSettings.UiScale);
		GameSettings.UiScale = MathHelper.Clamp(GameSettings.UiScale, 0, 12);
        GameSettings.MusicVolume = saveManager.GetInt("MusicVolume", GameSettings.MusicVolume);
        GameSettings.EffectVolume = saveManager.GetInt("EffectVolume", GameSettings.EffectVolume);
        GameSettings.EnableShadows = saveManager.GetBool("EnableShadows", GameSettings.EnableShadows);
        GameSettings.Autosave = saveManager.GetBool("Autosave", GameSettings.Autosave);
        GameSettings.ExtraDialog = saveManager.GetBool("ExtraDialog", GameSettings.ExtraDialog);
        GameSettings.BoostWalkSpeed = saveManager.GetBool("BoostWalkSpeed", GameSettings.BoostWalkSpeed);

        string dialogFontNameStr = saveManager.GetString("DialogFontName", FontNames.SpriteFontName.smallFontOriginal.ToString());
        GameSettings.DialogFontName = Enum.Parse<FontNames.SpriteFontName>(dialogFontNameStr);

        GameSettings.SmoothCamera = saveManager.GetBool("SmoothCamera", GameSettings.SmoothCamera);
        GameSettings.BorderlessWindowed = saveManager.GetBool("BorderlessWindowed", GameSettings.BorderlessWindowed);
        GameSettings.IsFullscreen = saveManager.GetBool("IsFullscreen", GameSettings.IsFullscreen);
        GameSettings.LockFps = saveManager.GetBool("LockFPS", GameSettings.LockFps);

        Values.ControllerDeadzone = saveManager.GetFloat("ControllerDeadzone", Values.ControllerDeadzone);
        Game1.LanguageManager.CurrentLanguageIndex = saveManager.GetInt("CurrentLanguage", Game1.LanguageManager.CurrentLanguageIndex);

        ControlHandler.LoadButtonMap(saveManager);
    }

    public static void SaveSettings()
    {
        var saveManager = new SaveManager();

        saveManager.SetString("ContentPath", Values.PathContentFolder);
        saveManager.SetString("SavePath", Values.PathSaveFolder);

        saveManager.SetInt("Version", 1);
        saveManager.SetInt("GameScale", GameSettings.GameScale);
        saveManager.SetInt("UIScale", GameSettings.UiScale);
        saveManager.SetInt("MusicVolume", GameSettings.MusicVolume);
        saveManager.SetInt("EffectVolume", GameSettings.EffectVolume);
        saveManager.SetBool("EnableShadows", GameSettings.EnableShadows);
        saveManager.SetBool("Autosave", GameSettings.Autosave);
        saveManager.SetBool("ExtraDialog", GameSettings.ExtraDialog);
        saveManager.SetBool("BoostWalkSpeed", GameSettings.BoostWalkSpeed);

        saveManager.SetString("DialogFontName", GameSettings.DialogFontName.ToString());

        saveManager.SetBool("SmoothCamera", GameSettings.SmoothCamera);
        saveManager.SetBool("BorderlessWindowed", GameSettings.BorderlessWindowed);
        saveManager.SetBool("IsFullscreen", GameSettings.IsFullscreen);
        saveManager.SetBool("LockFPS", GameSettings.LockFps);

        saveManager.SetFloat("ControllerDeadzone", Values.ControllerDeadzone);
        saveManager.SetInt("CurrentLanguage", Game1.LanguageManager.CurrentLanguageIndex);

        ControlHandler.SaveButtonMaps(saveManager);

        saveManager.Save(SettingsFileName, Values.SaveRetries);
    }
}
