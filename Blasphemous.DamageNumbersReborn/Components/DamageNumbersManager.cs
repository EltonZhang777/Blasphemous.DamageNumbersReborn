using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.Framework.UI;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class DamageNumbersManager : NumbersManager
{
    internal List<DamageNumberObject> damageNumbers;
    internal CyclicalPositioningHandler penitentCyclicalXPosition;
    internal CyclicalPositioningHandler enemyCyclicalXPosition;

    public static DamageNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
        damageNumbers = new(_poolSize);
        penitentCyclicalXPosition = new(
            Main.DamageNumbersReborn.config.penitentDamageNumbers.cyclicalMovementPeriod,
            Main.DamageNumbersReborn.config.penitentDamageNumbers.cyclicalXRange);
        enemyCyclicalXPosition = new(
            Main.DamageNumbersReborn.config.enemyDamageNumbers.cyclicalMovementPeriod,
            Main.DamageNumbersReborn.config.enemyDamageNumbers.cyclicalXRange);
    }

    public void AddDamageNumber(Hit hit, Entity entity)
    {
        // Determine the damaged entity type for the damage number
        DamageNumberObject.EntityType entityType;
        if (entity is Penitent)
        {
            entityType = DamageNumberObject.EntityType.Penitent;
        }
        else if (entity is Enemy)
        {
            if (entity.Id.StartsWith("BS"))
            {
                entityType = DamageNumberObject.EntityType.Boss;
            }
            else
            {
                entityType = DamageNumberObject.EntityType.Enemy;
            }
        }
        else
        {
            entityType = DamageNumberObject.EntityType.Other;
        }

        // Determine config type based on entity type
        DamageNumberConfig currentConfig = Main.DamageNumbersReborn.config.EntityTypeToConfig[entityType];

        // set starting position
        Vector3 entityPosition = entity.GetComponentInChildren<DamageArea>().TopCenter;
        // apply random offset
        Vector2 randomOffset = new(
            Random.Range(currentConfig.randomXRange.X, currentConfig.randomXRange.Y),
            Random.Range(currentConfig.randomYRange.X, currentConfig.randomYRange.Y));
        // apply cyclical x offset
        float cyclicalXOffset = entityType == DamageNumberObject.EntityType.Penitent
            ? penitentCyclicalXPosition.GetNextCyclicalOffset()
            : enemyCyclicalXPosition.GetNextCyclicalOffset();
        // finalize starting and final position
        Vector2 startingPosition = (Vector2)entityPosition
            + randomOffset
            + new Vector2(cyclicalXOffset, 0)
            + currentConfig.labelWorldPositionOffset;
        Vector2 finalPosition = startingPosition + currentConfig.animation.totalMovement;

        // calculate post-mitigation damage
        float postMitigationDamage = Mathf.Max(entity.GetReducedDamage(hit) - entity.Stats.Defense.Final, 0f);

        DamageNumberObject result = UIObjectPoolManager.HighRes.ReuseObject(
            Prefab,
            startingPosition,
            Quaternion.identity,
            true,
            _poolSize).GameObject.GetComponent<DamageNumberObject>();
        result.hit = hit;
        result.postMitigationDamage = postMitigationDamage;
        result.startingPosition = startingPosition;
        result.finalPosition = finalPosition;
        result.damagedEntity = entity;
        result.damagedEntityType = entityType;
        result.currentPosition = result.startingPosition;
        damageNumbers.Add(result);
        result.gameObject.SetActive(true);
    }

    public void RemoveDamageNumber(DamageNumberObject damageNumber)
    {
        damageNumbers.Remove(damageNumber);
    }

    /// <summary>
    /// Create the prefab of damage numbers that instances will instantitate upon.
    /// </summary>
    private protected override GameObject CreatePrefab()
    {
        GameObject result = new($"DamageNumber");
        result.transform.SetParent(UIModder.Parents.CanvasHighRes);
        result.AddComponent<DamageNumberObject>();

        return result;
    }
}
