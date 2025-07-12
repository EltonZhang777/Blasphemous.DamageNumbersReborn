using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class DamageNumberObject
{
    public Vector2 originalPosition;
    public float screenY;
    public float timePassed;
    public Hit hit;
    public float postMitigationDamage;
    public Entity damagedEntity;
    public EntityType damagedEntityType;

    internal GameObject gameObj;

    public enum EntityType
    {
        Penitent,
        Enemy,
        Boss,
        Other
    }
}
