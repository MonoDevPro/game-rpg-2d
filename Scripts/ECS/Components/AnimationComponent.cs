using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

[Component]
public struct AnimationComponent
{
    public AnimationState CurrentState;
    public AnimationState PreviousState;
    public Direction CurrentDirection;
    public Direction PreviousDirection;
    public float AnimationSpeed;
    public bool IsPlaying;
    public bool HasChanged;
    
    public AnimationComponent(AnimationState state = AnimationState.Idle, Direction direction = Direction.South, float speed = 1.0f)
    {
        CurrentState = state;
        PreviousState = state;
        CurrentDirection = direction;
        PreviousDirection = direction;
        AnimationSpeed = speed;
        IsPlaying = false;
        HasChanged = false;
    }
}

[Component]
public struct AnimatedSpriteComponent
{
    public AnimatedSprite2D AnimatedSprite;
    public bool AutoPlay;
    
    public AnimatedSpriteComponent(AnimatedSprite2D sprite, bool autoPlay = true)
    {
        AnimatedSprite = sprite;
        AutoPlay = autoPlay;
    }
}