using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class PenitentBarNumbersManager : NumbersManager
{
    internal List<PenitentBarNumberObject> numbers;

    private int _poolSize;

    public static PenitentBarNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _poolSize = Main.DamageNumbersReborn.config.PenitentBarTextTypeToConfig.Values.Select(x => x.poolSize).Max() * 4;
        numbers = new(_poolSize);
        AddHealthBarNumber();
        //Main.DamageNumbersReborn.OnLevelLoadedEvent += OnLevelLoaded;
    }

    internal void AddHealthBarNumber()
    {
        foreach (PenitentBarNumberObject.TextType textType in Enum.GetValues(typeof(PenitentBarNumberObject.TextType)))
        {
            if (!Main.DamageNumbersReborn.config.PenitentBarTextTypeToConfig[textType].enabled)
                continue;

            // choose flask display type
            if ((Main.DamageNumbersReborn.config.briefFlasksDisplay == true) && (textType == PenitentBarNumberObject.TextType.FlaskDetailsVanilla))
                continue;
            if ((Main.DamageNumbersReborn.config.briefFlasksDisplay == false) && (textType == PenitentBarNumberObject.TextType.FlaskDetailsBrief))
                continue;

            // check for duplicates
            if (numbers.FirstOrDefault(x => x.textType == textType) != null)
                continue;

            PenitentBarNumberObject numberObject = UIObjectPoolManager.HighRes.ReuseObject(
                Prefab,
                Vector3.zero,
                Quaternion.identity,
                true,
                _poolSize).GameObject.GetComponent<PenitentBarNumberObject>();
            numberObject.textType = textType;
            numbers.Add(numberObject);
            numberObject.gameObject.SetActive(true);
        }
    }

    internal void RemoveHealthBarNumber(PenitentBarNumberObject number)
    {
        number.gameObject.SetActive(false);
        if (!numbers.Contains(number))
            return;
        numbers.Remove(number);
    }

    internal void RemoveAllHealthBarNumbers()
    {
        numbers.ForEach(x => RemoveHealthBarNumber(x));
    }

    internal void OnLevelLoaded(string oldLevel, string newLevel)
    {
        bool active = false;
        if (SceneHelper.GameSceneLoaded && Core.GameModeManager.GetCurrentGameMode() != GameModeManager.GAME_MODES.DEMAKE)
            active = true;
        numbers.ForEach(x => x.gameObject.SetActive(active));
    }

    private protected override GameObject CreatePrefab()
    {
        // Initialize transform and parent.
        GameObject result = new($"PenitentBarNumbers");
        result.transform.SetParent(UIModder.Parents.CanvasHighRes);
        result.AddComponent<PenitentBarNumberObject>();

        // start prefab as inactive
        result.SetActive(false);
        return result;
    }
}
