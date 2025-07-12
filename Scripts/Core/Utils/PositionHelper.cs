using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.Core.Utils;

/// <summary>
/// Utilitário para conversões de posição entre grid e mundo
/// </summary>
public static class PositionHelper
{
    /// <summary>
    /// Converte posição do grid para coordenadas do mundo
    /// </summary>
    /// <param name="gridPosition">Posição no grid</param>
    /// <returns>Posição no mundo em pixels</returns>
    public static Vector2 GridToWorld(Vector2I gridPosition)
    {
        return new Vector2(
            gridPosition.X * GameConstants.GRID_SIZE,
            gridPosition.Y * GameConstants.GRID_SIZE
        );
    }

    /// <summary>
    /// Converte coordenadas do mundo para posição do grid
    /// </summary>
    /// <param name="worldPosition">Posição no mundo em pixels</param>
    /// <returns>Posição no grid</returns>
    public static Vector2I WorldToGrid(Vector2 worldPosition)
    {
        return new Vector2I(
            Mathf.RoundToInt(worldPosition.X / GameConstants.GRID_SIZE),
            Mathf.RoundToInt(worldPosition.Y / GameConstants.GRID_SIZE)
        );
    }

    /// <summary>
    /// Converte posição do grid para coordenadas do mundo (versão com Vector2)
    /// </summary>
    /// <param name="gridPosition">Posição no grid como Vector2</param>
    /// <returns>Posição no mundo em pixels</returns>
    public static Vector2 GridToWorld(Vector2 gridPosition)
    {
        return new Vector2(
            gridPosition.X * GameConstants.GRID_SIZE,
            gridPosition.Y * GameConstants.GRID_SIZE
        );
    }

    /// <summary>
    /// Converte coordenadas do mundo para posição do grid (versão com Vector2I)
    /// </summary>
    /// <param name="worldPosition">Posição no mundo em pixels</param>
    /// <returns>Posição no grid</returns>
    public static Vector2I WorldToGrid(Vector2I worldPosition)
    {
        return new Vector2I(
            worldPosition.X / GameConstants.GRID_SIZE,
            worldPosition.Y / GameConstants.GRID_SIZE
        );
    }

    /// <summary>
    /// Calcula a distância entre duas posições do grid
    /// </summary>
    /// <param name="from">Posição inicial</param>
    /// <param name="to">Posição final</param>
    /// <returns>Distância em células do grid</returns>
    public static float GridDistance(Vector2I from, Vector2I to)
    {
        return from.DistanceTo(to);
    }

    /// <summary>
    /// Calcula a distância Manhattan entre duas posições do grid
    /// </summary>
    /// <param name="from">Posição inicial</param>
    /// <param name="to">Posição final</param>
    /// <returns>Distância Manhattan em células do grid</returns>
    public static int ManhattanDistance(Vector2I from, Vector2I to)
    {
        return Mathf.Abs(from.X - to.X) + Mathf.Abs(from.Y - to.Y);
    }

    /// <summary>
    /// Verifica se uma posição está dentro dos limites do grid
    /// </summary>
    /// <param name="gridPosition">Posição no grid</param>
    /// <param name="gridSize">Tamanho do grid</param>
    /// <returns>True se a posição está dentro dos limites</returns>
    public static bool IsWithinBounds(Vector2I gridPosition, Vector2I gridSize)
    {
        return gridPosition.X >= 0 && gridPosition.X < gridSize.X &&
               gridPosition.Y >= 0 && gridPosition.Y < gridSize.Y;
    }

    /// <summary>
    /// Interpola entre duas posições do mundo
    /// </summary>
    /// <param name="from">Posição inicial</param>
    /// <param name="to">Posição final</param>
    /// <param name="weight">Peso da interpolação (0.0 a 1.0)</param>
    /// <returns>Posição interpolada</returns>
    public static Vector2 Lerp(Vector2 from, Vector2 to, float weight)
    {
        return from.Lerp(to, weight);
    }

    /// <summary>
    /// Interpola suavemente entre duas posições do mundo
    /// </summary>
    /// <param name="from">Posição inicial</param>
    /// <param name="to">Posição final</param>
    /// <param name="weight">Peso da interpolação (0.0 a 1.0)</param>
    /// <returns>Posição interpolada suavemente</returns>
    public static Vector2 SmoothStep(Vector2 from, Vector2 to, float weight)
    {
        // Smooth step function: 3t² - 2t³
        var smoothWeight = weight * weight * (3.0f - 2.0f * weight);
        return from.Lerp(to, smoothWeight);
    }

    /// <summary>
    /// Converte uma direção para um vetor de movimento no grid
    /// </summary>
    /// <param name="direction">Direção</param>
    /// <returns>Vetor de movimento no grid</returns>
    public static Vector2I DirectionToVector(Direction direction)
    {
        return direction switch
        {
            Direction.North => Vector2I.Up,
            Direction.NorthEast => Vector2I.Up + Vector2I.Right,
            Direction.East => Vector2I.Right,
            Direction.SouthEast => Vector2I.Down + Vector2I.Right,
            Direction.South => Vector2I.Down,
            Direction.SouthWest => Vector2I.Down + Vector2I.Left,
            Direction.West => Vector2I.Left,
            Direction.NorthWest => Vector2I.Up + Vector2I.Left,
            _ => Vector2I.Zero
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

    /// <summary>
    /// Converte um vetor de movimento para uma direção (versão com Vector2)
    /// </summary>
    /// <param name="vector">Vetor de movimento</param>
    /// <returns>Direção correspondente</returns>
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
    /// Obtém a próxima posição no grid baseada na direção
    /// </summary>
    /// <param name="currentPosition">Posição atual</param>
    /// <param name="direction">Direção do movimento</param>
    /// <returns>Próxima posição no grid</returns>
    public static Vector2I GetNextGridPosition(Vector2I currentPosition, Direction direction)
    {
        var directionVector = DirectionToVector(direction);
        return currentPosition + directionVector;
    }

    /// <summary>
    /// Centraliza uma posição no grid
    /// </summary>
    /// <param name="worldPosition">Posição no mundo</param>
    /// <returns>Posição centralizada no grid</returns>
    public static Vector2 CenterOnGrid(Vector2 worldPosition)
    {
        var gridPosition = WorldToGrid(worldPosition);
        return GridToWorld(gridPosition) + new Vector2(GameConstants.GRID_SIZE / 2.0f, GameConstants.GRID_SIZE / 2.0f);
    }
}
