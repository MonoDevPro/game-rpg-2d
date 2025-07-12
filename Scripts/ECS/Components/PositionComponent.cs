using System.Runtime.CompilerServices;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

/// <summary>
/// Componente que representa a posição de uma entidade no mundo
/// </summary>
[Component]
public struct PositionComponent
{
    public float X;
    public float Y;
    
    public PositionComponent(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public PositionComponent(Vector2 position)
    {
        X = position.X;
        Y = position.Y;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2 ToVector2() => new(X, Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(PositionComponent pos) => new(pos.X, pos.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PositionComponent(Vector2 vec) => new(vec.X, vec.Y);
}