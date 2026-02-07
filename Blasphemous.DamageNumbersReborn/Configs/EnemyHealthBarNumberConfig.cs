using Blasphemous.DamageNumbersReborn.Components;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;
public class EnemyHealthBarNumberConfig : NumberConfig
{
    public string textColor = "#000000";

    /// <summary>
    /// Offset of world position of the label from the damaged entity to show the damage number at.
    /// </summary>
    public SerializableVector3 labelWorldPositionOffset = new(-1.5f, 0.15f);

    /// <summary>
    /// X and Y distance of text outline to the main text.
    /// </summary>
    public SerializableVector3 outlineDistance = new(0.6f, 0.8f);

    public EnemyHealthBarNumberAnimationConfig animation = new();

    internal Color TextColor => MasterConfig.ParseHtmlToColorOrWhite(textColor);
}
