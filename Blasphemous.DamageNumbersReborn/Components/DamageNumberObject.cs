using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class DamageNumberObject
{
    public Vector2 startingPosition;
    public Vector2 finalPosition;
    public float timePassed;
    public Hit hit;
    public float postMitigationDamage;
    public Entity damagedEntity;
    public EntityType damagedEntityType;

    internal GameObject gameObj;
    internal Vector2 currentPosition;

    public enum EntityType
    {
        Penitent,
        Enemy,
        Boss,
        Other
    }
}
