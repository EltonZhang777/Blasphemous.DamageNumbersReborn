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
    public EnemyHealthBarNumberConfig enemyHealthBarNumbers = new()
    {
        outlineColor = "#ddc752",
        textColor = "#d00b0d",
        fontSize = 12,
        labelWorldPositionOffset = new(-1.5f, 0.15f),
        outlineDistance = new(0.6f, 0.8f)
    };

    public DamageElementColorConfig elementColors = new();

    public int precisionDigits = 3;

    /// <summary>
    /// If true, enemy health bar will always be shown regardless of whether Eye of Erudition is equipped.
    /// </summary>
    public bool alwaysShowEnemyHealthBar = false;

    internal static bool ShowingEnemyHealthBar => Core.Events.GetFlag("SHOW_ENEMY_BAR");
    internal static float ScreenWidthScale => Screen.width / NumbersManager.Camera.pixelWidth;
    internal static float ScreenHeightScale => Screen.height / NumbersManager.Camera.pixelHeight;
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
