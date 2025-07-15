using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Facing;

public struct FacingComponent(Direction currentDirection) : IComponent
{
    /// <summary>
    /// Direção atual da entidade
    /// </summary>
    public Direction CurrentDirection = currentDirection;

    public Vector2 FacingVector => PositionHelper.DirectionToVector(CurrentDirection);
    
    public Vector2I FacingVectorI => PositionHelper.DirectionToVector(CurrentDirection);

    /// <summary>Última direção válida, mesmo ao ficar parado.</summary>
    public Direction LastDirection = currentDirection;
}