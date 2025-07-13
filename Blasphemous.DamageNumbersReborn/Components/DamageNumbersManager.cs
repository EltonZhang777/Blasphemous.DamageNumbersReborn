using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class DamageNumbersManager : MonoBehaviour
{
    internal List<DamageNumberObject> damageNumbers;

    private Vector2 _labelWorldPositionOffset = new(0f, 1f);
    private GameObject _prefab;
    private static Dictionary<string, Font> _fonts = new();
    private int _cyclicalMovementPeriod = 3;
    private int _cyclicalCounter = 1;
    private static readonly string _defaultFontName = "MajesticExtended_Pixel_Scroll";

    private GameObject Prefab => _prefab ??= CreatePrefab();
    private Camera Camera => Core.Screen.GameCamera;
    private float ScreenWidthScale => (float)Screen.width / 640f;
    private float ScreenHeightScale => (float)Screen.height / 360f;
    private float GuiScale => (ScreenHeightScale > ScreenWidthScale) ? ScreenWidthScale : ScreenHeightScale;

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

    public static DamageNumbersManager instance { get; private set; }

    private void Awake()
    {
        DamageNumbersManager.instance = this;
        damageNumbers = new List<DamageNumberObject>(40);

        // Initialize font storage
        List<string> allFontNamesInConfig = Main.DamageNumbersReborn.config
            .GetType().GetFields((BindingFlags.Public | BindingFlags.Instance))
            .Where(x => x.FieldType == typeof(DamageNumberConfig))
            .Select(x => ((DamageNumberConfig)x.GetValue(Main.DamageNumbersReborn.config)).fontName).ToList();
        allFontNamesInConfig.Add(_defaultFontName);
        foreach (string fontName in allFontNamesInConfig.Distinct())
        {
            Font font = GetFont(fontName);
        }
    }

    public void AddDamageNumber(Hit hit, Entity entity)
    {
        float postMitigationDamage = Mathf.Max(entity.GetReducedDamage(hit) - entity.Stats.Defense.Final, 0f);
        Vector3 entityPosition = entity.GetComponentInChildren<DamageArea>().TopCenter;
        float randomXOffset = UnityEngine.Random.Range(-0.2f, 0.2f);
        float cyclicalXOffset = (CyclicalCounter - 1) * 0.7f;

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

        DamageNumberObject item = new()
        {
            hit = hit,
            postMitigationDamage = postMitigationDamage,
            originalPosition = new Vector2(entityPosition.x + randomXOffset + cyclicalXOffset, entityPosition.y),
            damagedEntity = entity,
            damagedEntityType = entityType
        };
        damageNumbers.Add(item);

        CyclicalCounter++;
    }

    private void Update()
    {
        if (damageNumbers.Count != 0)
        {
            // process damage numbers
            for (int i = damageNumbers.Count - 1; i >= 0; i--)
            {
                DamageNumberObject currentDamageNumber = damageNumbers[i];
                DamageNumberConfig currentConfig = Main.DamageNumbersReborn.config.EntityTypeToConfig[currentDamageNumber.damagedEntityType];

                if (currentDamageNumber.timePassed >= currentConfig.animation.totalDurationSeconds)
                {
                    // If the damage number has exceeded its duration, kill it
                    GameObject.Destroy(damageNumbers[i].gameObj);
                    damageNumbers.RemoveAt(i);
                }
                else
                {
                    // calculate screen position of the damage number
                    float currentYSpeed = 12f;
                    float currentYDisplacement = currentYSpeed * Time.deltaTime;
                    currentDamageNumber.screenY += currentYDisplacement;
                    Vector3 screenPosition = Camera.WorldToScreenPoint(currentDamageNumber.originalPosition + _labelWorldPositionOffset);
                    screenPosition = new Vector2()
                    {
                        x = screenPosition.x,
                        y = (screenPosition.y + currentDamageNumber.screenY)
                    };

                    // calculate current alpha
                    double currentElapsedTimeRatio = 1.0 - (double)(1f / (currentConfig.animation.totalDurationSeconds / (currentConfig.animation.totalDurationSeconds - currentDamageNumber.timePassed)));
                    float currentAlpha = EaseInQuint(1f, 0f, (float)currentElapsedTimeRatio);
                    // if the game is paused, alpha is set to 0.
                    if (UIController.instance.Paused)
                    {
                        currentAlpha = 0f;
                    }

                    // display damage number
                    // Instatiate the prefab if it doesn't exist
                    currentDamageNumber.gameObj ??= GameObject.Instantiate(Prefab, UIController.instance.transform);
                    currentDamageNumber.gameObj.SetActive(true);

                    // initialize font
                    //int fontSize = (int)(currentConfig.fontSize * (GuiScale / 3f));
                    int fontSize = (int)(currentConfig.fontSize * (GuiScale / 3f));
                    Vector2 rectSize = new Vector2(fontSize * 10f, fontSize * 2f);
                    ModLog.Warn($"fontSize: {fontSize} \n  GuiScale: {GuiScale} \n  rectSize: {rectSize}");

                    // Set position
                    RectTransform rectTransform = currentDamageNumber.gameObj.GetComponent<RectTransform>();

                    rectTransform.anchorMin = new(0f, 0f);
                    rectTransform.anchorMax = new(0f, 0f);
                    rectTransform.pivot = new(0.5f, 0.5f);
                    rectTransform.anchoredPosition = screenPosition;
                    rectTransform.sizeDelta = rectSize;

                    // Set text
                    Text text = currentDamageNumber.gameObj.GetComponent<Text>();
                    text.text = Main.DamageNumbersReborn.config.NumberStringFormatted(currentDamageNumber.postMitigationDamage);
                    text.fontSize = fontSize;
                    Font font = GetFont(currentConfig.fontName);
                    ModLog.Warn($"font.fontSize : {font.fontSize}");
                    text.font = font;
                    Color textColor = Main.DamageNumbersReborn.config.DamageElementToColor[currentDamageNumber.hit.DamageElement];
                    textColor.a = currentAlpha;
                    text.color = textColor;

                    // Set outline
                    Outline outline = currentDamageNumber.gameObj.GetComponent<Outline>();
                    Color outlineColor = currentConfig.OutlineColor;
                    outlineColor.a = currentAlpha;
                    outline.effectColor = outlineColor;

                    // Increment time
                    currentDamageNumber.timePassed += Time.deltaTime;
                }
            }
        }

    }

    private float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    /// <summary>
    /// Create the prefab of damage numbers that instances will instantitate upon.
    /// </summary>
    private GameObject CreatePrefab()
    {
        // Initialize transform and parent.
        GameObject result = new($"DamageNumbers");
        result.transform.SetParent(UIController.instance.transform);
        result.transform.localPosition = Vector3.zero;
        result.layer = LayerMask.NameToLayer("UI");
        result.transform.SetSiblingIndex(0);

        // Initialize text component
        Text text = result.AddComponent<Text>();
        text.font = GetFont(_defaultFontName);
        text.fontSize = 16;
        text.alignment = TextAnchor.MiddleCenter;

        Outline outline = result.AddComponent<Outline>();
        outline.effectDistance = new Vector2(1f, 1f);

        // Set color to transparent
        Color transparent = new(0, 0, 0, 0);
        text.color = transparent;
        outline.effectColor = transparent;

        return result;
    }

    /// <summary>
    /// Gets the font by name, or the default font if not found. 
    /// First queries through the internal storage, then query I2LocManager if not found. 
    /// If found, store it in the internal storage if hasn't already.
    /// </summary>
    private Font GetFont(string fontName)
    {
        _fonts.TryGetValue(fontName, out Font result);
        if (result == null)  // isn't in the internal storage
        {
            // find in I2Loc storage
            result = I2LocManager.FindAsset(fontName) as Font;
            if (result == null)  // isn't in the I2Loc storage either
            {
                // fall back to default font
                result = I2LocManager.FindAsset(_defaultFontName) as Font;
            }
            else
            {
                // store it in the internal storage
                _fonts.Add(fontName, result);
            }
        }

        return result;
    }
}
