using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Extensions;
internal static class UnityExtensions
{
    public static T GetOrElseAddComponent<T>(this GameObject obj) where T : Component
    {
        T result = obj.GetComponent<T>();
        result ??= obj.AddComponent<T>();
        return result;
    }

    public static Color ChangeAlphaTo(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
    }
}
