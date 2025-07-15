using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.Core.Utils;

public static class DirectionHelper
{
    public static Direction GetDirectionFromInput(Vector2 input)
    {
        if (input.LengthSquared() == 0)
            return Direction.None;

        // Normaliza o input para determinar a direção
        var angle = Mathf.Atan2(input.Y, input.X);
        var degrees = Mathf.RadToDeg(angle);

        // Ajusta para valores positivos (0-360)
        if (degrees < 0)
            degrees += 360;

        // Determina a direção baseada no ângulo
        // Considera 8 direções com tolerância de 22.5 graus para cada
        return degrees switch
        {
            >= 337.5f or < 22.5f => Direction.East,
            >= 22.5f and < 67.5f => Direction.SouthEast,
            >= 67.5f and < 112.5f => Direction.South,
            >= 112.5f and < 157.5f => Direction.SouthWest,
            >= 157.5f and < 202.5f => Direction.West,
            >= 202.5f and < 247.5f => Direction.NorthWest,
            >= 247.5f and < 292.5f => Direction.North,
            >= 292.5f and < 337.5f => Direction.NorthEast,
            _ => Direction.None
        };
    }
    
    public static Direction VectorToDirection(Vector2 vector)
    {
        if (vector.LengthSquared() == 0) return Direction.None;

        var angle = Mathf.Atan2(vector.Y, vector.X);
        var degrees = Mathf.RadToDeg(angle);

        // Normaliza para 0-360 graus
        if (degrees < 0) degrees += 360;

        return degrees switch
        {
            >= 315 or < 45 => Direction.East,
            >= 45 and < 135 => Direction.South,
            >= 135 and < 225 => Direction.West,
            >= 225 and < 315 => Direction.North,
            _ => Direction.None
        };
    }
    
    /// <summary>
    /// Converte um vetor de movimento para uma direção
    /// </summary>
    /// <param name="vector">Vetor de movimento</param>
    /// <returns>Direção correspondente</returns>
    public static Direction VectorToDirection(Vector2I vector)
    {
        if (vector == Vector2I.Up) return Direction.North;
        if (vector == Vector2I.Up + Vector2I.Right) return Direction.NorthEast;
        if (vector == Vector2I.Right) return Direction.East;
        if (vector == Vector2I.Down + Vector2I.Right) return Direction.SouthEast;
        if (vector == Vector2I.Down) return Direction.South;
        if (vector == Vector2I.Down + Vector2I.Left) return Direction.SouthWest;
        if (vector == Vector2I.Left) return Direction.West;
        if (vector == Vector2I.Up + Vector2I.Left) return Direction.NorthWest;
        return Direction.None;
    }
}