using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Core;
using GameRpg2D.Scripts.ECS.Components.Input;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Movement;

/// <summary>
/// Sistema responsável por processar o movimento de todas as entidades
/// </summary>
public partial class MovementSystem : BaseSystem<World, float>
{
    public MovementSystem(World world) : base(world) { }

    /// <summary>
    /// Processa input de movimento para jogadores locais
    /// </summary>
    [Query]
    [All<MovementComponent, InputComponent, LocalPlayerTag>]
    private void ProcessInput([Data] in float deltaTime, ref MovementComponent movement, in InputComponent input, in LocalPlayerTag playerTag)
    {
        // Atualiza flag de input contínuo
        movement.HasContinuousInput = input.IsMovementPressed && input.MovementDirection != Direction.None;

        // Se não está se movendo e há input de movimento, inicia novo movimento
        if (!movement.IsMoving && movement.HasContinuousInput)
        {
            var newTargetGridPosition = movement.GridPosition + PositionHelper.DirectionToVector(input.MovementDirection);
            var newTargetWorldPosition = PositionHelper.GridToWorld(newTargetGridPosition);

            movement.CurrentDirection = input.MovementDirection;
            movement.TargetGridPosition = newTargetGridPosition;
            movement.TargetWorldPosition = newTargetWorldPosition;
            movement.StartWorldPosition = movement.WorldPosition;
            movement.IsMoving = true;
            movement.MoveProgress = 0.0f;
        }
        // Se está se movendo e há input de nova direção, armazena para próximo movimento
        else if (movement.IsMoving && movement.HasContinuousInput && input.MovementDirection != movement.CurrentDirection)
        {
            movement.PendingDirection = input.MovementDirection;
        }
        // Se parou de pressionar, limpa input contínuo
        else if (!movement.HasContinuousInput)
        {
            movement.PendingDirection = Direction.None;
        }
    }

    /// <summary>
    /// Processa movimento de todas as entidades
    /// </summary>
    [Query]
    [All<MovementComponent, NodeComponent>]
    private void ProcessMovement([Data] in float deltaTime, ref MovementComponent movement, ref NodeComponent node)
    {
        if (!movement.IsMoving)
            return;

        // Calcula novo progresso do movimento
        var moveDistance = movement.Speed * deltaTime;
        var totalDistance = movement.WorldPosition.DistanceTo(movement.TargetWorldPosition);
        var progressIncrement = totalDistance > 0 ? moveDistance / totalDistance : 1.0f;

        var newProgress = Mathf.Clamp(movement.MoveProgress + progressIncrement, 0.0f, 1.0f);        // Interpola posição atual
        var currentWorldPosition = PositionHelper.SmoothStep(movement.StartWorldPosition, movement.TargetWorldPosition, newProgress);

        // Atualiza posição do nó
        node.Node.Position = currentWorldPosition;

        // Verifica se o movimento foi concluído
        if (newProgress >= 1.0f)
        {
            // Publica evento de movimento concluído
            GameEventBus.PublishEntityMoved(new EntityMovedEvent(
                entityId: (uint)node.Node.GetInstanceId(),
                fromGridPosition: movement.GridPosition,
                toGridPosition: movement.TargetGridPosition,
                direction: movement.CurrentDirection
            ));

            // Movimento concluído
            movement.GridPosition = movement.TargetGridPosition;
            movement.WorldPosition = movement.TargetWorldPosition;
            movement.MoveProgress = 0.0f;

            // Verifica se deve continuar movimento contínuo
            if (movement.HasContinuousInput)
            {
                // Se há direção pendente, usa ela; senão continua na mesma direção
                var nextDirection = movement.PendingDirection != Direction.None ? movement.PendingDirection : movement.CurrentDirection;

                // Inicia próximo movimento imediatamente
                var newTargetGridPosition = movement.GridPosition + PositionHelper.DirectionToVector(nextDirection);
                var newTargetWorldPosition = PositionHelper.GridToWorld(newTargetGridPosition);

                movement.CurrentDirection = nextDirection;
                movement.TargetGridPosition = newTargetGridPosition;
                movement.TargetWorldPosition = newTargetWorldPosition;
                movement.StartWorldPosition = movement.WorldPosition;
                movement.IsMoving = true;
                movement.MoveProgress = 0.0f;
                movement.PendingDirection = Direction.None;
            }
            else
            {
                // Para o movimento
                movement.IsMoving = false;
            }
        }
        else
        {
            // Movimento em progresso
            movement.WorldPosition = currentWorldPosition;
            movement.MoveProgress = newProgress;
        }
    }
}
