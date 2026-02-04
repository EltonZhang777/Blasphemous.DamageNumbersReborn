using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.DamageNumbersReborn.Extensions;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI.Helpers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using Gameplay.UI.Others.UIGameLogic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;

[RequireComponent(typeof(RectTransform), typeof(Text), typeof(Outline))]
internal class BossHealthBarNumberObject : MonoBehaviour
{
    public string bossId;
    public BossHealth bossHealthBar;

    internal TextType textType;
    internal BossHealthBarNumberConfig config;
    internal RectTransform rectTransform;
    internal Text text;
    internal Outline outline;
    internal float timeSinceLastHitSeconds;

    private int fontSize;
    private Vector2 rectSize;
    private static readonly float _recentlyLostHealthDisplayDurationSeconds = 2f;

    public Entity Entity => bossHealthBar.GetTarget();
    public float CurrentHealth => Entity.Stats.Life.Current;
    public float MaxHealth => Entity.Stats.Life.Final;
    public float RecentlyLostHealth { get; internal set; }
    public string HealthText
    {
        get
        {
            if (CurrentHealth == 0f)
                return "";
            switch (textType)
            {
                case (TextType.Percentage):
                    return $"({(CurrentHealth / MaxHealth).ToString($"P1")})";
                case (TextType.Details):
                    return $"{Main.DamageNumbersReborn.config.NumberStringFormatted(CurrentHealth)} / {Main.DamageNumbersReborn.config.NumberStringFormatted(MaxHealth)}";
                case (TextType.RecentlyLost):
                    if (Mathf.Approximately(RecentlyLostHealth, 0f))
                        return "";
                    return $"{Main.DamageNumbersReborn.config.NumberStringFormatted(RecentlyLostHealth)}";
                default:
                    return "#ERROR";
            }
        }
    }
    public bool ShouldShowNumber => !UIController.instance.Paused;
    private float Alpha
    {
        get
        {
            // if the game is paused, set current alpha to 0
            float result = ShouldShowNumber
                ? 1f
                : 0f;
            return result;
        }
    }

    public enum TextType
    {
        Percentage,
        Details,
        RecentlyLost
    }

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        text = gameObject.GetComponent<Text>();
        outline = gameObject.GetComponent<Outline>();

        // Initialize transform and parent.
        gameObject.transform.SetParent(UIModder.Parents.CanvasHighRes);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetSiblingIndex(0);

        // set rectTransform
        rectTransform.anchorMin = new(0f, 0f);
        rectTransform.anchorMax = new(0f, 0f);
        rectTransform.pivot = new(0.5f, 0.5f);

        // start as inactive
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // reference to the boss health bar of desired boss
        bossHealthBar = GameObject.FindObjectsOfType<BossHealth>().FirstOrDefault(x => x.GetTarget().Id.Equals(bossId));
        if (bossHealthBar == null)
        {
            BossHealthBarNumbersManager.Instance.RemoveHealthBarNumber(this);
            return;
        }

        // start config
        config = Main.DamageNumbersReborn.config.TextTypeToConfig[textType];

        // start text and font
        fontSize = Mathf.CeilToInt(config.fontSize * MasterConfig.GuiScale);
        rectSize = new Vector2(fontSize * 10f, fontSize * 2f);
        text.font = FontStorage.GetFont(config.fontName);
        text.fontSize = fontSize;
        text.alignment = TextAnchor.UpperRight;
        text.color = config.TextColor;

        // start outline
        outline.effectColor = config.OutlineColor;
        outline.effectDistance = (Vector2)config.outlineDistance * MasterConfig.GuiScale;
    }

    private void Update()
    {
        // remove health bar if game scene not loaded
        if (!SceneHelper.GameSceneLoaded)
        {
            BossHealthBarNumbersManager.Instance.RemoveHealthBarNumber(this);
            return;
        }

        if (textType == TextType.RecentlyLost)
        {
            timeSinceLastHitSeconds += Time.deltaTime;
            if (!Mathf.Approximately(RecentlyLostHealth, 0f) && (timeSinceLastHitSeconds > _recentlyLostHealthDisplayDurationSeconds))
            {
                RecentlyLostHealth = 0f;
            }
        }

        // calculate current screen position of the damage number
        Vector2 worldPosition = NumbersManager.Camera.ScreenToWorldPoint(bossHealthBar.transform.position) + config.labelWorldPositionOffset;
        Vector2 screenPosition = NumbersManager.WorldPointToHighResCameraScreenPoint(worldPosition);

        // Set position
        rectTransform.anchoredPosition = screenPosition;
        rectTransform.sizeDelta = rectSize;

        // Set text
        text.text = HealthText;
        text.color = text.color.ChangeAlphaTo(Alpha);

        // Set text outline
        outline.effectColor = outline.effectColor.ChangeAlphaTo(Alpha);
    }
}
