using Blasphemous.DamageNumbersReborn.Configs;
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
    protected internal static Camera Camera => Core.Screen.GameCamera;

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

    protected internal static Vector2 WorldPointToHighResCameraScreenPoint(Vector2 pos)
    {
        return Camera.WorldToScreenPoint(pos) * MasterConfig.GuiScale;
    }
}
