using Blasphemous.ModdingAPI;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class TraverseUtils
{
    public enum TraverseAccessType
    {
        Field,
        Property
    }

    /// <summary>
    /// Get value from a field/property of a traverse instance, regardless of accessibility levels
    /// </summary>
    public static TValue GetValue<TValue>(Traverse traverse, string variableName, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        if (traverse == null)
        {
            ModLog.Error($"Failed to get null value from null traverse instance! Returning default");
            return default(TValue);
        }
        variableName = variableName.Trim();
        return accessType switch
        {
            TraverseAccessType.Field => traverse.Field(variableName).GetValue<TValue>(),
            TraverseAccessType.Property => traverse.Property(variableName).GetValue<TValue>(),
            _ => default(TValue)
        };
    }

    /// <summary>
    /// Traverse and get value of a variable, regardless of accessibility levels
    /// </summary>
    public static TValue GetValue<TValue>(object obj, string variableName, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        Traverse traverse = Traverse.Create(obj);
        if (traverse == null)
        {
            ModLog.Error($"Failed to get null value from object of type `{obj.GetType()}`! Returning default");
            return default(TValue);
        }
        return GetValue<TValue>(traverse, variableName, accessType);
    }

    /// <summary>
    /// Set value to a field/property of a traverse instance, regardless of accessibility levels
    /// </summary>
    public static void SetValue<TValue>(ref Traverse traverse, string variableName, TValue value, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        variableName = variableName.Trim();
        switch (accessType)
        {
            case TraverseAccessType.Field:
                traverse.Field(variableName).SetValue(value);
                break;
            case TraverseAccessType.Property:
                traverse.Property(variableName).SetValue(value);
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// Traverse and set value of a variable, regardless of accessibility levels
    /// </summary>
    public static void SetValue<TTarget, TValue>(ref TTarget obj, string variableName, TValue value, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        Traverse traverse = Traverse.Create(obj);
        SetValue<TValue>(ref traverse, variableName, value, accessType);
    }

    /// <summary>
    /// Validate if the given value satisfies the given restriction, 
    /// then traverse and set value of a variable, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfValidated<TTarget, TValue>(ref TTarget obj, string variableName, TValue value, Func<TValue, bool> validate, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        if (!validate(value))
        {
            return;
        }
        SetValue(ref obj, variableName, value, accessType);
    }

    /// <summary>
    /// Validate if the given value satisfies the given restriction, 
    /// then set value of a traverse instance, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfValidated<TValue>(ref Traverse traverse, string variableName, TValue value, Func<TValue, bool> validate, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        if (!validate(value))
        {
            return;
        }
        SetValue<TValue>(ref traverse, variableName, value, accessType);
    }

    /// <summary>
    /// Validate if the given value satisfies all given restrictions, 
    /// then traverse and set value of a variable, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfValidated<TTarget, TValue>(ref TTarget obj, string variableName, TValue value, List<Func<TValue, bool>> validates, TraverseAccessType accessType = TraverseAccessType.Field)
    {
        foreach (Func<TValue, bool> validate in validates)
        {
            if (!validate(value))
            {
                return;
            }
        }
        SetValue(ref obj, variableName, value, accessType);
    }

    /// <summary>
    /// Validate if the given nullable struct value is not null, 
    /// then set value of the traverse instance, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfNotNull<TValue>(ref Traverse traverse, string variableName, TValue? value, TraverseAccessType accessType = TraverseAccessType.Field)
        where TValue : struct
    {
        if (!value.HasValue)
        {
            return;
        }
        SetValue<TValue>(ref traverse, variableName, value.Value, accessType);
    }

    /// <summary>
    /// Validate if the given nullable struct value is not null, 
    /// then traverse and set value of a variable, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfNotNull<TTarget, TValue>(ref TTarget obj, string variableName, TValue? value, TraverseAccessType accessType = TraverseAccessType.Field)
        where TValue : struct
    {
        if (!value.HasValue)
        {
            return;
        }
        SetValue(ref obj, variableName, value.Value, accessType);
    }

    /// <summary>
    /// Validate if the given nullable class value is not null, 
    /// then set value of the traverse instance, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfNotNull<TValue>(ref Traverse traverse, string variableName, TValue value, TraverseAccessType accessType = TraverseAccessType.Field)
        where TValue : class
    {
        if (value == null)
        {
            return;
        }
        SetValue<TValue>(ref traverse, variableName, value, accessType);
    }

    /// <summary>
    /// Validate if the given nullable class value is not null, 
    /// then traverse and set value of a variable, regardless of accessibility levels
    /// </summary>
    public static void SetValueIfNotNull<TTarget, TValue>(ref TTarget obj, string variableName, TValue value, TraverseAccessType accessType = TraverseAccessType.Field)
        where TValue : class
    {
        if (value == null)
        {
            return;
        }
        SetValue(ref obj, variableName, value, accessType);
    }
}

