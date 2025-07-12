using Arch.Core;
using Godot;

namespace GameRpg2D.Scripts.ECS.Events;

public readonly record struct MovementStartedEvent(Entity Entity, Vector2I Direction, Vector2I GridPosition);
public readonly record struct MovementCompletedEvent(Entity Entity, Vector2I OldGridPosition, Vector2I NewGridPosition);
public readonly record struct MovementStoppedEvent(Entity Entity, Vector2I GridPosition);