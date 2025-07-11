using BepInEx;

namespace Blasphemous.DamageNumbersReborn;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "0.1.0")]
public class Main : BaseUnityPlugin
{
    internal static DamageNumbersReborn DamageNumbersReborn { get; private set; }

    private void Start()
    {
        DamageNumbersReborn = new DamageNumbersReborn();
    }
}
