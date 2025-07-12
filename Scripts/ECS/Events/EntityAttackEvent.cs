using GameRpg2D.Scripts.Core.Enums;

namespace GameRpg2D.Scripts.ECS.Events;

/// <summary>
/// Evento disparado quando uma entidade ataca
/// </summary>
public readonly struct EntityAttackEvent
{
    public readonly uint AttackerId;
    public readonly Direction AttackDirection;
    public readonly float Damage;
    public readonly int Range;

    public EntityAttackEvent(uint attackerId, Direction attackDirection, float damage, int range)
    {
        AttackerId = attackerId;
        AttackDirection = attackDirection;
        Damage = damage;
        Range = range;
    }
}
