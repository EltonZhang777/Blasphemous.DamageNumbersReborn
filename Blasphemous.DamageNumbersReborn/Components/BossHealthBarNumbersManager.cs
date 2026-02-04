using Blasphemous.Framework.UI;
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
        _poolSize = Main.DamageNumbersReborn.config.TextTypeToConfig.Values.Select(x => x.poolSize).Max() * 3;
        numbers = new(_poolSize);
    }

    internal void AddHealthBarNumber(string entityId)
    {
        // check for duplicates
        BossHealthBarNumberObject existingNumber = numbers.FirstOrDefault(x => x.bossId == entityId);
        if (existingNumber != null)
            return;

        if (Main.DamageNumbersReborn.config.TextTypeToConfig[BossHealthBarNumberObject.TextType.Percentage].enabled)
        {
            BossHealthBarNumberObject percentageNumber = UIObjectPoolManager.HighRes.ReuseObject(
                Prefab,
                Vector3.zero,
                Quaternion.identity,
                true,
                _poolSize).GameObject.GetComponent<BossHealthBarNumberObject>();
            percentageNumber.bossId = entityId;
            percentageNumber.textType = BossHealthBarNumberObject.TextType.Percentage;
            numbers.Add(percentageNumber);
            percentageNumber.gameObject.SetActive(true);
        }

        if (Main.DamageNumbersReborn.config.TextTypeToConfig[BossHealthBarNumberObject.TextType.Details].enabled)
        {
            BossHealthBarNumberObject detailsNumber = UIObjectPoolManager.HighRes.ReuseObject(
                Prefab,
                Vector3.zero,
                Quaternion.identity,
                true,
                _poolSize).GameObject.GetComponent<BossHealthBarNumberObject>();
            detailsNumber.bossId = entityId;
            detailsNumber.textType = BossHealthBarNumberObject.TextType.Details;
            numbers.Add(detailsNumber);
            detailsNumber.gameObject.SetActive(true);
        }

        if (Main.DamageNumbersReborn.config.TextTypeToConfig[BossHealthBarNumberObject.TextType.RecentlyLost].enabled)
        {
            BossHealthBarNumberObject recentlyLostNumber = UIObjectPoolManager.HighRes.ReuseObject(
                Prefab,
                Vector3.zero,
                Quaternion.identity,
                true,
                _poolSize).GameObject.GetComponent<BossHealthBarNumberObject>();
            recentlyLostNumber.bossId = entityId;
            recentlyLostNumber.textType = BossHealthBarNumberObject.TextType.RecentlyLost;
            numbers.Add(recentlyLostNumber);
            recentlyLostNumber.gameObject.SetActive(true);
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
