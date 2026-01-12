using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;

/// <summary>
/// Contains config of one type of damage number.
/// </summary>
public class DamageNumberConfig
{
    public bool enabled = true;
    public string fontName = "MajesticExtended_Pixel_Scroll";
    public int fontSize = 16;
    public string outlineColor = "#FFFFFF";
    public DamageNumberAnimationConfig animation = new();

    internal Color OutlineColor => MasterConfig.ParseHtmlToColorOrWhite(outlineColor);
}
