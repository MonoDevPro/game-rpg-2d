using System;
using System.Collections.Generic;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.Utilities;

// Extensões para o enum Direction
public static class DirectionExtensions
{
    // Dicionário para cache de vetores (performance)
    private static readonly Dictionary<Direction, Vector2I> VectorCache = new()
    {
        { Direction.None, Vector2I.Zero },
        { Direction.North, new Vector2I(0, -1) },
        { Direction.NorthEast, new Vector2I(1, -1) },
        { Direction.East, new Vector2I(1, 0) },
        { Direction.SouthEast, new Vector2I(1, 1) },
        { Direction.South, new Vector2I(0, 1) },
        { Direction.SouthWest, new Vector2I(-1, 1) },
        { Direction.West, new Vector2I(-1, 0) },
        { Direction.NorthWest, new Vector2I(-1, -1) }
    };

    private static readonly Dictionary<Vector2I, Direction> DirectionCache = new()
    {
        { Vector2I.Zero, Direction.None },
        { new Vector2I(0, -1), Direction.North },
        { new Vector2I(1, -1), Direction.NorthEast },
        { new Vector2I(1, 0), Direction.East },
        { new Vector2I(1, 1), Direction.SouthEast },
        { new Vector2I(0, 1), Direction.South },
        { new Vector2I(-1, 1), Direction.SouthWest },
        { new Vector2I(-1, 0), Direction.West },
        { new Vector2I(-1, -1), Direction.NorthWest }
    };

    // Conversões para vetores
    public static Vector2I ToVector2I(this Direction direction)
    {
        return VectorCache.GetValueOrDefault(direction, Vector2I.Zero);
    }

    // Conversões de vetores para direções
    public static Direction ToDirection(this Vector2I vector)
    {
        return DirectionCache.GetValueOrDefault(vector, Direction.None);
    }

    // Utilidades para direções
    public static Direction GetOpposite(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.NorthEast => Direction.SouthWest,
            Direction.East => Direction.West,
            Direction.SouthEast => Direction.NorthWest,
            Direction.South => Direction.North,
            Direction.SouthWest => Direction.NorthEast,
            Direction.West => Direction.East,
            Direction.NorthWest => Direction.SouthEast,
            _ => Direction.None
        };
    }

    public static Direction RotateClockwise(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.NorthEast,
            Direction.NorthEast => Direction.East,
            Direction.East => Direction.SouthEast,
            Direction.SouthEast => Direction.South,
            Direction.South => Direction.SouthWest,
            Direction.SouthWest => Direction.West,
            Direction.West => Direction.NorthWest,
            Direction.NorthWest => Direction.North,
            _ => Direction.None
        };
    }

    public static Direction RotateCounterClockwise(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.NorthWest,
            Direction.NorthWest => Direction.West,
            Direction.West => Direction.SouthWest,
            Direction.SouthWest => Direction.South,
            Direction.South => Direction.SouthEast,
            Direction.SouthEast => Direction.East,
            Direction.East => Direction.NorthEast,
            Direction.NorthEast => Direction.North,
            _ => Direction.None
        };
    }

    public static bool IsCardinal(this Direction direction)
    {
        return direction is Direction.North or Direction.East or Direction.South or Direction.West;
    }

    public static bool IsDiagonal(this Direction direction)
    {
        return direction is Direction.NorthEast or Direction.SouthEast or Direction.SouthWest or Direction.NorthWest;
    }

    public static float ToAngle(this Direction direction)
    {
        return direction switch
        {
            Direction.East => 0f,
            Direction.SouthEast => 45f,
            Direction.South => 90f,
            Direction.SouthWest => 135f,
            Direction.West => 180f,
            Direction.NorthWest => 225f,
            Direction.North => 270f,
            Direction.NorthEast => 315f,
            _ => 0f
        };
    }

    // Obter todas as direções adjacentes
    public static IEnumerable<Direction> GetAllDirections()
    {
        return
        [
            Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast,
            Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest
        ];
    }

    public static IEnumerable<Direction> GetCardinalDirections()
    {
        return [Direction.North, Direction.East, Direction.South, Direction.West];
    }

    public static IEnumerable<Direction> GetDiagonalDirections()
    {
        return [Direction.NorthEast, Direction.SouthEast, Direction.SouthWest, Direction.NorthWest];
    }
    
    /// <summary>
    /// Obtém nome da animação baseado no tipo e direção
    /// </summary>
    public static string GetAnimationName(this AnimationState type, Vector2I direction)
    {
        string prefix = type switch
        {
            AnimationState.Attack => "attack_",
            AnimationState.Walk => "move_",
            AnimationState.Idle => "idle_",
            _ => "idle_"
        };

        string suffix = GetDirectionSuffix(direction);
        return prefix + suffix;
    }

    private static string GetDirectionSuffix(Vector2I direction)
    {
        var dir = direction.ToDirection();
        
        // Tratar direções cardinais primeiro
        if (dir == Direction.North)
            return "up";
        if (dir == Direction.South)
            return "down";
        if (dir == Direction.East)
            return "left";
        if (dir == Direction.West)
            return "right";
        
        // Para diagonais, normalizar primeiro
        var normalizedDirection = dir switch
        {
            Direction.NorthEast => "left",
            Direction.SouthEast => "left",
            Direction.SouthWest => "right",
            Direction.NorthWest => "right",
            _ => ""
        };
        return normalizedDirection;
    }
}
