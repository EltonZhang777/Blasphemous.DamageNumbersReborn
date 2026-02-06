using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.DamageNumbersReborn.Extensions;
using Blasphemous.Framework.UI;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;

[RequireComponent(typeof(RectTransform), typeof(Text), typeof(Outline))]
internal class PenitentBarNumberObject : MonoBehaviour
{
    internal TextType textType;
    internal UIBarNumberConfig config;
    internal RectTransform rectTransform;
    internal Text text;
    internal Outline outline;
    internal float timeSinceLastHitSeconds;

    private int _fontSize;
    private Vector2 _rectSize;
    private Transform _penitentFervourBar;
    private CanvasGroup _penitentUICanvas;
    private GameObject _loadWidget;

    public Penitent Penitent => Core.Logic.Penitent;
    public float CurrentHealth => Penitent.Stats.Life.Current;
    public float MaxHealth => Penitent.Stats.Life.Final;
    public float CurrentFervour => Penitent.Stats.Fervour.Current;
    public float MaxFervour => Penitent.Stats.Fervour.Final;
    public string DisplayText
    {
        get
        {
            switch (textType)
            {
                case (TextType.HealthDetails):
                    return $"{Main.DamageNumbersReborn.config.NumberStringFormatted(CurrentHealth)} / {Main.DamageNumbersReborn.config.NumberStringFormatted(MaxHealth)}";
                case (TextType.HealthPercentage):
                    return $"({(CurrentHealth / MaxHealth).ToString($"P1")})";
                case (TextType.FervourDetails):
                    return $"{Main.DamageNumbersReborn.config.NumberStringFormatted(CurrentFervour)} / {Main.DamageNumbersReborn.config.NumberStringFormatted(MaxFervour)}";
                case (TextType.FervourPercentage):
                    return $"({(CurrentFervour / MaxFervour).ToString($"P1")})";
                default:
                    return "#ERROR";
            }
        }
    }
    public bool ShouldShowNumber => !UIController.instance.Paused && !_loadWidget.activeInHierarchy;
    private float Alpha
    {
        get
        {
            float result;
            result = _penitentUICanvas.alpha;
            // if the game is paused or is loading, set current alpha to 0
            result = ShouldShowNumber
                ? result
                : 0f;
            return result;
        }
    }

    public enum TextType
    {
        HealthDetails,
        HealthPercentage,
        FervourDetails,
        FervourPercentage
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
        // reference to penitent health bar
        _penitentUICanvas = Traverse.Create(UIController.instance).Field("gameplayWidget").Field("canvas").GetValue<CanvasGroup>();
        _loadWidget = Traverse.Create(UIController.instance).Field("loadWidget").GetValue<GameObject>();
        _penitentFervourBar = GameObject.FindObjectOfType<PlayerFervour>()?.transform;
        if (_penitentFervourBar == null)
        {
            PenitentBarNumbersManager.Instance.RemoveHealthBarNumber(this);
            return;
        }

        // start config
        config = Main.DamageNumbersReborn.config.PenitentBarTextTypeToConfig[textType];

        // start text and font
        _fontSize = Mathf.CeilToInt(config.fontSize * MasterConfig.GuiScale);
        _rectSize = new Vector2(_fontSize * 10f, _fontSize * 2f);
        text.font = FontStorage.GetFont(config.fontName);
        text.fontSize = _fontSize;
        text.alignment = TextAnchor.UpperRight;
        text.color = config.TextColor;

        // start outline
        outline.effectColor = config.OutlineColor;
        outline.effectDistance = (Vector2)config.outlineDistance * MasterConfig.GuiScale;
    }

    private void Update()
    {
        // calculate current screen position of the damage number
        Vector2 worldPosition = NumbersManager.Camera.ScreenToWorldPoint(_penitentFervourBar.transform.position) + config.labelWorldPositionOffset;
        Vector2 screenPosition = NumbersManager.WorldPointToHighResCameraScreenPoint(worldPosition);

        // Set position
        rectTransform.anchoredPosition = screenPosition;
        rectTransform.sizeDelta = _rectSize;

        // Set text
        text.text = DisplayText;
        text.color = text.color.ChangeAlphaTo(Alpha);

        // Set text outline
        outline.effectColor = outline.effectColor.ChangeAlphaTo(Alpha);
    }
}
