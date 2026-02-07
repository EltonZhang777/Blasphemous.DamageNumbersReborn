using Blasphemous.DamageNumbersReborn.Components;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using Gameplay.UI.Others.UIGameLogic;

namespace Blasphemous.DamageNumbersReborn.Extensions;
internal static class EnemyHealthBarExtensions
{
    public static Enemy GetOwner(this EnemyHealthBar bar)
    {
        return TraverseUtils.GetValue<Enemy>(bar, "Owner");
    }

    public static Entity GetTarget(this BossHealth bossHealth)
    {
        return TraverseUtils.GetValue<Entity>(bossHealth, "target");
    }

    public static BossHealth GetBossHealth(this UIController UiController)
    {
        return TraverseUtils.GetValue<BossHealth>(UiController, "bossHealth");
    }
}
