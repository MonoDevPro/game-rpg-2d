using System.Runtime.CompilerServices;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

[Component]
public struct MovementConfigComponent(float moveSpeed = 4f, float inputStopDelay = 0.1f)
{
    public readonly float MoveSpeed = moveSpeed;
    public readonly float InputStopDelay = inputStopDelay;
}

[Component]
public struct MovementComponent
{
    public Vector2I Direction;
    public Vector2I StartGridPosition;
    public Vector2I TargetGridPosition;
    public float Progress;
    public float TimeSinceLastInput;

    public bool IsMoving => Direction != Vector2I.Zero;
    public bool HasContinuousInput;
}

/// <summary>
/// Componente que representa a velocidade de uma entidade
/// </summary>
[Component]
public struct VelocityComponent
{
    public float X;
    public float Y;

    public VelocityComponent(float x, float y)
    {
        X = x;
        Y = y;
    }

    public VelocityComponent(Vector2 velocity)
    {
        X = velocity.X;
        Y = velocity.Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ToVector2() => new(X, Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(VelocityComponent vel) => new(vel.X, vel.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator VelocityComponent(Vector2 vec) => new(vec.X, vec.Y);
}
