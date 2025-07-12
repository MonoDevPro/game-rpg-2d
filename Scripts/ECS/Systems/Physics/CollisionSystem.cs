using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Physics;

/// <summary>
/// Sistema responsável por gerenciar colisões no ambiente grid-based
/// </summary>
public partial class CollisionSystem : BaseSystem<World, float>
{
    public CollisionSystem(World world) : base(world) { }

    /// <summary>
    /// Atualiza informações de colisão para todas as entidades
    /// </summary>
    [Query]
    [All<CollisionComponent, MovementComponent, NodeComponent>]
    private void UpdateCollisionData([Data] in float deltaTime, ref CollisionComponent collision, in MovementComponent movement, in NodeComponent node)
    {
        if (collision.Body == null)
            return;

        // Atualiza direções bloqueadas
        collision.BlockedDirections = CollisionHelper.GetBlockedDirections(
            movement.GridPosition,
            collision.Body,
            collision.Body // Exclui a própria entidade
        );

        // Atualiza flag de colisão
        collision.IsColliding = collision.BlockedDirections != CollisionDirections.None;

        // Atualiza última posição válida se não está colidindo
        if (!collision.IsColliding)
        {
            collision.LastValidPosition = movement.WorldPosition;
        }
    }

    /// <summary>
    /// Valida movimento antes de ser executado
    /// </summary>
    [Query]
    [All<CollisionComponent, MovementComponent>]
    private void ValidateMovement([Data] in float deltaTime, in CollisionComponent collision, ref MovementComponent movement)
    {
        // Só valida se está tentando se mover
        if (!movement.IsMoving)
            return;

        // Verifica se a direção atual está bloqueada
        var isDirectionBlocked = CollisionHelper.IsDirectionBlocked(
            collision.BlockedDirections,
            movement.CurrentDirection
        );

        if (isDirectionBlocked)
        {
            // Movimento bloqueado - tenta encontrar alternativa
            var alternativeDirection = CollisionHelper.FindAlternativeDirection(
                movement.CurrentDirection,
                collision.BlockedDirections
            );

            if (alternativeDirection != Direction.None)
            {
                // Aplica movimento alternativo
                var newTargetGridPosition = movement.GridPosition + PositionHelper.DirectionToVector(alternativeDirection);

                movement.CurrentDirection = alternativeDirection;
                movement.TargetGridPosition = newTargetGridPosition;
                movement.TargetWorldPosition = PositionHelper.GridToWorld(newTargetGridPosition);

                // Publica evento de correção de movimento
                GameEventBus.PublishMovementCorrected(new MovementCorrectedEvent(
                    entityId: (uint)collision.Body.GetInstanceId(),
                    originalDirection: movement.CurrentDirection,
                    correctedDirection: alternativeDirection,
                    gridPosition: movement.GridPosition
                ));
            }
            else
            {
                // Nenhuma alternativa disponível - para o movimento
                movement.IsMoving = false;
                movement.MoveProgress = 0.0f;
                movement.PendingDirection = Direction.None;

                // Publica evento de movimento bloqueado
                GameEventBus.PublishMovementBlocked(new MovementBlockedEvent(
                    entityId: (uint)collision.Body.GetInstanceId(),
                    blockedDirection: movement.CurrentDirection,
                    gridPosition: movement.GridPosition
                ));
            }
        }
        else
        {
            // Movimento válido - verifica se destino final está livre
            var canMoveToTarget = CollisionHelper.CanMoveInDirection(
                movement.GridPosition,
                movement.CurrentDirection,
                collision.Body,
                collision.Body
            );

            if (!canMoveToTarget)
            {
                // Destino bloqueado após iniciar movimento - para
                movement.IsMoving = false;
                movement.MoveProgress = 0.0f;
                movement.WorldPosition = collision.LastValidPosition;

                // Publica evento de colisão durante movimento
                GameEventBus.PublishCollisionDetected(new CollisionDetectedEvent(
                    entityId: (uint)collision.Body.GetInstanceId(),
                    collisionPosition: movement.TargetWorldPosition,
                    movementDirection: movement.CurrentDirection
                ));
            }
        }
    }

    /// <summary>
    /// Atualiza posição física da entidade baseada no movimento ECS
    /// </summary>
    [Query]
    [All<CollisionComponent, MovementComponent>]
    private void SynchronizePhysicsPosition([Data] in float deltaTime, in CollisionComponent collision, in MovementComponent movement)
    {
        if (collision.Body == null)
            return;

        // Sincroniza posição do CharacterBody2D com o componente de movimento
        collision.Body.GlobalPosition = movement.WorldPosition;
    }

    /// <summary>
    /// Sistema de debug visual para colisões
    /// </summary>
    [Query]
    [All<CollisionComponent, MovementComponent>]
    private void DebugCollisionVisualization([Data] in float deltaTime, in CollisionComponent collision, in MovementComponent movement)
    {
        if (!collision.EnableDebugVisualization)
            return;

        // Debug info no console
        if (collision.IsColliding)
        {
            GD.Print($"[CollisionSystem] Entity at {movement.GridPosition} - Blocked directions: {collision.BlockedDirections}");
        }
    }
}
