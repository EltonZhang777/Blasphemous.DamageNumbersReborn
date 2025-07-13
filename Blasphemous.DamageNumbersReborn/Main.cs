global using I2LocManager = I2.Loc.LocalizationManager;

using BepInEx;

namespace Blasphemous.DamageNumbersReborn;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.4.1")]
public class Main : BaseUnityPlugin
{
    internal static DamageNumbersReborn DamageNumbersReborn { get; private set; }

    private void Start()
    {
        DamageNumbersReborn = new DamageNumbersReborn();
    }
}
