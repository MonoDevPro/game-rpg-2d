using GameRpg2D.Scripts.Core.Enums;

namespace GameRpg2D.Scripts.ECS.Events;

/// <summary>
/// Evento disparado quando uma animação muda
/// </summary>
public readonly struct AnimationChangedEvent
{
    public readonly uint EntityId;
    public readonly AnimationState OldState;
    public readonly AnimationState NewState;
    public readonly Direction Direction;
    public readonly string AnimationName;

    public AnimationChangedEvent(uint entityId, AnimationState oldState, AnimationState newState, Direction direction, string animationName)
    {
        EntityId = entityId;
        OldState = oldState;
        NewState = newState;
        Direction = direction;
        AnimationName = animationName;
    }
}
