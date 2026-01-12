using Framework.Managers;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class NumbersManager : MonoBehaviour
{
    private protected GameObject _prefab;
    private protected GameObject Prefab
    {
        get
        {
            _prefab ??= CreatePrefab();
            return _prefab;
        }
    }
    private protected Camera Camera => Core.Screen.GameCamera;

    private protected virtual void Awake()
    {
    }

    /// <summary>
    /// Creates a numbers GameObject prefab. Should be overwritten in child classes
    /// </summary>
    private protected virtual GameObject CreatePrefab()
    {
        return null;
    }
}
