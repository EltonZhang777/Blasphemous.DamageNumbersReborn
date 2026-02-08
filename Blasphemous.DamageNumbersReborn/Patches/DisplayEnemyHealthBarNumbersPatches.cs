using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Extensions;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using HarmonyLib;
using System.Linq;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(EnemyHealthBar))]
class EnemyHealthBar_DisplayEnemyHealthBarNumbers_Patches
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

    [HarmonyPatch("OnDamaged")]
    [HarmonyPostfix]
    public static void EnableBarNumber(
        EnemyHealthBar __instance)
    {
        EnemyHealthBarNumberObject number = EnemyHealthBarNumbersManager.Instance.numbers.FirstOrDefault(x => x.healthBar == __instance);
        if (number == null)
            return;

        number.IsBarEnabled = __instance.IsEnabled;
    }

    [HarmonyPatch("UpdateParent")]
    [HarmonyPostfix]
    public static void AddHealthNumberToDisplay(
        EnemyHealthBar __instance)
    {
        if (!Main.DamageNumbersReborn.config.enemyHealthBarNumbers.enabled
            || (!__instance.GetOwner()?.UseHealthBar ?? true))
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
