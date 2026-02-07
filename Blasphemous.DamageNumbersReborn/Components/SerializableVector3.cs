using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

/// <summary>
/// Serializable representation of a <see cref="Vector3"/>
/// </summary>
public readonly record struct SerializableVector3
{
    /// <summary> The X coordinate </summary>
    public float X { get; }

    /// <summary> The Y coordinate </summary>
    public float Y { get; }

    /// <summary> The Z coordinate </summary>
    public float Z { get; }

    /// <summary>
    /// Creates a new instance of <see cref="SerializableVector3"/> with the specified properties
    /// </summary>
    public SerializableVector3(float x, float y, float z = 0f)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Formats the output string of the <see cref="SerializableVector3"/>
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";

    /// <summary>
    /// (0, 0, 0)
    /// </summary>
    public static SerializableVector3 Zero => new(0, 0, 0);
    /// <summary>
    /// (1, 1, 1)
    /// </summary>
    public static SerializableVector3 One => new(1, 1, 1);

    /// <summary>
    /// Converts to a <see cref="Vector3"/>
    /// </summary>
    public static implicit operator Vector3(SerializableVector3 v) => new(v.X, v.Y, v.Z);

    /// <summary>
    /// Converts to a <see cref="SerializableVector3"/>
    /// </summary>
    public static implicit operator SerializableVector3(Vector3 v) => new(v.x, v.y, v.z);

    /// <summary>
    /// Converts to a <see cref="Vector2"/>
    /// </summary>
    public static implicit operator Vector2(SerializableVector3 v) => new(v.X, v.Y);

    /// <summary>
    /// Converts to a <see cref="SerializableVector3"/>
    /// </summary>
    public static implicit operator SerializableVector3(Vector2 v) => new(v.x, v.y, 0);
}
