using GameRpg2D.Scripts.ECS.Components.AI;
using Godot;

namespace GameRpg2D.Scripts.ECS.Events;

/// <summary>
/// Evento disparado quando NPC alcança um waypoint
/// </summary>
public readonly struct PatrolWaypointReachedEvent
{
    public readonly uint EntityId;
    public readonly Vector2I WayPoint;
    public readonly int WayPointIndex;
    public readonly PatrolState NewState;

    public PatrolWaypointReachedEvent(uint entityId, Vector2I wayPoint, int wayPointIndex, PatrolState newState)
    {
        EntityId = entityId;
        WayPoint = wayPoint;
        WayPointIndex = wayPointIndex;
        NewState = newState;
    }
}

/// <summary>
/// Evento disparado quando estado da patrulha muda
/// </summary>
public readonly struct PatrolStateChangedEvent
{
    public readonly uint EntityId;
    public readonly PatrolState OldState;
    public readonly PatrolState NewState;
    public readonly Vector2I CurrentWayPoint;

    public PatrolStateChangedEvent(uint entityId, PatrolState oldState, PatrolState newState, Vector2I currentWayPoint)
    {
        EntityId = entityId;
        OldState = oldState;
        NewState = newState;
        CurrentWayPoint = currentWayPoint;
    }
}

/// <summary>
/// Evento disparado quando patrulha é completada
/// </summary>
public readonly struct PatrolCompletedEvent
{
    public readonly uint EntityId;
    public readonly Vector2I FinalWayPoint;
    public readonly bool WillRestart;

    public PatrolCompletedEvent(uint entityId, Vector2I finalWayPoint, bool willRestart)
    {
        EntityId = entityId;
        FinalWayPoint = finalWayPoint;
        WillRestart = willRestart;
    }
}

/// <summary>
/// Evento disparado quando patrulha é pausada/interrompida
/// </summary>
public readonly struct PatrolInterruptedEvent
{
    public readonly uint EntityId;
    public readonly PatrolState PreviousState;
    public readonly Vector2I CurrentPosition;
    public readonly string Reason;

    public PatrolInterruptedEvent(uint entityId, PatrolState previousState, Vector2I currentPosition, string reason)
    {
        EntityId = entityId;
        PreviousState = previousState;
        CurrentPosition = currentPosition;
        Reason = reason;
    }
}
