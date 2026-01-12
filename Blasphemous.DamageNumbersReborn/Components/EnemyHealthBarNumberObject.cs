using Blasphemous.DamageNumbersReborn.Extensions;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class EnemyHealthBarNumberObject
{
    public GameObject gameObj;
    public EnemyHealthBar healthBar;
    public bool started = false;
    public Enemy Enemy => healthBar.GetOwner();
    public float CurrentHealth => Enemy.Stats.Life.Current;
    public float MaxHealth => Enemy.Stats.Life.Final;
    public string HealthText => $"{Main.DamageNumbersReborn.config.NumberStringFormatted(CurrentHealth)} / {Main.DamageNumbersReborn.config.NumberStringFormatted(MaxHealth)}";
    public bool ShouldShowNumber => !UIController.instance.Paused
                && !(Core.Logic.Penitent?.Dead ?? true)
                && !(Enemy?.Dead ?? true)
                && healthBar.IsEnabled;
}
