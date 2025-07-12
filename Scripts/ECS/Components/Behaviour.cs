using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

[Component]
public struct BehaviorConfigComponent(NpcBehaviourType behaviour = NpcBehaviourType.Patrol, float idleTime = 2f, float moveTime = 1.5f, float cooldown = 2f)
{
    public readonly NpcBehaviourType Type = behaviour;
    public readonly float IdleTime = idleTime;
    public readonly float MoveTime = moveTime;
    public readonly float ActionCooldown = cooldown;
}

[Component]
public struct BehaviorComponent()
{
    public float StateTimer = 0f;
    public Vector2I PatrolTarget = Vector2I.Zero;
    public float TimeSinceLastAction = 0f;
}

[Component]
public struct BehaviourComponent
{
    public NpcBehaviourType BehaviourType;
    public float ActionTimer;
    public float ActionInterval;
    public Vector2I PatrolStart;
    public Vector2I PatrolEnd;
    public bool IsPatrolling;
    public bool IsReturning;
    public Vector2I Target;
    
    public BehaviourComponent(NpcBehaviourType type, float interval = 2.0f)
    {
        BehaviourType = type;
        ActionTimer = 0.0f;
        ActionInterval = interval;
        PatrolStart = Vector2I.Zero;
        PatrolEnd = Vector2I.Zero;
        IsPatrolling = false;
        IsReturning = false;
        Target = Vector2I.Zero;
    }
}

[Component]
public struct AIStateComponent
{
    public bool IsActive;
    public float DecisionCooldown;
    public float LastDecisionTime;
    public Vector2I LastKnownPlayerPosition;
    public bool HasSeenPlayer;
    
    public AIStateComponent(bool active = true, float cooldown = 1.0f)
    {
        IsActive = active;
        DecisionCooldown = cooldown;
        LastDecisionTime = 0.0f;
        LastKnownPlayerPosition = Vector2I.Zero;
        HasSeenPlayer = false;
    }
}
