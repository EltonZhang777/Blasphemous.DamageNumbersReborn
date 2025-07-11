using Blasphemous.ModdingAPI;

namespace Blasphemous.DamageNumbersReborn;

internal class DamageNumbersReborn : BlasMod
{
    internal Config config;

    public DamageNumbersReborn() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        config = ConfigHandler.Load<Config>();
    }
}
