using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Physics;
using Godot;

namespace GameRpg2D.Scripts.Core.Utils;

/// <summary>
/// Utilitário para operações de colisão no sistema grid-based
/// </summary>
public static class CollisionHelper
{
    /// <summary>
    /// Verifica se uma posição no grid está livre para movimento
    /// </summary>
    /// <param name="gridPosition">Posição no grid a verificar</param>
    /// <param name="body">CharacterBody2D da entidade</param>
    /// <param name="excludeEntity">Entidade para excluir da verificação (normalmente a própria entidade)</param>
    /// <returns>True se a posição está livre</returns>
    public static bool IsGridPositionFree(Vector2I gridPosition, CharacterBody2D body, CharacterBody2D excludeEntity = null)
    {
        var worldPosition = PositionHelper.GridToWorld(gridPosition);
        return IsWorldPositionFree(worldPosition, body, excludeEntity);
    }

    /// <summary>
    /// Verifica se uma posição no mundo está livre para movimento
    /// </summary>
    /// <param name="worldPosition">Posição no mundo a verificar</param>
    /// <param name="body">CharacterBody2D da entidade</param>
    /// <param name="excludeEntity">Entidade para excluir da verificação</param>
    /// <returns>True se a posição está livre</returns>
    public static bool IsWorldPositionFree(Vector2 worldPosition, CharacterBody2D body, CharacterBody2D excludeEntity = null)
    {
        // Verifica colisão usando test_move do Godot
        Vector2 motion = worldPosition - body.GlobalPosition;
        return !body.TestMove(body.Transform, motion);
    }

    /// <summary>
    /// Verifica se movimento em uma direção é possível
    /// </summary>
    /// <param name="currentGridPosition">Posição atual no grid</param>
    /// <param name="direction">Direção do movimento</param>
    /// <param name="body">CharacterBody2D da entidade</param>
    /// <param name="excludeEntity">Entidade para excluir da verificação</param>
    /// <returns>True se o movimento é possível</returns>
    public static bool CanMoveInDirection(Vector2I currentGridPosition, Direction direction, CharacterBody2D body, CharacterBody2D excludeEntity = null)
    {
        if (direction == Direction.None)
            return false;

        var targetGridPosition = currentGridPosition + PositionHelper.DirectionToVector(direction);
        return IsGridPositionFree(targetGridPosition, body, excludeEntity);
    }

    /// <summary>
    /// Obtém todas as direções bloqueadas a partir de uma posição
    /// </summary>
    /// <param name="gridPosition">Posição no grid</param>
    /// <param name="body">CharacterBody2D da entidade</param>
    /// <param name="excludeEntity">Entidade para excluir da verificação</param>
    /// <returns>Flags das direções bloqueadas</returns>
    public static CollisionDirections GetBlockedDirections(Vector2I gridPosition, CharacterBody2D body, CharacterBody2D excludeEntity = null)
    {
        var blockedDirections = CollisionDirections.None;

        // Verifica cada direção
        var directions = new[]
        {
            (Direction.North, CollisionDirections.North),
            (Direction.South, CollisionDirections.South),
            (Direction.East, CollisionDirections.East),
            (Direction.West, CollisionDirections.West),
            (Direction.NorthEast, CollisionDirections.NorthEast),
            (Direction.NorthWest, CollisionDirections.NorthWest),
            (Direction.SouthEast, CollisionDirections.SouthEast),
            (Direction.SouthWest, CollisionDirections.SouthWest)
        };

        foreach (var (direction, flag) in directions)
        {
            if (!CanMoveInDirection(gridPosition, direction, body, excludeEntity))
            {
                blockedDirections |= flag;
            }
        }

        return blockedDirections;
    }

    /// <summary>
    /// Converte Direction enum para CollisionDirections flag
    /// </summary>
    /// <param name="direction">Direção do movimento</param>
    /// <returns>Flag correspondente</returns>
    public static CollisionDirections DirectionToCollisionFlag(Direction direction)
    {
        return direction switch
        {
            Direction.North => CollisionDirections.North,
            Direction.South => CollisionDirections.South,
            Direction.East => CollisionDirections.East,
            Direction.West => CollisionDirections.West,
            Direction.NorthEast => CollisionDirections.NorthEast,
            Direction.NorthWest => CollisionDirections.NorthWest,
            Direction.SouthEast => CollisionDirections.SouthEast,
            Direction.SouthWest => CollisionDirections.SouthWest,
            _ => CollisionDirections.None
        };
    }

    /// <summary>
    /// Verifica se uma direção específica está bloqueada
    /// </summary>
    /// <param name="blockedDirections">Flags das direções bloqueadas</param>
    /// <param name="direction">Direção a verificar</param>
    /// <returns>True se a direção está bloqueada</returns>
    public static bool IsDirectionBlocked(CollisionDirections blockedDirections, Direction direction)
    {
        var flag = DirectionToCollisionFlag(direction);
        return (blockedDirections & flag) == flag;
    }

    /// <summary>
    /// Encontra a direção mais próxima que não está bloqueada
    /// </summary>
    /// <param name="desiredDirection">Direção desejada</param>
    /// <param name="blockedDirections">Direções bloqueadas</param>
    /// <returns>Direção alternativa ou Direction.None se todas estão bloqueadas</returns>
    public static Direction FindAlternativeDirection(Direction desiredDirection, CollisionDirections blockedDirections)
    {
        // Se a direção desejada não está bloqueada, retorna ela
        if (!IsDirectionBlocked(blockedDirections, desiredDirection))
            return desiredDirection;

        // Tenta direções adjacentes baseadas na direção desejada
        var alternatives = desiredDirection switch
        {
            Direction.North => new[] { Direction.NorthEast, Direction.NorthWest, Direction.East, Direction.West },
            Direction.South => new[] { Direction.SouthEast, Direction.SouthWest, Direction.East, Direction.West },
            Direction.East => new[] { Direction.NorthEast, Direction.SouthEast, Direction.North, Direction.South },
            Direction.West => new[] { Direction.NorthWest, Direction.SouthWest, Direction.North, Direction.South },
            Direction.NorthEast => new[] { Direction.North, Direction.East, Direction.NorthWest, Direction.SouthEast },
            Direction.NorthWest => new[] { Direction.North, Direction.West, Direction.NorthEast, Direction.SouthWest },
            Direction.SouthEast => new[] { Direction.South, Direction.East, Direction.NorthEast, Direction.SouthWest },
            Direction.SouthWest => new[] { Direction.South, Direction.West, Direction.NorthWest, Direction.SouthEast },
            _ => new Direction[0]
        };

        // Retorna a primeira direção não bloqueada
        foreach (var alternative in alternatives)
        {
            if (!IsDirectionBlocked(blockedDirections, alternative))
                return alternative;
        }

        return Direction.None;
    }
}
