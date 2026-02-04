using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Extensions;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(UIController))]
class UIController_DisplayBossHealthBarNumbers_Patches
{
    [HarmonyPatch("ShowBossHealth")]
    [HarmonyPostfix]
    public static void AddBossNumber(
        Entity entity)
    {
        BossHealthBarNumbersManager.Instance.AddHealthBarNumber(entity.Id);
    }

    [HarmonyPatch("HideBossHealth")]
    [HarmonyPrefix]
    public static void RemoveBossNumber(
        EnemyHealthBar __instance)
    {
        BossHealthBarNumbersManager.Instance.RemoveAllHealthBarNumbers();
    }
}

[HarmonyPatch(typeof(BossHealth))]
class BossHealth_UpdateBossHealthBarNumberText_Patches
{
    [HarmonyPatch("OnDamaged")]
    [HarmonyPostfix]
    public static void UpdateRecentlyLostHealth(Hit hit)
    {
        Entity entity = UIController.instance.GetBossHealth().GetTarget();
        float postMitigationDamage = Mathf.Max(entity.GetReducedDamage(hit) - entity.Stats.Defense.Final, 0f);
        BossHealthBarNumbersManager.Instance.numbers
            .Where(x => x.textType == BossHealthBarNumberObject.TextType.RecentlyLost).ToList()
            .ForEach(x =>
            {
                x.RecentlyLostHealth += postMitigationDamage;
                x.timeSinceLastHitSeconds = 0f;
            });
    }
}