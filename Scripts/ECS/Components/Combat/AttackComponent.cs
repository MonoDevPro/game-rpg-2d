using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;

namespace GameRpg2D.Scripts.ECS.Components.Combat;

/// <summary>
/// Componente responsável pelo sistema de combate das entidades
/// </summary>
public struct AttackComponent : IComponent
{
    /// <summary>
    /// Velocidade de ataque (duração da animação)
    /// </summary>
    public float AttackSpeed;

    /// <summary>
    /// Tempo de recarga entre ataques
    /// </summary>
    public float AttackCooldown;

    /// <summary>
    /// Timestamp do último ataque
    /// </summary>
    public double LastAttackTime;

    /// <summary>
    /// Indica se está atacando atualmente
    /// </summary>
    public bool IsAttacking;

    /// <summary>
    /// Direção do ataque atual
    /// </summary>
    public Direction AttackDirection;

    /// <summary>
    /// Progresso do ataque atual (0.0 a 1.0)
    /// </summary>
    public float AttackProgress;

    /// <summary>
    /// Dano base do ataque
    /// </summary>
    public float BaseDamage;

    /// <summary>
    /// Alcance do ataque em células do grid
    /// </summary>
    public int AttackRange;
}
