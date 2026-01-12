using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Extensions;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using HarmonyLib;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(EnemyHealthBar))]
class DisplayHealthOnEnemyHealthBarPatches
{
    [HarmonyPatch("OnDamaged")]
    [HarmonyPrefix]
    public static void ForceDisplayBar()
    {
        if (Main.DamageNumbersReborn.config.alwaysShowEnemyHealthBar)
        {
            Core.Events.SetFlag("SHOW_ENEMY_BAR", true);
        }
    }

    [HarmonyPatch("UpdateParent")]
    [HarmonyPostfix]
    public static void DisplayHealthNumber(
        EnemyHealthBar __instance,
        Enemy ___Owner)
    {
        if (!__instance.GetOwner().UseHealthBar)
            return;

        EnemyHealthBarNumbersManager.Instance.AddHealthBarNumber(__instance);
    }

    [HarmonyPatch("OnDead")]
    [HarmonyPrefix]
    public static void RemoveHealthNumber(
        EnemyHealthBar __instance)
    {
        EnemyHealthBarNumbersManager.Instance.RemoveHealthBarNumber(__instance);
    }
}

[HarmonyPatch(typeof(UIController), "Awake")]
class UIController_Awake_AttachHealthBarNumbersManager_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        UIController.instance.gameObject?.AddComponent<EnemyHealthBarNumbersManager>();
    }
}