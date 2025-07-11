using Blasphemous.DamageNumbersReborn.Components;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.InputSystem;
using HarmonyLib;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(PlatformCharacterInput), "Awake")]
class PlatformCharacterInput_Awake_AttachDamageNumbersManager_Patch
{
    [HarmonyPrefix]
    public static void Prefix(PlatformCharacterInput __instance)
    {
        __instance.gameObject.AddComponent<DamageNumbersManager>();
    }
}

[HarmonyPatch(typeof(EnemyDamageArea), "TakeDamageAmount")]
class EnemyDamageArea_TakeDamageAmount_SpawnDamageNumber_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Hit hit, EnemyDamageArea __instance)
    {
        if (DamageNumbersManager.instance != null)
        {
            DamageNumbersManager.instance.AddHit(hit.DamageAmount, __instance.OwnerEntity);
        }
    }
}