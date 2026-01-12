using Blasphemous.DamageNumbersReborn.Components;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Damage;
using HarmonyLib;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(EnemyDamageArea), "TakeDamageAmount")]
class EnemyDamageArea_TakeDamageAmount_SpawnDamageNumber_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Hit hit, EnemyDamageArea __instance)
    {
        if (Main.DamageNumbersReborn.config.enemyDamageNumbers.enabled)
        {
            DamageNumbersManager.Instance?.AddDamageNumber(hit, __instance.OwnerEntity);
        }
    }
}

[HarmonyPatch(typeof(PenitentDamageArea), "RaiseDamageEvent")]
class PenitentDamageArea_RaiseDamageEvent_SpawnDamageNumber_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Hit hit, PenitentDamageArea __instance)
    {
        if (Main.DamageNumbersReborn.config.penitentDamageNumbers.enabled)
        {
            DamageNumbersManager.Instance?.AddDamageNumber(hit, __instance.OwnerEntity);
        }
    }
}