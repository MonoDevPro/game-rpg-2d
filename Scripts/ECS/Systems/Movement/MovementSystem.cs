using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Movement
{
    /// <summary>
    /// Sistema responsável por processar o movimento de todas as entidades
    /// </summary>
    public partial class MovementSystem : BaseSystem<World, float>
    {
        public MovementSystem(World world) : base(world) { }

        /// <summary>
        /// Processa input de movimento para jogadores locais
        /// </summary>
        [Query, All<MovementComponent, InputComponent, LocalPlayerTag>]
        private void ProcessInput([Data] in float deltaTime, ref MovementComponent movement, in InputComponent input)
        {
            movement.HasContinuousInput = input.IsMovementPressed && input.MovementDirection != Direction.None;

            if (!movement.IsMoving && movement.HasContinuousInput)
            {
                var newTargetGrid = movement.GridPosition + PositionHelper.DirectionToVector(input.MovementDirection);
                movement.CurrentDirection = input.MovementDirection;
                movement.TargetGridPosition = newTargetGrid;
                movement.TargetWorldPosition = PositionHelper.GridToWorld(newTargetGrid);
                movement.StartWorldPosition = movement.WorldPosition;
                movement.IsMoving = true;
                movement.MoveProgress = 0.0f;
            }
            else if (movement.IsMoving && movement.HasContinuousInput && input.MovementDirection != movement.CurrentDirection)
            {
                movement.PendingDirection = input.MovementDirection;
            }
            else if (!movement.HasContinuousInput)
            {
                movement.PendingDirection = Direction.None;
            }
        }

        /// <summary>
        /// Processa movimento de todas as entidades
        /// </summary>
        [Query, All<MovementComponent, NodeComponent>]
        private void ProcessMovement([Data] in float deltaTime, ref MovementComponent movement, ref NodeComponent node)
        {
            if (!movement.IsMoving)
                return;

            // Calcula progresso e posição atual
            var moveDistance = movement.Speed * deltaTime;
            var totalDistance = movement.StartWorldPosition.DistanceTo(movement.TargetWorldPosition);
            var increment = totalDistance > 0 ? moveDistance / totalDistance : 1.0f;
            var newProgress = Mathf.Clamp(movement.MoveProgress + increment, 0.0f, 1.0f);
            var currentWorldPos = PositionHelper.SmoothStep(movement.StartWorldPosition, movement.TargetWorldPosition, newProgress);

            // Aplica posição ao node e componente antes de checar conclusão
            node.Node.Position = currentWorldPos;
            movement.WorldPosition = currentWorldPos;
            movement.MoveProgress = newProgress;

            if (newProgress >= 1.0f)
            {
                // Evento de movimento concluído
                GameEventBus.PublishEntityMoved(new EntityMovedEvent(
                    entityId: (uint)node.Node.GetInstanceId(),
                    fromGridPosition: movement.GridPosition,
                    toGridPosition: movement.TargetGridPosition,
                    direction: movement.CurrentDirection
                ));

                // Atualiza estado de grid
                movement.GridPosition = movement.TargetGridPosition;

                if (movement.HasContinuousInput)
                {
                    var nextDir = movement.PendingDirection != Direction.None ? movement.PendingDirection : movement.CurrentDirection;
                    var nextGrid = movement.GridPosition + PositionHelper.DirectionToVector(nextDir);
                    movement.CurrentDirection = nextDir;
                    movement.TargetGridPosition = nextGrid;
                    movement.TargetWorldPosition = PositionHelper.GridToWorld(nextGrid);
                    movement.StartWorldPosition = movement.WorldPosition;
                    movement.MoveProgress = 0.0f;
                    movement.PendingDirection = Direction.None;
                }
                else
                {
                    movement.IsMoving = false;
                }
            }
        }
    }
}
