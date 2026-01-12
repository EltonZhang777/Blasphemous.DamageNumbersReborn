using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.ModdingAPI;

namespace Blasphemous.DamageNumbersReborn;

internal class DamageNumbersReborn : BlasMod
{
    internal MasterConfig config;

    public DamageNumbersReborn() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        config = ConfigHandler.Load<MasterConfig>();

        config.penitentDamageNumbers.animation.Validate();
        config.enemyDamageNumbers.animation.Validate();

        ConfigHandler.Save(config);

        // Initialize font storage
        FontStorage.LoadAllFontsFromConfig();
    }
}
