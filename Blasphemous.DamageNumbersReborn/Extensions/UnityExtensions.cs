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
}
