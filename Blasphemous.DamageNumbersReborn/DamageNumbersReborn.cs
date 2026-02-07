using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using System;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn;

internal class DamageNumbersReborn : BlasMod
{
    internal MasterConfig config;
    internal event Action OnFirstEnterMainMenu;
    internal event Action OnUpdateEvent;
    internal event Action<string, string> OnLevelLoadedEvent;

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
            GameObject canvasGameObj = UIModder.Parents.CanvasHighRes.gameObject;
            canvasGameObj.AddComponent<DamageNumbersManager>();
            canvasGameObj.AddComponent<EnemyHealthBarNumbersManager>();
            canvasGameObj.AddComponent<BossHealthBarNumbersManager>();
            canvasGameObj.AddComponent<PenitentBarNumbersManager>();
        };
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (SceneHelper.MenuSceneLoaded)
        {
            OnFirstEnterMainMenu?.Invoke();
            OnFirstEnterMainMenu = null;
        }
        OnLevelLoadedEvent?.Invoke(oldLevel, newLevel);
    }

    protected override void OnUpdate()
    {
        OnUpdateEvent?.Invoke();
    }
}
