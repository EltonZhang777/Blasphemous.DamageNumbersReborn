using Blasphemous.DamageNumbersReborn.Components;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;

public class MasterConfig
{
    public DamageNumberConfig enemyDamageNumbers = new()
    {
        outlineColor = "#000000"
    };

    public DamageNumberConfig penitentDamageNumbers = new()
    {
        outlineColor = "#900000"
    };

    public int precisionDigits = 3;

    public DamageElementColorConfig elementColors = new();

    internal Dictionary<DamageArea.DamageElement, Color> DamageElementToColor => new()
    {
        { DamageArea.DamageElement.Normal, ParseHtmlToColorOrDefault(elementColors.physicalColor) },
        { DamageArea.DamageElement.Contact, ParseHtmlToColorOrDefault(elementColors.contactColor) },
        { DamageArea.DamageElement.Fire, ParseHtmlToColorOrDefault(elementColors.fireColor) },
        { DamageArea.DamageElement.Magic, ParseHtmlToColorOrDefault(elementColors.magicColor) },
        { DamageArea.DamageElement.Lightning, ParseHtmlToColorOrDefault(elementColors.lightningColor) },
        { DamageArea.DamageElement.Toxic, ParseHtmlToColorOrDefault(elementColors.toxicColor) },
    };

    internal Dictionary<DamageNumberObject.EntityType, DamageNumberConfig> EntityTypeToConfig => new()
    {
        { DamageNumberObject.EntityType.Penitent, penitentDamageNumbers },
        { DamageNumberObject.EntityType.Enemy, enemyDamageNumbers },
        { DamageNumberObject.EntityType.Boss, enemyDamageNumbers },
        { DamageNumberObject.EntityType.Other, enemyDamageNumbers }
    };

    internal static string NumberStringFormatted(float number, int precision)
    {
        precision = Mathf.Max(precision, 0);
        if (precision == 0)
        {
            return Mathf.RoundToInt(number).ToString();
        }
        else
        {
            return number.ToString($"F{precision}").TrimEnd('0').TrimEnd('.');
        }
    }

    internal string NumberStringFormatted(float number)
    {
        return NumberStringFormatted(number, precisionDigits);
    }

    internal static Color ParseHtmlToColorOrDefault(string htmlColor)
    {
        if (!ColorUtility.TryParseHtmlString(htmlColor, out Color result))
        {
            return new Color(1f, 1f, 1f, 1f); // Default to white if parsing fails
        }
        return result;
    }
}
