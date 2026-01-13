using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using System;

namespace Blasphemous.DamageNumbersReborn;

internal class DamageNumbersReborn : BlasMod
{
    internal MasterConfig config;
    internal event Action OnFirstEnterMainMenu;
    internal event Action OnUpdateEvent;

    public DamageNumbersReborn() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        config = ConfigHandler.Load<MasterConfig>();

        config.penitentDamageNumbers.animation.Validate();
        config.enemyDamageNumbers.animation.Validate();

        ConfigHandler.Save(config);
    }

    protected override void OnAllInitialized()
    {
        // Initialize font storage
        FontStorage.LoadAllFontsFromConfig();

        // Attach number managers to the high-resolution canvas
        OnFirstEnterMainMenu += () =>
        {
            UIModder.Parents.CanvasHighRes.gameObject.AddComponent<DamageNumbersManager>();
            UIModder.Parents.CanvasHighRes.gameObject.AddComponent<EnemyHealthBarNumbersManager>();
        };
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (SceneHelper.MenuSceneLoaded)
        {
            OnFirstEnterMainMenu?.Invoke();
            OnFirstEnterMainMenu = null;
        }
    }

    protected override void OnUpdate()
    {
        OnUpdateEvent?.Invoke();
    }
}
