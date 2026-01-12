using Blasphemous.DamageNumbersReborn.Components;
using Gameplay.GameControllers.Entities;

namespace Blasphemous.DamageNumbersReborn.Extensions;
internal static class EnemyHealthBarExtensions
{
    public static Enemy GetOwner(this EnemyHealthBar bar)
    {
        return TraverseUtils.GetValue<Enemy>(bar, "Owner");
    }
}
