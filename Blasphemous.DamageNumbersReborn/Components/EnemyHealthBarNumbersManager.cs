using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.DamageNumbersReborn.Extensions;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class EnemyHealthBarNumbersManager : NumbersManager
{
    internal List<EnemyHealthBarNumberObject> numbers = new(50);
    private Vector2 _labelWorldPositionOffset = new(-1.5f, 0.15f);

    public static EnemyHealthBarNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Update()
    {
        if (numbers.Count == 0)
            return;

        // process enemy health bar numbers
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            EnemyHealthBarNumberObject currentNumber = numbers[i];
            DamageNumberConfig currentConfig = Main.DamageNumbersReborn.config.enemyHealthBarNumbers;

            if ((currentNumber?.healthBar == null)
                || (currentNumber?.Enemy.Dead ?? true))
            {
                // If the enemy or health bar doesn't exist, or the enemy dies, remove it
                //currentNumber?.gameObj?.SetActive(false);
                RemoveHealthBarNumber(currentNumber);
                continue;
            }

            // calculate current screen position of the damage number
            Vector2 worldPosition = currentNumber.healthBar.transform.position + (Vector3)_labelWorldPositionOffset;
            Vector2 screenPosition = Camera.WorldToScreenPoint(worldPosition);

            // if the game is paused, deactivate the bar number
            currentNumber?.gameObj?.SetActive(currentNumber.ShouldShowNumber);
            if (!currentNumber.ShouldShowNumber)
                continue;

            // display damage number
            // start the damage number if it isn't started yet
            if (!currentNumber.started)
            {
                // Instatiate the prefab if it doesn't exist
                currentNumber.gameObj ??= GameObject.Instantiate(Prefab, UIController.instance.transform);
                currentNumber.gameObj.SetActive(true);

                // finish starting
                currentNumber.started = true;
            }


            // Set position
            int fontSize = (int)(currentConfig.fontSize * (MasterConfig.GuiScale / 3f));
            Vector2 rectSize = new(fontSize * 10f, fontSize * 2f);
            RectTransform rectTransform = currentNumber.gameObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPosition;
            rectTransform.sizeDelta = rectSize;

            // Set text
            Text text = currentNumber.gameObj.GetComponent<Text>();
            text.text = currentNumber.HealthText;
            text.fontSize = fontSize;
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

    internal void RemoveHealthBarNumber(EnemyHealthBar bar)
    {
        EnemyHealthBarNumberObject number = numbers.FirstOrDefault(x => x.healthBar == bar);
        if (number == null)
            return;

        RemoveHealthBarNumber(number);
    }

    internal void RemoveHealthBarNumber(EnemyHealthBarNumberObject number)
    {
        GameObject.Destroy(number?.gameObj);
        numbers.Remove(number);
    }

    private protected override GameObject CreatePrefab()
    {
        DamageNumberConfig currentConfig = Main.DamageNumbersReborn.config.enemyHealthBarNumbers;

        // Initialize transform and parent.
        GameObject result = new($"HealthBarNumbers");
        result.transform.SetParent(UIController.instance.transform);
        result.transform.localPosition = Vector3.zero;
        result.layer = LayerMask.NameToLayer("UI");
        result.transform.SetSiblingIndex(0);

        // set text and font
        Text text = result.AddComponent<Text>();
        int fontSize = (int)(currentConfig.fontSize * (MasterConfig.GuiScale / 3f));
        Vector2 rectSize = new Vector2(fontSize * 10f, fontSize * 2f);
        text.font = FontStorage.GetFont(currentConfig.fontName);
        text.fontSize = fontSize;
        text.alignment = TextAnchor.UpperRight;
        text.color = MasterConfig.ParseHtmlToColorOrWhite("#d00b0d");

        // set outline
        Outline outline = result.AddComponent<Outline>();
        outline.effectColor = currentConfig.OutlineColor;
        outline.effectDistance = new Vector2(1f, 1f);

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
