using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Events;

/// <summary>
/// Evento disparado quando movimento é corrigido por colisão
/// </summary>
public readonly struct MovementCorrectedEvent
{
    public readonly uint EntityId;
    public readonly Direction OriginalDirection;
    public readonly Direction CorrectedDirection;
    public readonly Vector2I GridPosition;

    public MovementCorrectedEvent(uint entityId, Direction originalDirection, Direction correctedDirection, Vector2I gridPosition)
    {
        EntityId = entityId;
        OriginalDirection = originalDirection;
        CorrectedDirection = correctedDirection;
        GridPosition = gridPosition;
    }
}

/// <summary>
/// Evento disparado quando movimento é completamente bloqueado
/// </summary>
public readonly struct MovementBlockedEvent
{
    public readonly uint EntityId;
    public readonly Direction BlockedDirection;
    public readonly Vector2I GridPosition;

    public MovementBlockedEvent(uint entityId, Direction blockedDirection, Vector2I gridPosition)
    {
        EntityId = entityId;
        BlockedDirection = blockedDirection;
        GridPosition = gridPosition;
    }
}

/// <summary>
/// Evento disparado quando colisão é detectada durante movimento
/// </summary>
public readonly struct CollisionDetectedEvent
{
    public readonly uint EntityId;
    public readonly Vector2 CollisionPosition;
    public readonly Direction MovementDirection;

    public CollisionDetectedEvent(uint entityId, Vector2 collisionPosition, Direction movementDirection)
    {
        EntityId = entityId;
        CollisionPosition = collisionPosition;
        MovementDirection = movementDirection;
    }
}
