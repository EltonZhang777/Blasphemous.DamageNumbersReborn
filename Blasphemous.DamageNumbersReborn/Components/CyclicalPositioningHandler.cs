using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class CyclicalPositioningHandler
{
    private readonly int _cyclicalMovementPeriod;
    private readonly Vector2 _cyclicalPositionRange;
    private int _cyclicalCounter = 1;

    /// <summary>
    /// Cyclical counter for cyclical positioning of new damage numbers. 
    /// Counter starts at `0` and ends at `<see cref="_cyclicalMovementPeriod"/> - 1`.
    /// </summary>
    private int CyclicalCounter
    {
        get => _cyclicalCounter;
        set
        {
            _cyclicalCounter = value;
            while (_cyclicalCounter >= _cyclicalMovementPeriod)
            {
                _cyclicalCounter -= _cyclicalMovementPeriod;
            }
            while (_cyclicalCounter < 0)
            {
                _cyclicalCounter += _cyclicalMovementPeriod;
            }
        }
    }

    internal CyclicalPositioningHandler(
        int cyclicalMovementPeriod,
        Vector2 cyclicalPositionRange)
    {
        _cyclicalMovementPeriod = cyclicalMovementPeriod;
        _cyclicalPositionRange = cyclicalPositionRange;
    }

    internal float GetNextCyclicalOffset()
    {
        float cyclicalRatio = CyclicalCounter / (_cyclicalMovementPeriod - 1f);
        float cyclicalXOffset = Mathf.Lerp(_cyclicalPositionRange.x, _cyclicalPositionRange.y, cyclicalRatio);
        CyclicalCounter++;
        return cyclicalXOffset;
    }
}
