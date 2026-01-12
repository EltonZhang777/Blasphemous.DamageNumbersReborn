using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.Framework.UI;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class DamageNumbersManager : NumbersManager
{
    internal List<DamageNumberObject> damageNumbers;

    private Vector2 _labelWorldPositionOffset = new(0f, 1f);
    private int _cyclicalMovementPeriod = 3;
    private int _cyclicalCounter = 1;
    private Vector2 _cyclicalXRange = new Vector2(-0.8f, 0.8f);


    /// <summary>
    /// Cyclical counter for cyclical positioning of new damage numbers. 
    /// Counter starts at `0` and ends at `<see cref="_cyclicalMovementPeriod"/> - 1`.
    /// </summary>
    private int CyclicalCounter
    {
        get => _cyclicalCounter;
        set
        {
            _cyclicalCounter = value;
            while (_cyclicalCounter >= _cyclicalMovementPeriod)
            {
                _cyclicalCounter -= _cyclicalMovementPeriod;
            }
            while (_cyclicalCounter < 0)
            {
                _cyclicalCounter += _cyclicalMovementPeriod;
            }
        }
    }

    private Vector2 CyclicalXRange
    {
        get => _cyclicalXRange;
        set
        {
            // Ensure the range is valid
            if (value.x > value.y)
            {
                _cyclicalXRange = new Vector2(value.y, value.x);
            }
            else
            {
                _cyclicalXRange = value;
            }
        }
    }

    public static DamageNumbersManager Instance { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Instance = this;
        damageNumbers = new List<DamageNumberObject>(40);
    }

    public void AddDamageNumber(Hit hit, Entity entity)
    {
        // Determine the damaged entity type for the damage number
        DamageNumberObject.EntityType entityType;
        if (entity is Penitent)
        {
            entityType = DamageNumberObject.EntityType.Penitent;
        }
        else if (entity is Enemy)
        {
            if (entity.Id.StartsWith("BS"))
            {
                entityType = DamageNumberObject.EntityType.Boss;
            }
            else
            {
                entityType = DamageNumberObject.EntityType.Enemy;
            }
        }
        else
        {
            entityType = DamageNumberObject.EntityType.Other;
        }

        // Determine config type based on entity type
        DamageNumberConfig currentConfig = Main.DamageNumbersReborn.config.EntityTypeToConfig[entityType];

        // set starting position
        Vector3 entityPosition = entity.GetComponentInChildren<DamageArea>().TopCenter;
        // apply random x offset
        float randomXOffset = UnityEngine.Random.Range(-0.2f, 0.2f);
        // apply cyclical x offset
        float cyclicalRatio = (float)CyclicalCounter / ((float)_cyclicalMovementPeriod - 1f);
        float cyclicalXOffset = Mathf.Lerp(CyclicalXRange.x, CyclicalXRange.y, cyclicalRatio);
        CyclicalCounter++;
        // finalize starting and final position
        Vector2 startingPosition = new Vector2(entityPosition.x + randomXOffset + cyclicalXOffset, entityPosition.y) + _labelWorldPositionOffset;
        Vector2 finalPosition = startingPosition + new Vector2(
            currentConfig.animation.totalXMovement,
            currentConfig.animation.totalYMovement);

        // calculate post-mitigation damage
        float postMitigationDamage = Mathf.Max(entity.GetReducedDamage(hit) - entity.Stats.Defense.Final, 0f);

        DamageNumberObject result = new()
        {
            hit = hit,
            postMitigationDamage = postMitigationDamage,
            startingPosition = startingPosition,
            finalPosition = finalPosition,
            damagedEntity = entity,
            damagedEntityType = entityType
        };
        result.currentPosition = result.startingPosition;
        damageNumbers.Add(result);
    }

    private void Update()
    {
        if (damageNumbers.Count == 0)
            return;

        // process damage numbers
        for (int i = damageNumbers.Count - 1; i >= 0; i--)
        {
            DamageNumberObject currentDamageNumber = damageNumbers[i];
            DamageNumberConfig currentConfig = Main.DamageNumbersReborn.config.EntityTypeToConfig[currentDamageNumber.damagedEntityType];

            if (currentDamageNumber == null
                || currentDamageNumber?.timePassed >= currentConfig.animation.totalDurationSeconds)
            {
                // If the damage number has exceeded its duration, kill it
                GameObject.Destroy(currentDamageNumber?.gameObj);
                damageNumbers.RemoveAt(i);
                continue;
            }

            // calculate current screen position of the damage number
            // calculate world position movement progress
            currentConfig.animation.CalculateCurrentPosition(ref currentDamageNumber.currentPosition, currentDamageNumber.timePassed, currentDamageNumber.startingPosition, currentDamageNumber.finalPosition);
            // convert world position to screen position
            Vector2 screenPosition = WorldPointToHighResCameraScreenPoint(currentDamageNumber.currentPosition);

            // calculate current alpha
            float currentAlpha = currentConfig.animation.GetCurrentAlpha(currentDamageNumber.timePassed);
            // if the game is paused, alpha is set to 0.
            if (UIController.instance.Paused)
            {
                currentAlpha = 0f;
            }

            // calculate font and rect size based on current GUI scale
            int fontSize = Mathf.CeilToInt(currentConfig.fontSize * MasterConfig.GuiScale);
            Vector2 rectSize = new Vector2(fontSize * 10f, fontSize * 2f);

            // display damage number
            // Start the damage number if it isn't started yet
            if (!currentDamageNumber.started)
            {
                currentDamageNumber.gameObj ??= GameObject.Instantiate(Prefab, UIModder.Parents.CanvasHighRes);
                currentDamageNumber.gameObj.SetActive(true);

                // start rectTransform
                RectTransform t_rect = currentDamageNumber.gameObj.GetComponent<RectTransform>();
                t_rect.sizeDelta = rectSize;

                // start text
                Text t_text = currentDamageNumber.gameObj.GetComponent<Text>();
                Font font = FontStorage.GetFont(currentConfig.fontName);
                t_text.font = font;
                t_text.fontSize = fontSize;
                Color t_textColor = Main.DamageNumbersReborn.config.DamageElementToColor[currentDamageNumber.hit.DamageElement];
                t_text.color = t_textColor;

                // start text outline
                Outline t_outline = currentDamageNumber.gameObj.GetComponent<Outline>();
                t_outline.effectColor = currentConfig.OutlineColor;

                // finish starting
                currentDamageNumber.started = true;
            }

            // Set position
            RectTransform rectTransform = currentDamageNumber.gameObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPosition;

            // Set text
            Text text = currentDamageNumber.gameObj.GetComponent<Text>();
            text.text = Main.DamageNumbersReborn.config.NumberStringFormatted(currentDamageNumber.postMitigationDamage);
            Color textColor = text.color;
            textColor.a = currentAlpha;
            text.color = textColor;

            // Set text outline
            Outline outline = currentDamageNumber.gameObj.GetComponent<Outline>();
            Color outlineColor = outline.effectColor;
            outlineColor.a = currentAlpha;
            outline.effectColor = outlineColor;

            // Increment time
            currentDamageNumber.timePassed += Time.deltaTime;
        }
    }

    /// <summary>
    /// Create the prefab of damage numbers that instances will instantitate upon.
    /// </summary>
    private protected override GameObject CreatePrefab()
    {
        // Initialize transform and parent.
        GameObject result = new($"DamageNumbers");
        result.transform.SetParent(UIModder.Parents.CanvasHighRes);
        result.transform.localPosition = Vector3.zero;
        result.layer = LayerMask.NameToLayer("UI");
        result.transform.SetSiblingIndex(0);

        // Initialize text component
        Text text = result.AddComponent<Text>();
        text.font = FontStorage.GetDefaultFont();
        text.fontSize = 16;
        text.alignment = TextAnchor.MiddleCenter;

        Outline outline = result.AddComponent<Outline>();
        outline.effectDistance = new Vector2(1f, 1f) * MasterConfig.GuiScale;

        // Initialize rectTransform
        RectTransform rectTransform = result.GetComponent<RectTransform>();
        rectTransform.anchorMin = new(0f, 0f);
        rectTransform.anchorMax = new(0f, 0f);
        rectTransform.pivot = new(0.5f, 0.5f);

        // Set color to transparent
        Color transparent = new(0, 0, 0, 0);
        text.color = transparent;
        outline.effectColor = transparent;

        return result;
    }
}
