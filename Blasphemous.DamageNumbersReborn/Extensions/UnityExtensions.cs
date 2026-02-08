using System.Collections;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Extensions;
internal static class UnityExtensions
{
    /// <summary>
    /// Get a component, add it if it doesn't exist
    /// </summary>
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

    /// <summary>
    /// Recursively trace a GameObject's parent until reaching the root object, output the hierarchy as a string
    /// </summary>
    public static string GetHierarchy(this GameObject gameObject)
    {
        string hierarchy = gameObject.name;
        Transform currentTransform = gameObject.transform;
        while (currentTransform.parent != null)
        {
            currentTransform = currentTransform.parent;
            hierarchy = currentTransform.name + "/" + hierarchy;
        }
        return hierarchy;
    }

    public static Coroutine StartCoroutineSafe(this MonoBehaviour mb, IEnumerator routine)
    {
        if (!mb.gameObject.activeInHierarchy)
            return null;

        return mb.StartCoroutine(routine);
    }

    public static bool TryStartCoroutine(this MonoBehaviour mb, IEnumerator routine, out Coroutine coroutine)
    {
        coroutine = mb.StartCoroutineSafe(routine);
        return coroutine != null;
    }
}
