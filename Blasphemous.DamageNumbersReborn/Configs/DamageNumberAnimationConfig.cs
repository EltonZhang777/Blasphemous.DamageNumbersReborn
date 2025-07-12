namespace Blasphemous.DamageNumbersReborn.Configs;

public class DamageNumberAnimationConfig
{
    /// <summary>
    /// Total display duration in seconds, from start of fade-in to end of fade-out.
    /// </summary>
    public float totalDurationSeconds = 1.2f;

    /// <summary>
    /// Duration of the fade-out effect in seconds.
    /// </summary>
    public float fadeOutDurationSeconds = 0.2f;

    /// <summary>
    /// Duration of the fade-in effect in seconds.
    /// </summary>
    public float fadeInDurationSeconds = 0.1f;

    /// <summary>
    /// Duration of the moving effect in seconds. Moving starts on initialization simultaneously with fade-in.
    /// </summary>
    public float movingDurationSeconds = 0.8f;

    internal float FadeOutStartTimeSeconds => totalDurationSeconds - fadeOutDurationSeconds;
}
