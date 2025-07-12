using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;

public class MasterConfig
{
    public DamageNumberConfig enemyDamageNumbers = new()
    {
        outlineColor = "#000000"
    };

    public DamageNumberConfig penitentDamageNumbers = new()
    {
        outlineColor = "#FF0000"
    };

    public int precisionDigits = 3;

    internal static string NumberStringFormatted(float number, int precision)
    {
        precision = Mathf.Max(precision, 0);
        if (precision == 0)
        {
            return Mathf.RoundToInt(number).ToString();
        }
        else
        {
            return number.ToString($"F{precision}").TrimEnd('0').TrimEnd('.');
        }
    }

    internal string NumberStringFormatted(float number)
    {
        return NumberStringFormatted(number, precisionDigits);
    }

}
