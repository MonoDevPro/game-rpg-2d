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
    {        // Se não está se movendo e há input de movimento, inicia novo movimento
        if (!movement.IsMoving && input.IsMovementPressed && input.MovementDirection != Direction.None)
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
        // Se está se movendo e não há mais input, continua o movimento atual
        else if (movement.IsMoving && !input.IsMovementPressed)
        {
            // Movimento continua até completar
        }
        // Se está se movendo e há input de nova direção, pode iniciar novo movimento após completar o atual
        else if (movement.IsMoving && input.IsMovementPressed && input.MovementDirection != movement.CurrentDirection)
        {
            // Atualiza direção para próximo movimento
            movement.CurrentDirection = input.MovementDirection;
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
            movement.IsMoving = false;
            movement.MoveProgress = 0.0f;
        }
        else
        {
            // Movimento em progresso
            movement.WorldPosition = currentWorldPosition;
            movement.MoveProgress = newProgress;
        }
    }
}
