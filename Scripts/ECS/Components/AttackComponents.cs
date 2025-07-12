using GameRpg2D.Scripts.Core.Enums;

namespace GameRpg2D.Scripts.ECS.Components;

[Component]
public struct AttackComponent
{
    public bool IsAttacking;
    public float AttackDuration;
    public float AttackTimer;
    public Direction AttackDirection;
    
    public AttackComponent(float duration = 0.5f)
    {
        IsAttacking = false;
        AttackDuration = duration;
        AttackTimer = 0.0f;
        AttackDirection = Direction.South;
    }
}

[Component]
public struct AttackConfigComponent
{
    public float AttackDuration;
    public float AttackCooldown;
    public float LastAttackTime;
    
    public AttackConfigComponent(float duration = 0.5f, float cooldown = 1.0f)
    {
        AttackDuration = duration;
        AttackCooldown = cooldown;
        LastAttackTime = 0.0f;
    }
}
