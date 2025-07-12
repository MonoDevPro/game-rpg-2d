using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Events;

/// <summary>
/// Evento disparado quando uma entidade se move
/// </summary>
public readonly struct EntityMovedEvent
{
    public readonly uint EntityId;
    public readonly Vector2I FromGridPosition;
    public readonly Vector2I ToGridPosition;
    public readonly Direction Direction;

    public EntityMovedEvent(uint entityId, Vector2I fromGridPosition, Vector2I toGridPosition, Direction direction)
    {
        EntityId = entityId;
        FromGridPosition = fromGridPosition;
        ToGridPosition = toGridPosition;
        Direction = direction;
    }
}
