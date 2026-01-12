using Blasphemous.DamageNumbersReborn.Components;
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

    /// <summary>
    /// Offset of world position of the label from the damaged entity to show the damage number at.
    /// </summary>
    public SerializableVector3 labelWorldPositionOffset = new(0f, 1f);

    /// <summary>
    /// X and Y distance of text outline to the main text.
    /// </summary>
    public SerializableVector3 outlineDistance = new(1f, 1f);

    /// <summary>
    /// Number of horizontal positions that different damage numbers can be shown cyclically.
    /// </summary>
    public int cyclicalMovementPeriod = 3;

    /// <summary>
    /// The range of cyclical X-axis offset of damage numbers.
    /// </summary>
    public SerializableVector3 cyclicalXRange = new(-0.8f, 0.8f);
    /// <summary>
    /// The range of random X-axis offset of damage numbers.
    /// </summary>
    public SerializableVector3 randomXRange = new(-0.2f, 0.2f);
    /// <summary>
    /// The range of random Y-axis offset of damage numbers.
    /// </summary>
    public SerializableVector3 randomYRange = new(0f, 0f);

    public DamageNumberAnimationConfig animation = new();

    internal Color OutlineColor => MasterConfig.ParseHtmlToColorOrWhite(outlineColor);
}
