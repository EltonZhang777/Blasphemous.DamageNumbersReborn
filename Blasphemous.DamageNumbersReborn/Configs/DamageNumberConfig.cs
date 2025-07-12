using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;

/// <summary>
/// Contains config of one type of damage number.
/// </summary>
public class DamageNumberConfig
{
    public bool enabled = true;
    public int fontSize = 16;
    public string outlineColor = "#FFFFFF";
    public DamageNumberAnimationConfig animation = new();

    internal Color OutlineColor => ColorUtility.TryParseHtmlString(outlineColor, out Color color)
        ? color
        : new Color(1, 1, 1, 1);
}
