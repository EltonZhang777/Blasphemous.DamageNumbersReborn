using Blasphemous.DamageNumbersReborn.Components;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;

public class DamageNumberAnimationConfig
{
    /// <summary>
    /// Total display duration in seconds, from start of fade-in to end of fade-out.
    /// </summary>
    public float totalDurationSeconds = 0.7f;

    /// <summary>
    /// Duration of the fade-out effect in seconds.
    /// </summary>
    public float fadeOutDurationSeconds = 0.1f;

    /// <summary>
    /// Duration of the fade-in effect in seconds.
    /// </summary>
    public float fadeInDurationSeconds = 0.05f;

    /// <summary>
    /// Duration of the moving effect in seconds. Moving starts on initialization simultaneously with fade-in.
    /// </summary>
    public float movingDurationSeconds = 0.4f;

    /// <summary>
    /// Total displacement (signed) during the moving effect.
    /// </summary>
    public SerializableVector3 totalMovement = new(0f, 0.7f);

    internal float FadeOutStartTimeSeconds => totalDurationSeconds - fadeOutDurationSeconds;

    /// <summary>
    /// Ensures all durations are proper.
    /// </summary>
    internal DamageNumberAnimationConfig Validate()
    {
        // No animation duration should exceed total duration.
        movingDurationSeconds = Mathf.Clamp(movingDurationSeconds, 0, totalDurationSeconds);
        fadeInDurationSeconds = Mathf.Clamp(fadeInDurationSeconds, 0, totalDurationSeconds);
        fadeOutDurationSeconds = Mathf.Clamp(fadeOutDurationSeconds, 0, totalDurationSeconds);

        // Sum of fade-in and fade-out should not exceed total duration. If exceeds, reduce fade-in duration.
        if (fadeInDurationSeconds + fadeOutDurationSeconds > totalDurationSeconds)
        {
            fadeInDurationSeconds = totalDurationSeconds - fadeOutDurationSeconds;
            fadeInDurationSeconds = Mathf.Clamp(fadeInDurationSeconds, 0, totalDurationSeconds);
        }

        return this;
    }

    internal float GetCurrentAlpha(float currentTimeSeconds)
    {
        currentTimeSeconds = Mathf.Max(currentTimeSeconds, 0);

        // determine which part of the fade animation the current time is in
        if (currentTimeSeconds < fadeInDurationSeconds)
        {
            // Fade-in phase
            return currentTimeSeconds / fadeInDurationSeconds;
        }
        else if (currentTimeSeconds < FadeOutStartTimeSeconds)
        {
            // Full opacity phase
            return 1f;
        }
        else if (currentTimeSeconds < totalDurationSeconds)
        {
            // Fade-out phase
            return 1f - ((currentTimeSeconds - FadeOutStartTimeSeconds) / fadeOutDurationSeconds);
        }
        else
        {
            // After total duration, fully transparent
            return 0f;
        }
    }

    internal void CalculateCurrentPosition(ref Vector2 currentPosition, float currentTimeSeconds, Vector2 startPosition, Vector2 endPosition)
    {
        // Calculate the progress of the moving effect
        float progress = Mathf.Clamp01(currentTimeSeconds / movingDurationSeconds);

        // Interpolate between start and end positions based on progress
        currentPosition = Vector3.Slerp(startPosition, endPosition, progress);
    }
}
