using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.DamageNumbersReborn.Extensions;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class EnemyHealthBarNumbersManager : NumbersManager
{
    private int _poolSize = 50;
    internal List<EnemyHealthBarNumberObject> numbers;

    public static EnemyHealthBarNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
        numbers = new(_poolSize);
    }

    private void Update()
    {
        if (numbers.Count == 0)
            return;

        // if Penitent is dead or the scene isn't a game scene, kill all numbers
        if ((Core.Logic.Penitent?.Dead ?? true)
            || !SceneHelper.GameSceneLoaded)
        {
            RemoveAllHealthBarNumbers();
            return;
        }

        // process enemy health bar numbers
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            EnemyHealthBarNumberObject currentNumber = numbers[i];
            EnemyHealthBarNumberConfig currentConfig = Main.DamageNumbersReborn.config.enemyHealthBarNumbers;

            if (((currentNumber?.healthBar == null)
                || (currentNumber?.Enemy.Dead ?? true))
                && currentNumber.animationState != EnemyHealthBarNumberObject.AnimationState.FadingOut)
            {
                // If the enemy or health bar doesn't exist, or the enemy dies, remove it
                // If the number is already fading out, don't remove it again
                RemoveHealthBarNumber(currentNumber);
                continue;
            }

            // calculate current screen position of the damage number
            Vector2 worldPosition = currentNumber.healthBar.transform.position + currentConfig.labelWorldPositionOffset;
            Vector2 screenPosition = WorldPointToHighResCameraScreenPoint(worldPosition);

            // if the game is paused or TPO is dead, set current alpha to 0
            float currentAlpha = currentNumber.ShouldShowNumber
                ? currentConfig.animation.GetCurrentAlpha(currentNumber)
                : 0f;

            // display damage number
            // start the unstarted damage number if the enemy health bar is enabled
            // else, skip it until enemy health bar is enabled
            if (!currentNumber.started)
            {
                if (!(currentNumber?.healthBar?.IsEnabled ?? false))
                    continue;

                // Instatiate the prefab if it doesn't exist
                currentNumber.gameObj ??= GameObject.Instantiate(Prefab, UIModder.Parents.CanvasHighRes);
                currentNumber.gameObj.SetActive(true);

                // finish starting
                currentNumber.started = true;
                currentNumber.FadeIn();
            }

            // Set position
            int fontSize = Mathf.CeilToInt(currentConfig.fontSize * MasterConfig.GuiScale);
            Vector2 rectSize = new(fontSize * 10f, fontSize * 2f);
            RectTransform rectTransform = currentNumber.gameObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPosition;
            rectTransform.sizeDelta = rectSize;

            // Set text
            Text text = currentNumber.gameObj.GetComponent<Text>();
            text.text = currentNumber.HealthText;
            text.fontSize = fontSize;
            text.color = text.color.ChangeAlphaTo(currentAlpha);

            // Set text outline
            Outline outline = currentNumber.gameObj.GetComponent<Outline>();
            outline.effectColor = outline.effectColor.ChangeAlphaTo(currentAlpha);
        }
    }

    internal void AddHealthBarNumber(EnemyHealthBar bar)
    {
        EnemyHealthBarNumberObject result = new()
        {
            healthBar = bar
        };
        numbers.Add(result);
    }

    internal void RemoveHealthBarNumber(EnemyHealthBar bar, bool force = false)
    {
        EnemyHealthBarNumberObject number = numbers.FirstOrDefault(x => x.healthBar == bar);
        if (number == null)
            return;

        RemoveHealthBarNumber(number, force);
    }

    internal void RemoveHealthBarNumber(EnemyHealthBarNumberObject number, bool force = false)
    {
        if (force)
        {
            number.KillSelf();
            return;
        }

        if (number.animationState == EnemyHealthBarNumberObject.AnimationState.FadingOut)
            return;
        number.FadeOutAndKillSelf();
    }

    internal void RemoveAllHealthBarNumbers(bool force = false)
    {
        numbers.ForEach(x => RemoveHealthBarNumber(x, force));
    }

    private protected override GameObject CreatePrefab()
    {
        EnemyHealthBarNumberConfig currentConfig = Main.DamageNumbersReborn.config.enemyHealthBarNumbers;

        // Initialize transform and parent.
        GameObject result = new($"HealthBarNumbers");
        result.transform.SetParent(UIModder.Parents.CanvasHighRes);
        result.transform.localPosition = Vector3.zero;
        result.layer = LayerMask.NameToLayer("UI");
        result.transform.SetSiblingIndex(0);

        // set text and font
        Text text = result.AddComponent<Text>();
        int fontSize = Mathf.CeilToInt(currentConfig.fontSize * MasterConfig.GuiScale);
        Vector2 rectSize = new Vector2(fontSize * 10f, fontSize * 2f);
        text.font = FontStorage.GetFont(currentConfig.fontName);
        text.fontSize = fontSize;
        text.alignment = TextAnchor.UpperRight;
        text.color = currentConfig.TextColor;

        // set outline
        Outline outline = result.AddComponent<Outline>();
        outline.effectColor = currentConfig.OutlineColor;
        outline.effectDistance = (Vector2)currentConfig.outlineDistance * MasterConfig.GuiScale;

        // set rectTransform
        RectTransform rectTransform = result.GetOrElseAddComponent<RectTransform>();
        rectTransform.anchorMin = new(0f, 0f);
        rectTransform.anchorMax = new(0f, 0f);
        rectTransform.pivot = new(0.5f, 0.5f);

        // start prefab as inactive
        result.SetActive(false);

        return result;
    }
}
