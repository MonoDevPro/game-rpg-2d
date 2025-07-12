using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Core;

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
    /// Direção atual da entidade
    /// </summary>
    public Direction CurrentDirection;

    /// <summary>
    /// Posição atual no grid
    /// </summary>
    public Vector2I GridPosition;

    /// <summary>
    /// Posição de destino no grid
    /// </summary>
    public Vector2I TargetGridPosition;

    /// <summary>
    /// Posição atual no mundo (pixels)
    /// </summary>
    public Vector2 WorldPosition;

    /// <summary>
    /// Posição de destino no mundo (pixels)
    /// </summary>
    public Vector2 TargetWorldPosition;

    /// <summary>
    /// Posição inicial do movimento atual (pixels)
    /// </summary>
    public Vector2 StartWorldPosition;

    /// <summary>
    /// Posição de destino (alias para TargetWorldPosition)
    /// </summary>
    public Vector2 TargetPosition
    {
        get => TargetWorldPosition;
        set => TargetWorldPosition = value;
    }

    /// <summary>
    /// Indica se a entidade está se movendo
    /// </summary>
    public bool IsMoving;

    /// <summary>
    /// Progresso do movimento atual (0.0 a 1.0)
    /// </summary>
    public float MoveProgress;

    /// <summary>
    /// Direção pendente para o próximo movimento
    /// </summary>
    public Direction PendingDirection;

    /// <summary>
    /// Indica se há input contínuo de movimento
    /// </summary>
    public bool HasContinuousInput;
}
