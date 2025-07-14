/*
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Bridges;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Pure;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;

namespace GameRpg2D.Scripts.ECS.Systems.Pure;

/// <summary>
/// Sistema de animação puro - sem dependências de Godot
/// </summary>
public partial class PureAnimationSystem : BaseSystem<World, float>
{
    private readonly IRenderBridge _renderBridge;

    public PureAnimationSystem(World world, IRenderBridge renderBridge) : base(world)
    {
        _renderBridge = renderBridge;
    }

    /// <summary>
    /// Processa animações baseadas no movimento
    /// </summary>
    [Query]
    [All<PureAnimationComponent, MovementComponent>]
    private void ProcessMovementAnimation(Entity entity, [Data] in float deltaTime, ref PureAnimationComponent animation, in MovementComponent movement)
    {
        // Determina estado baseado no movimento
        var newState = movement.IsMoving ? AnimationState.Move : AnimationState.Idle;
        var newDirection = movement.CurrentDirection;

        // Só atualiza se houve mudança
        if (animation.State != newState || animation.Direction != newDirection)
        {
            // Atualiza componente ECS
            animation.State = newState;
            animation.Direction = newDirection;
            animation.CurrentAnimation = GetAnimationName(newState, newDirection);
            animation.LastChangeTime = Time.GetUnixTimeFromSystem();

            // Calcula duração customizada se necessário
            if (newState == AnimationState.Move)
            {
                animation.CustomDuration = AnimationConfig.GetAnimationDuration(newState, movementSpeed: movement.Speed);
                animation.Speed = CalculateAnimationSpeed(animation.CurrentAnimation, animation.CustomDuration);
            }
            else
            {
                animation.CustomDuration = null;
                animation.Speed = 1.0f;
            }

            animation.IsPlaying = true;

            // Sincroniza com Godot via bridge
            _renderBridge.UpdateAnimation(animation.EntityId, animation);

            // Publica evento
            GameEventBus.PublishAnimationChanged(new AnimationChangedEvent(
                entityId: (uint)entity.Id,
                oldState: animation.State,
                newState: newState,
                direction: newDirection,
                animationName: animation.CurrentAnimation
            ));
        }
    }

    /// <summary>
    /// Gera nome da animação baseado no estado e direção
    /// </summary>
    private static string GetAnimationName(AnimationState state, Direction direction)
    {
        var stateName = state switch
        {
            AnimationState.Idle => "idle",
            AnimationState.Move => "move",
            AnimationState.Attack => "attack",
            _ => "idle"
        };

        var directionName = direction switch
        {
            Direction.North => "north",
            Direction.South => "south",
            Direction.East => "east",
            Direction.West => "west",
            _ => "south"
        };

        return $"{stateName}_{directionName}";
    }

    /// <summary>
    /// Calcula velocidade da animação para atingir duração customizada
    /// </summary>
    private static float CalculateAnimationSpeed(string animationName, float? customDuration)
    {
        if (!customDuration.HasValue || customDuration.Value <= 0)
            return 1.0f;

        // Esta seria uma lookup table ou calculation baseada em configuração
        // Por simplicidade, assumindo duração padrão de 1 segundo
        const float defaultDuration = 1.0f;
        return defaultDuration / customDuration.Value;
    }
}

/// <summary>
/// Sistema de input puro
/// </summary>
public partial class PureInputSystem : BaseSystem<World, float>
{
    private readonly IInputBridge _inputBridge;

    public PureInputSystem(World world, IInputBridge inputBridge) : base(world)
    {
        _inputBridge = inputBridge;
    }

    /// <summary>
    /// Atualiza input para jogadores locais
    /// </summary>
    [Query]
    [All<PureInputComponent>]
    private void UpdatePlayerInput(Entity entity, [Data] in float deltaTime, ref PureInputComponent input)
    {
        // Obtém input atual via bridge
        var currentInput = _inputBridge.GetPlayerInput(input.EntityId);
        
        // Atualiza componente
        input.InputVector = currentInput.InputVector;
        input.AttackPressed = currentInput.AttackPressed;
        input.InteractPressed = currentInput.InteractPressed;
        input.PausePressed = currentInput.PausePressed;
        input.DirectionInput = currentInput.DirectionInput;
        input.LastInputTime = currentInput.LastInputTime;
    }
}

/// <summary>
/// Sistema de física puro
/// </summary>
public partial class PurePhysicsSystem : BaseSystem<World, float>
{
    private readonly IPhysicsBridge _physicsBridge;

    public PurePhysicsSystem(World world, IPhysicsBridge physicsBridge) : base(world)
    {
        _physicsBridge = physicsBridge;
    }

    /// <summary>
    /// Atualiza dados de colisão
    /// </summary>
    [Query]
    [All<PurePhysicsComponent, MovementComponent>]
    private void UpdateCollisionData(Entity entity, [Data] in float deltaTime, ref PurePhysicsComponent physics, in MovementComponent movement)
    {
        // Atualiza posição
        physics.Position = movement.WorldPosition;

        // Verifica colisões via bridge
        physics.BlockedDirections = _physicsBridge.CheckCollisions(physics.EntityId, movement.GridPosition);
        physics.IsColliding = physics.BlockedDirections != CollisionDirections.None;

        // Atualiza última posição válida
        if (!physics.IsColliding)
        {
            physics.LastValidPosition = physics.Position;
        }
    }

    /// <summary>
    /// Valida movimento antes da execução
    /// </summary>
    [Query]
    [All<PurePhysicsComponent, MovementComponent>]
    private void ValidateMovement(Entity entity, [Data] in float deltaTime, in PurePhysicsComponent physics, ref MovementComponent movement)
    {
        if (!movement.IsMoving)
            return;

        // Testa se o movimento é válido
        bool canMove = _physicsBridge.TestMovement(
            physics.EntityId,
            physics.Position,
            movement.TargetWorldPosition
        );

        if (!canMove)
        {
            // Para o movimento se há colisão
            movement.IsMoving = false;
            movement.MoveProgress = 0.0f;

            // Publica evento de movimento bloqueado
            GameEventBus.PublishMovementBlocked(new MovementBlockedEvent(
                entityId: physics.EntityId,
                direction: movement.CurrentDirection,
                gridPosition: movement.GridPosition
            ));
        }
    }
}
*/
