using Blasphemous.DamageNumbersReborn.Extensions;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class EnemyHealthBarNumberObject
{
    public GameObject gameObj;
    public EnemyHealthBar healthBar;
    public bool started = false;
    public AnimationState animationState;
    public float timePassedSeconds = 0f;
    public Enemy Enemy => healthBar.GetOwner();
    public float CurrentHealth => Enemy.Stats.Life.Current;
    public float MaxHealth => Enemy.Stats.Life.Final;
    public string HealthText => $"{Main.DamageNumbersReborn.config.NumberStringFormatted(CurrentHealth)} / {Main.DamageNumbersReborn.config.NumberStringFormatted(MaxHealth)}";
    public bool ShouldShowNumber => !UIController.instance.Paused;
    public enum AnimationState
    {
        FadingIn,
        Showing,
        FadingOut
    }

    /// <summary>
    /// Updates timer of this number.
    /// </summary>
    private void OnUpdate()
    {
        timePassedSeconds += Time.deltaTime;
    }

    private void StartTimer()
    {
        timePassedSeconds = 0f;
        Main.DamageNumbersReborn.OnUpdateEvent += OnUpdate;
    }

    private void StopTimer()
    {
        Main.DamageNumbersReborn.OnUpdateEvent -= OnUpdate;
        timePassedSeconds = 0f;
    }

    internal void FadeIn()
    {
        gameObj.GetComponent<Text>().StartCoroutine(FadeInCoroutine());
    }

    internal void FadeOutAndKillSelf()
    {
        gameObj?.GetComponent<Text>()?.StartCoroutine(FadeOutAndKillSelfCoroutine());
    }

    internal void KillSelf()
    {
        EnemyHealthBarNumbersManager.Instance.numbers.Remove(this);
        GameObject.Destroy(gameObj);
    }

    internal IEnumerator FadeInCoroutine()
    {
        animationState = AnimationState.FadingIn;
        StartTimer();
        yield return new WaitUntil(() => timePassedSeconds >= Main.DamageNumbersReborn.config.enemyHealthBarNumbers.animation.fadeInDurationSeconds);
        animationState = AnimationState.Showing;
        StopTimer();
        yield break;
    }

    internal IEnumerator FadeOutCoroutine()
    {
        animationState = AnimationState.FadingOut;
        StartTimer();
        yield return new WaitUntil(() => timePassedSeconds >= Main.DamageNumbersReborn.config.enemyHealthBarNumbers.animation.fadeOutDurationSeconds);
        StopTimer();
        yield break;
    }

    internal IEnumerator FadeOutAndKillSelfCoroutine()
    {
        yield return FadeOutCoroutine();
        KillSelf();
    }
}
