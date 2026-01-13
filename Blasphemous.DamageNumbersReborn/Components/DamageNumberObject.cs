using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.DamageNumbersReborn.Extensions;
using Blasphemous.Framework.UI;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;

[RequireComponent(typeof(RectTransform), typeof(Text), typeof(Outline))]
internal class DamageNumberObject : MonoBehaviour
{
    public Vector2 startingPosition;
    public Vector2 finalPosition;
    public bool started = false;
    public float timePassedSeconds;
    public Hit hit;
    public float postMitigationDamage;
    public Entity damagedEntity;
    public EntityType damagedEntityType;

    internal Vector2 currentPosition;
    internal DamageNumberConfig config;
    internal RectTransform rectTransform;
    internal Text text;
    internal Outline outline;

    private int fontSize;
    private Vector2 rectSize;

    private float Alpha
    {
        get
        {
            float result = UIController.instance.Paused
                ? 0f  // if the game is paused, alpha is set to 0.
                : config.animation.GetCurrentAlpha(timePassedSeconds);
            return result;
        }
    }

    public enum EntityType
    {
        Penitent,
        Enemy,
        Boss,
        Other
    }

    private void Awake()
    {
        // Initialize transform and parent.
        gameObject.transform.SetParent(UIModder.Parents.CanvasHighRes);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetSiblingIndex(0);

        // Initialize text component
        text = gameObject.GetComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;

        // Initialize rectTransform
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new(0f, 0f);
        rectTransform.anchorMax = new(0f, 0f);
        rectTransform.pivot = new(0.5f, 0.5f);

        // Initialize Outline
        outline = gameObject.GetComponent<Outline>();

        // Start as deactivated
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // start config
        config = Main.DamageNumbersReborn.config.EntityTypeToConfig[damagedEntityType];

        // calculate font size
        fontSize = Mathf.CeilToInt(config.fontSize * MasterConfig.GuiScale);
        rectSize = new Vector2(fontSize * 10f, fontSize * 2f);

        // start rectTransform
        rectTransform.sizeDelta = rectSize;

        // start text
        text.font = FontStorage.GetFont(config.fontName);
        text.fontSize = fontSize;
        text.color = Main.DamageNumbersReborn.config.DamageElementToColor[hit.DamageElement];

        // start text outline
        outline.effectColor = config.OutlineColor;
        outline.effectDistance = (Vector2)config.outlineDistance * MasterConfig.GuiScale;

        // finish starting
        started = true;
    }

    private void Update()
    {
        if (timePassedSeconds >= config.animation.totalDurationSeconds)
        {
            DamageNumbersManager.Instance.RemoveDamageNumber(this);
            gameObject.SetActive(false);
            return;
        }

        // calculate current screen position of the damage number
        // calculate world position movement progress
        config.animation.CalculateCurrentPosition(ref currentPosition, timePassedSeconds, startingPosition, finalPosition);
        // convert world position to screen position
        Vector2 screenPosition = DamageNumbersManager.WorldPointToHighResCameraScreenPoint(currentPosition);

        // Set position
        rectTransform.anchoredPosition = screenPosition;

        // Set text
        text.text = Main.DamageNumbersReborn.config.NumberStringFormatted(postMitigationDamage);
        text.color = text.color.ChangeAlphaTo(Alpha);

        // Set text outline
        outline.effectColor = outline.effectColor.ChangeAlphaTo(Alpha);

        // Increment time
        timePassedSeconds += Time.deltaTime;
    }
}
