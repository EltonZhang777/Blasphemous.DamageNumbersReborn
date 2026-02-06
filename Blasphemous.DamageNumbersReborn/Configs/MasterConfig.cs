using Blasphemous.DamageNumbersReborn.Components;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;

public class MasterConfig
{
    /// <summary>
    /// Decimal digits of damage numbers
    /// </summary>
    public int damageNumberPrecisionDigits = 3;

    /// <summary>
    /// If true, enemy health bar will always be shown regardless of whether Eye of Erudition is equipped.
    /// </summary>
    public bool alwaysShowEnemyHealthBar = false;

    /// <summary>
    /// If true, only one flask sprite will be shown by the penitent UI
    /// </summary>
    public bool briefFlasksDisplay = false;

    public DamageNumberConfig enemyDamageNumbers = new()
    {
        outlineColor = "#000000"
    };

    public DamageNumberConfig penitentDamageNumbers = new()
    {
        outlineColor = "#900000"
    };

    public EnemyHealthBarNumberConfig enemyHealthBarNumbers = new()
    {
        outlineColor = "#ddc752",
        textColor = "#d00b0d",
        fontSize = 12,
        labelWorldPositionOffset = new(-1.5f, 0.15f),
        outlineDistance = new(0.6f, 0.8f)
    };

    public UIBarNumberConfig bossHealthBarDetailedNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#d00b0d",
        fontSize = 13,
        labelWorldPositionOffset = new(17.2f, 5f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 5,
    };

    public UIBarNumberConfig bossHealthBarPercentageNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#d00b0d",
        fontSize = 13,
        labelWorldPositionOffset = new(18.6f, 5.0f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 5,
    };

    public UIBarNumberConfig bossHealthBarRecentlyLostNumber = new()
    {
        outlineColor = "#000000",
        textColor = "#ffffff",
        fontSize = 13,
        labelWorldPositionOffset = new(18.6f, 5.7f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 5,
    };

    public UIBarNumberConfig penitentHealthBarDetailedNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#d00b0d",
        fontSize = 13,
        labelWorldPositionOffset = new(17.7f, 7.41f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 2,
    };

    public UIBarNumberConfig penitentHealthBarPercentageNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#d00b0d",
        fontSize = 13,
        labelWorldPositionOffset = new(19f, 7.41f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 2,
    };

    public UIBarNumberConfig penitentFervourBarDetailedNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#0960c1",
        fontSize = 13,
        labelWorldPositionOffset = new(17.7f, 6.42f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 2,
    };

    public UIBarNumberConfig penitentFervourBarPercentageNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#0960c1",
        fontSize = 13,
        labelWorldPositionOffset = new(19f, 6.42f),
        outlineDistance = new(0.6f, 0.8f),
        poolSize = 2,
    };

    public UIBarNumberConfig penitentFlaskDetailsVanillaNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#ffffff",
        fontSize = 13,
        labelWorldPositionOffset = new(19.0f, 5.5f),
        outlineDistance = new(0.6f, 0.8f),
    };

    public UIBarNumberConfig penitentFlaskDetailsBriefNumber = new()
    {
        outlineColor = "#ddc752",
        textColor = "#ffffff",
        fontSize = 13,
        labelWorldPositionOffset = new(19f, 6f),
        outlineDistance = new(0.6f, 0.8f),
    };

    public DamageElementColorConfig elementColors = new();

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
    internal Dictionary<BossHealthBarNumberObject.TextType, UIBarNumberConfig> BossBarTextTypeToConfig => new()
    {
        { BossHealthBarNumberObject.TextType.Percentage, bossHealthBarPercentageNumber },
        { BossHealthBarNumberObject.TextType.Details, bossHealthBarDetailedNumber },
        { BossHealthBarNumberObject.TextType.RecentlyLost, bossHealthBarRecentlyLostNumber },
    };
    internal Dictionary<PenitentBarNumberObject.TextType, UIBarNumberConfig> PenitentBarTextTypeToConfig => new()
    {
        { PenitentBarNumberObject.TextType.HealthPercentage, penitentHealthBarPercentageNumber },
        { PenitentBarNumberObject.TextType.HealthDetails, penitentHealthBarDetailedNumber },
        { PenitentBarNumberObject.TextType.FervourPercentage, penitentFervourBarPercentageNumber },
        { PenitentBarNumberObject.TextType.FervourDetails, penitentFervourBarDetailedNumber },
        { PenitentBarNumberObject.TextType.FlaskDetailsVanilla, penitentFlaskDetailsVanillaNumber },
        { PenitentBarNumberObject.TextType.FlaskDetailsBrief, penitentFlaskDetailsBriefNumber },
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
        return NumberStringFormatted(number, damageNumberPrecisionDigits);
    }

    internal static Color ParseHtmlToColorOrWhite(string htmlColor)
    {
        return ColorUtility.TryParseHtmlString(htmlColor, out Color color)
            ? color
            : new Color(1, 1, 1, 1);
    }
}
