using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.Framework.UI;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class EnemyHealthBarNumbersManager : NumbersManager
{
    internal List<EnemyHealthBarNumberObject> numbers;
    
    private protected EnemyHealthBarNumberConfig Config => Main.DamageNumbersReborn.config.enemyHealthBarNumbers;

    public static EnemyHealthBarNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
        numbers = new(Config.poolSize);
    }

    internal void AddHealthBarNumber(EnemyHealthBar bar)
    {
        //// check for duplicates
        //EnemyHealthBarNumberObject existingNumber = numbers.FirstOrDefault(x => x.healthBar == bar);
        //if (existingNumber != null)
        //    return;

        EnemyHealthBarNumberObject result = UIObjectPoolManager.HighRes.ReuseObject(
            Prefab,
            Vector3.zero,
            Quaternion.identity,
            true,
            Config.poolSize).GameObject.GetComponent<EnemyHealthBarNumberObject>();

        result.healthBar = bar;
        numbers.Add(result);
    }

    internal void RemoveHealthBarNumber(EnemyHealthBar bar, bool force = false)
    {
        EnemyHealthBarNumberObject number = numbers.FirstOrDefault(x => x.healthBar == bar);
        if (number == null)
            return;

        RemoveHealthBarNumber(number, force);
    }

    internal void RemoveHealthBarNumber(EnemyHealthBarNumberObject number, bool force = false)
    {
        if (!numbers.Contains(number))
            return;
        numbers.Remove(number);

        if (force)
        {
            number.KillSelf();
            return;
        }

        if (number.animationState == EnemyHealthBarNumberObject.AnimationState.FadingOut)
            return;
        number.FadeOutAndKillSelf();
    }

    internal void RemoveAllHealthBarNumbers(bool force = false)
    {
        numbers.ForEach(x => RemoveHealthBarNumber(x, force));
    }

    private protected override GameObject CreatePrefab()
    {
        // Initialize transform and parent.
        GameObject result = new($"HealthBarNumbers");
        result.transform.SetParent(UIModder.Parents.CanvasHighRes);
        result.AddComponent<EnemyHealthBarNumberObject>();

        // start prefab as inactive
        result.SetActive(false);
        return result;
    }
}
