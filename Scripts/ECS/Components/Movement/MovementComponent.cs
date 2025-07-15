using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Movement;

/// <summary>
/// Componente responsável pelo movimento em grid das entidades
/// </summary>
public struct MovementComponent : IComponent
{
    /// <summary>
    /// Velocidade de movimento da entidade
    /// </summary>
    public float Speed;

    /// <summary>
    /// Posição inicial do movimento atual (pixels)
    /// </summary>
    public Vector2I FromGridPosition;
    
    /// <summary>
    /// Posição de destino no grid
    /// </summary>
    public Vector2I ToGridPosition;

    /// <summary>
    /// Indica se a entidade está se movendo
    /// </summary>
    public bool IsMoving;

    /// <summary>
    /// Progresso do movimento atual (0.0 a 1.0)
    /// </summary>
    public float MoveProgress;

    /// <summary>
    /// Indica se há input contínuo de movimento
    /// </summary>
    public bool HasContinuousInput;
}
