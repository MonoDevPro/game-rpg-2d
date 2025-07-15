using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Physics;

/// <summary>
/// Sistema responsável por gerenciar colisões no ambiente grid-based
/// </summary>
public partial class CollisionSystem(World world) : BaseSystem<World, float>(world)
{
    /// <summary>
    /// Atualiza informações de colisão para todas as entidades
    /// </summary>
    [Query, All<CollisionComponent, TransformComponent>]
    private void UpdateCollisionData(
        ref CollisionComponent collision,
        in TransformComponent transform)
    {
        if (collision.Body == null)
            return;

        var gridPos = PositionHelper.WorldToGrid(transform.WorldPosition);

        collision.BlockedDirections = CollisionHelper.GetBlockedDirections(
            gridPos,
            collision.Body,
            collision.Body // exclui si mesmo
        );

        collision.IsColliding = collision.BlockedDirections != CollisionDirections.None;

        if (!collision.IsColliding)
            collision.LastValidPosition = transform.WorldPosition;
    }

    /// <summary>
    /// Antes de mover, valida se a direção pedida pelo input está bloqueada
    /// </summary>
    [Query, All<CollisionComponent, MovementInputComponent, GridPositionComponent>]
    private void ValidateMovement(
        in CollisionComponent cs,
        ref MovementInputComponent mi,
        ref FacingComponent facing,
        in GridPositionComponent grid)
    {
        if (!mi.IsMoving)
            return;

        var originalDir = facing.CurrentDirection;

        var isBlocked = CollisionHelper.IsDirectionBlocked(
            cs.BlockedDirections,
            originalDir
        );

        if (!isBlocked)
            return;

        // tenta correção
        var alt = CollisionHelper.FindAlternativeDirection(
            originalDir,
            cs.BlockedDirections
        );

        if (alt != Direction.None)
        {
            // publica evento com valores originais antes de sobrescrever
            GameEventBus.PublishMovementCorrected(new MovementCorrectedEvent(
                entityId: (uint)cs.Body.GetInstanceId(),
                originalDirection: originalDir,
                correctedDirection: alt,
                gridPosition: grid.GridPosition
            ));

            facing.CurrentDirection    = alt;
            mi.RawMovement          = PositionHelper.DirectionToVector(alt);
            mi.JustStarted             = true;
        }
        else
        {
            // sem alternativa: bloqueia de vez
            GameEventBus.PublishMovementBlocked(new MovementBlockedEvent(
                entityId: (uint)cs.Body.GetInstanceId(),
                blockedDirection: originalDir,
                gridPosition: grid.GridPosition
            ));

            mi.IsMoving          = false;
            mi.JustStarted       = false;
            mi.RawMovement       = Vector2I.Zero;
        }
    }

    /// <summary>
    /// Debug console para colisões
    /// </summary>
    [Query, All<CollisionComponent, GridPositionComponent>]
    private void DebugCollisionVisualization(
        [Data] in float deltaTime,
        in CollisionComponent collision,
        in GridPositionComponent grid)
    {
        if (!collision.EnableDebugVisualization || !collision.IsColliding)
            return;

        GD.Print($"[CollisionSystem] Entidade em {grid.GridPosition} bloqueada: {collision.BlockedDirections}");
    }
}