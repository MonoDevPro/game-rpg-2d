using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Movement
{
    /// <summary>
    /// Sistema responsável por processar o movimento de todas as entidades
    /// </summary>
    public partial class MovementSystem(World world) : BaseSystem<World, float>(world)
    {
        // 1) Mapeia input (local, remote ou IA) para o tween
        [Query, All<MovementComponent, MovementInputComponent, GridPositionComponent>]
        private void ApplyMovementInput(
            ref MovementComponent mv,
            in MovementInputComponent input,
            in FacingComponent facing,
            ref GridPositionComponent grid)
        {
            // Inicia movimentação se houver input e não estiver se movendo
            mv.HasContinuousInput = input.IsMoving;

            if (!mv.IsMoving && input.IsMoving)
            {
                var offset  = PositionHelper.DirectionToVector(facing.CurrentDirection);
                mv.FromGridPosition = grid.GridPosition;
                mv.ToGridPosition   = grid.GridPosition + offset;
                mv.MoveProgress     = 0f;
                mv.IsMoving         = true;
            }
        }
        
        // 2) Interpola todos os movers
        [Query, All<MovementComponent, MovementInputComponent, GridPositionComponent, TransformComponent>]
        private void ProcessMovement(
            [Data] in float delta,
            ref MovementComponent mv,
            in FacingComponent facing,
            ref GridPositionComponent grid,
            ref TransformComponent transform)
        {
            if (!mv.IsMoving)
                return;

            // 1) Calcula as posições de mundo
            var fromWorld = PositionHelper.GridToWorld(mv.FromGridPosition);
            var toWorld   = PositionHelper.GridToWorld(mv.ToGridPosition);

            // 2) Avanço do tween
            var totalDist  = fromWorld.DistanceTo(toWorld);
            var stepDist   = mv.Speed * delta;
            var t          = totalDist > 0f
                ? Mathf.Clamp(mv.MoveProgress + stepDist / totalDist, 0f, 1f)
                : 1f;

            // 3) Interpola e escreve no TransformComponent
            transform.WorldPosition = PositionHelper.SmoothStep(fromWorld, toWorld, t);
            mv.MoveProgress         = t;

            // 4) Quando chega:
            if (t >= 1f)
            {
                // 4.1) EventBus
                GameEventBus.PublishEntityMoved(new EntityMovedEvent(
                    entityId: (uint)transform.WorldPosition.GetHashCode(), // ou node.InstanceId se preferir
                    fromGridPosition: mv.FromGridPosition,
                    toGridPosition: mv.ToGridPosition,
                    direction: facing.CurrentDirection
                ));

                // 4.2) Atualiza o grid
                grid.GridPosition = mv.ToGridPosition;

                // 4.3) Próximo passo ou parar
                if (mv.HasContinuousInput)
                {
                    var offset = PositionHelper.DirectionToVector(facing.CurrentDirection);
                    mv.FromGridPosition = grid.GridPosition;
                    mv.ToGridPosition   = grid.GridPosition + offset;
                    mv.MoveProgress     = 0f;
                }
                else
                {
                    mv.IsMoving = false;
                }
            }
        }
    }
}