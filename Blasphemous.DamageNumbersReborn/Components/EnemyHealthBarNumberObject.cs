using Blasphemous.DamageNumbersReborn.Configs;
using Blasphemous.DamageNumbersReborn.Extensions;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;

[RequireComponent(typeof(RectTransform), typeof(Text), typeof(Outline))]
internal class EnemyHealthBarNumberObject : MonoBehaviour
{
    public EnemyHealthBar healthBar;
    public AnimationState animationState;
    public float timePassedSeconds = 0f;

    internal EnemyHealthBarNumberConfig config;
    internal RectTransform rectTransform;
    internal Text text;
    internal Outline outline;

    private int fontSize;
    private Vector2 rectSize;

    private bool _isBarEnabled;

    public Enemy Enemy => healthBar.GetOwner();
    public float CurrentHealth => Enemy.Stats.Life.Current;
    public float MaxHealth => Enemy.Stats.Life.Final;
    public string HealthText => $"{Main.DamageNumbersReborn.config.NumberStringFormatted(CurrentHealth)} / {Main.DamageNumbersReborn.config.NumberStringFormatted(MaxHealth)}";
    public bool ShouldShowNumber => !UIController.instance.Paused;
    public bool IsBarEnabled
    {
        get
        {
            return _isBarEnabled;
        }
        set
        {
            if ((_isBarEnabled == false) && (value == true))
                gameObject.SetActive(true);

            _isBarEnabled = value;
        }
    }
    private float Alpha
    {
        get
        {
            // if the game is paused, set current alpha to 0
            float result = ShouldShowNumber
                ? config.animation.GetCurrentAlpha(this)
                : 0f;
            return result;
        }
    }
    public enum AnimationState
    {
        FadingIn,
        Showing,
        FadingOut
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
        // start config
        config = Main.DamageNumbersReborn.config.enemyHealthBarNumbers;

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

        if (!(healthBar?.IsEnabled ?? false))
            return;

        // finish starting
        FadeIn();
    }

    private void Update()
    {
        // update timer
        if (animationState != AnimationState.Showing)
            timePassedSeconds += Time.deltaTime;

        // remove health bar if Penitent or the enemy is dead, or game scene not loaded
        if ((Core.Logic.Penitent?.Dead ?? true)
            || (healthBar?.GetOwner()?.Dead ?? true)
            || !SceneHelper.GameSceneLoaded)
        {
            EnemyHealthBarNumbersManager.Instance.RemoveHealthBarNumber(this);
            return;
        }

        // calculate current screen position of the damage number
        Vector2 worldPosition = healthBar.transform.position + config.labelWorldPositionOffset;
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

    internal void FadeIn()
    {
        text.StartCoroutineSafe(FadeInCoroutine());
    }

    internal void FadeOutAndKillSelf()
    {
        text?.StartCoroutineSafe(FadeOutAndKillSelfCoroutine());
    }

    internal void KillSelf()
    {
        IsBarEnabled = false;
        gameObject.SetActive(false);
    }

    internal IEnumerator FadeInCoroutine()
    {
        animationState = AnimationState.FadingIn;
        timePassedSeconds = 0f;
        yield return new WaitUntil(() => timePassedSeconds >= Main.DamageNumbersReborn.config.enemyHealthBarNumbers.animation.fadeInDurationSeconds);
        animationState = AnimationState.Showing;
        timePassedSeconds = 0f;
        yield break;
    }

    internal IEnumerator FadeOutCoroutine()
    {
        animationState = AnimationState.FadingOut;
        timePassedSeconds = 0f;
        yield return new WaitUntil(() => timePassedSeconds >= Main.DamageNumbersReborn.config.enemyHealthBarNumbers.animation.fadeOutDurationSeconds);
        timePassedSeconds = 0f;
        yield break;
    }

    internal IEnumerator FadeOutAndKillSelfCoroutine()
    {
        yield return FadeOutCoroutine();
        KillSelf();
    }
}
