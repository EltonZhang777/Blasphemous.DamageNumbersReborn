using Blasphemous.DamageNumbersReborn.Components;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;
public class EnemyHealthBarNumberAnimationConfig
{
    /// <summary>
    /// Duration of the fade-out effect in seconds.
    /// </summary>
    public float fadeOutDurationSeconds = 0.1f;

    /// <summary>
    /// Duration of the fade-in effect in seconds.
    /// </summary>
    public float fadeInDurationSeconds = 0.05f;

    private static readonly float _maxFadeDurationSeconds = 5f;

    /// <summary>
    /// Ensures all durations are proper.
    /// </summary>
    internal EnemyHealthBarNumberAnimationConfig Validate()
    {
        // No animation duration should exceed max fade duration.
        fadeInDurationSeconds = Mathf.Clamp(fadeInDurationSeconds, 0, _maxFadeDurationSeconds);
        fadeOutDurationSeconds = Mathf.Clamp(fadeOutDurationSeconds, 0, _maxFadeDurationSeconds);

        return this;
    }

    internal float GetCurrentAlpha(EnemyHealthBarNumberObject number)
    {
        float result;
        float currentTimeSeconds = Mathf.Max(number.timePassedSeconds, 0);
        switch (number.animationState)
        {
            case EnemyHealthBarNumberObject.AnimationState.Showing:
                // Normally showing, fully opaque
                result = 1f;
                break;
            case EnemyHealthBarNumberObject.AnimationState.FadingIn:
                result = currentTimeSeconds / fadeInDurationSeconds;
                break;
            case EnemyHealthBarNumberObject.AnimationState.FadingOut:
                result = 1f - (currentTimeSeconds / fadeOutDurationSeconds);
                break;
            default:
                return 0f;
        }
        return Mathf.Clamp01(result);
    }
}