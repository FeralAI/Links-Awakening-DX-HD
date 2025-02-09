﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectZ.InGame.Things;

public enum LanguageCode
{
    eng = 0,
    spa = 1,
    por = 2,
    ita = 3,
    ukr = 4,
    rus = 5,
}

public class Language
{
    public Dictionary<string, string> Strings => _languageStrings[CurrentLanguageIndex];

    private Dictionary<string, string>[] _languageStrings;

    public int CurrentLanguageIndex;

    public void Load()
    {
        // go through the .lng files and fill the _languageStrings dictionary array
        var files = Directory.GetFiles(Values.PathLanguageFolder);

        var languageStrings = new Dictionary<string, Dictionary<string, string>>
        {
            // the default (first) entry is english
            { "eng", [] },
            { "spa", [] },
            { "por", [] },
            { "ita", [] },
            { "ukr", [] },
            //{ "rus", [] },
        };

        for (var i = 0; i < files.Length; i++)
        {
            var extension = Path.GetExtension(files[i]);
            if (extension == ".lng")
            {
                var fileName = Path.GetFileNameWithoutExtension(files[i]);
                var split = fileName.Split('_');
                var lngName = "";

                // eng.lng
                if (split.Length == 1)
                    lngName = split[0];
                // dialog_eng.lng
                if (split.Length == 2)
                    lngName = split[1];

                languageStrings.TryGetValue(lngName, out var dict);

                if (dict == null)
                {
                    //dict = [];
                    //languageStrings.Add(lngName, dict);

                    // Ignore language files that aren't defined to prevent error when changing setting
                    continue;
                }

                if (split.Length == 1 || (split.Length == 2 && split[0] == "dialog"))
                    LoadFile(dict, files[i]);
            }
        }

        _languageStrings = languageStrings.Values.ToArray();
        CurrentLanguageIndex = Math.Clamp(CurrentLanguageIndex, 0, _languageStrings.Length - 1);
    }

    public void LoadFile(Dictionary<string, string> dictionary, string fileName)
    {
        var reader = new StreamReader(fileName);

        while (!reader.EndOfStream)
        {
            var strLine = reader.ReadLine();
            var spacePosition = strLine.IndexOf(' ');

            if (spacePosition < 0 || strLine.StartsWith("//"))
                continue;

            var strKey = strLine.Substring(0, spacePosition);

            // empty string
            if (spacePosition + 1 >= strLine.Length)
            {
                dictionary.Add(strKey, "");
                continue;
            }

            var strValue = strLine.Substring(spacePosition + 1);

            dictionary.Add(strKey, strValue);
        }

        reader.Close();
    }

    public string GetString(string strKey, string defaultString)
    {
        if (strKey == null)
            return "null";

        if (Strings.ContainsKey(strKey))
            defaultString = Strings[strKey];

        // use the english text if there is no translation
        else if (_languageStrings[0].ContainsKey(strKey))
            defaultString = _languageStrings[0][strKey];

        return defaultString;
    }

    public void ToggleLanguage()
    {
        CurrentLanguageIndex = (CurrentLanguageIndex + 1) % _languageStrings.Length;
    }
}
