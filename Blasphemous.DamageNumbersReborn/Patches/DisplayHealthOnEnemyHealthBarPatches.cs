using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Extensions;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
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
    public static void AddHealthNumberToDisplay(
        EnemyHealthBar __instance)
    {
        if (!__instance.GetOwner()?.UseHealthBar ?? true)
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
