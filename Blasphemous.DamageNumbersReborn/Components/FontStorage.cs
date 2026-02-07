using Blasphemous.DamageNumbersReborn.Configs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

internal static class FontStorage
{
    internal static readonly string defaultFontName = "MajesticExtended_Pixel_Scroll";
    private static Dictionary<string, Font> _fonts = new();

    /// <summary>
    /// Gets the font by name, or the default font if not found. 
    /// First queries through the internal storage, then query I2LocManager if not found. 
    /// If found, store it in the internal storage if hasn't already.
    /// </summary>
    internal static Font GetFont(string fontName)
    {
        _fonts.TryGetValue(fontName, out Font result);
        // isn't in the internal storage, try to find in I2Loc storage
        result ??= I2LocManager.FindAsset(fontName) as Font;
        // isn't in the I2Loc storage either, fall back to default font
        result ??= I2LocManager.FindAsset(defaultFontName) as Font;

        // store font
        StoreFont(fontName, result);

        return result;
    }

    internal static void StoreFont(string fontName, Font font, bool force = false)
    {
        if (_fonts.ContainsKey(fontName) && !force)
            return;

        _fonts[fontName] = font;
    }

    internal static Font GetDefaultFont()
    {
        return GetFont(defaultFontName);
    }

    internal static void LoadAllFontsFromConfig()
    {
        List<string> allFontNamesInConfig = Main.DamageNumbersReborn.config
            .GetType().GetFields((BindingFlags.Public | BindingFlags.Instance))
            .Where(x => x.FieldType == typeof(DamageNumberConfig))
            .Select(x => ((DamageNumberConfig)x.GetValue(Main.DamageNumbersReborn.config)).fontName).ToList();
        allFontNamesInConfig.Add(defaultFontName);
        foreach (string fontName in allFontNamesInConfig.Distinct())
        {
            GetFont(fontName);
        }
    }
}
