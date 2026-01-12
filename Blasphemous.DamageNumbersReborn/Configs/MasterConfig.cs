using Blasphemous.DamageNumbersReborn.Components;
using Framework.Managers;
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

    // WIP, need a new simpler config class
    public DamageNumberConfig enemyHealthBarNumbers = new()
    {
        outlineColor = "#ddc752",
    };

    public int precisionDigits = 3;

    public DamageElementColorConfig elementColors = new();

    /// <summary>
    /// If true, enemy health bar will always be shown regardless of whether Eye of Erudition is equipped.
    /// </summary>
    public bool alwaysShowEnemyHealthBar = false;

    internal static bool ShowingEnemyHealthBar => Core.Events.GetFlag("SHOW_ENEMY_BAR");
    internal static float ScreenWidthScale => Screen.width / 640f;
    internal static float ScreenHeightScale => Screen.height / 360f;
    internal static float GuiScale => (ScreenHeightScale > ScreenWidthScale) ? ScreenWidthScale : ScreenHeightScale;

    internal Dictionary<DamageArea.DamageElement, Color> DamageElementToColor => new()
    {
        { DamageArea.DamageElement.Normal, ParseHtmlToColorOrWhite(elementColors.physicalColor) },
        { DamageArea.DamageElement.Contact, ParseHtmlToColorOrWhite(elementColors.contactColor) },
        { DamageArea.DamageElement.Fire, ParseHtmlToColorOrWhite(elementColors.fireColor) },
        { DamageArea.DamageElement.Magic, ParseHtmlToColorOrWhite(elementColors.magicColor) },
        { DamageArea.DamageElement.Lightning, ParseHtmlToColorOrWhite(elementColors.lightningColor) },
        { DamageArea.DamageElement.Toxic, ParseHtmlToColorOrWhite(elementColors.toxicColor) },
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

    internal static Color ParseHtmlToColorOrWhite(string htmlColor)
    {
        return ColorUtility.TryParseHtmlString(htmlColor, out Color color)
            ? color
            : new Color(1, 1, 1, 1);
    }
}
