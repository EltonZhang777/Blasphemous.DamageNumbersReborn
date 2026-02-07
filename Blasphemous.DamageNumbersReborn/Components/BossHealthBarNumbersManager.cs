using Blasphemous.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class BossHealthBarNumbersManager : NumbersManager
{
    internal List<BossHealthBarNumberObject> numbers;

    private int _poolSize;

    public static BossHealthBarNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _poolSize = Main.DamageNumbersReborn.config.BossBarTextTypeToConfig.Values.Select(x => x.poolSize).Max() * 3;
        numbers = new(_poolSize);
    }

    internal void AddHealthBarNumber(string entityId)
    {
        // check for duplicates
        BossHealthBarNumberObject existingNumber = numbers.FirstOrDefault(x => x.bossId == entityId);
        if (existingNumber != null)
            return;

        foreach (BossHealthBarNumberObject.TextType textType in Enum.GetValues(typeof(BossHealthBarNumberObject.TextType)))
        {
            if (!Main.DamageNumbersReborn.config.BossBarTextTypeToConfig[textType].enabled)
                continue;

            BossHealthBarNumberObject numberObject = UIObjectPoolManager.HighRes.ReuseObject(
                Prefab,
                Vector3.zero,
                Quaternion.identity,
                true,
                _poolSize).GameObject.GetComponent<BossHealthBarNumberObject>();
            numberObject.bossId = entityId;
            numberObject.textType = textType;
            numbers.Add(numberObject);
            numberObject.gameObject.SetActive(true);
        }
    }

    internal void RemoveHealthBarNumber(string entityId)
    {
        List<BossHealthBarNumberObject> targetNumbers = numbers.Where(x => x.bossId.Equals(entityId)).ToList();
        if ((targetNumbers == null) || (targetNumbers.Count == 0))
            return;

        targetNumbers.ForEach(x => RemoveHealthBarNumber(x));
    }

    internal void RemoveHealthBarNumber(BossHealthBarNumberObject number)
    {
        number.gameObject.SetActive(false);
        if (!numbers.Contains(number))
            return;
        numbers.Remove(number);
    }

    internal void RemoveAllHealthBarNumbers()
    {
        numbers.ForEach(x => RemoveHealthBarNumber(x));
    }

    private protected override GameObject CreatePrefab()
    {
        // Initialize transform and parent.
        GameObject result = new($"BossBarNumbers");
        result.transform.SetParent(UIModder.Parents.CanvasHighRes);
        result.AddComponent<BossHealthBarNumberObject>();

        // start prefab as inactive
        result.SetActive(false);
        return result;
    }
}
