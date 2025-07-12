using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Core;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Animation;

/// <summary>
/// Sistema responsável por sincronizar animações com o estado das entidades
/// </summary>
public partial class AnimationSystem : BaseSystem<World, float>
{
    public AnimationSystem(World world) : base(world) { }

    /// <summary>
    /// Processa animações baseadas no movimento
    /// </summary>
    [Query]
    [All<AnimationComponent, MovementComponent>]
    private void ProcessMovementAnimation(Entity entity, [Data] in float deltaTime, ref AnimationComponent animation, in MovementComponent movement)
    {
        // Determina o estado da animação baseado no movimento
        var newState = movement.IsMoving ? AnimationState.Walk : AnimationState.Idle;
        var newDirection = movement.CurrentDirection;

        // Só atualiza se houve mudança
        if (animation.State != newState || animation.Direction != newDirection)
        {
            var animationName = GetAnimationName(newState, newDirection);

            // Publica evento de mudança de animação
            GameEventBus.PublishAnimationChanged(new AnimationChangedEvent(
                entityId: (uint)entity.Id,
                oldState: animation.State,
                newState: newState,
                direction: newDirection,
                animationName: animationName
            ));

            PlayAnimation(ref animation, newState, newDirection, animationName);
        }
    }

    /// <summary>
    /// Processa animações baseadas no ataque
    /// </summary>
    [Query]
    [All<AnimationComponent, AttackComponent>]
    private void ProcessAttackAnimation([Data] in float deltaTime, ref AnimationComponent animation, in AttackComponent attack)
    {
        // Se está atacando, prioriza animação de ataque
        if (attack.IsAttacking)
        {
            var newState = AnimationState.Attack;
            var newDirection = attack.AttackDirection;

            if (animation.State != newState || animation.Direction != newDirection)
            {
                var animationName = GetAnimationName(newState, newDirection);
                PlayAnimation(ref animation, newState, newDirection, animationName);
            }
        }
    }

    /// <summary>
    /// Gera o nome da animação baseado no estado e direção
    /// </summary>
    private string GetAnimationName(AnimationState state, Direction direction)
    {
        var stateName = state.ToString().ToLower();
        var directionName = GetDirectionName(direction);

        return $"{stateName}_{directionName}";
    }

    /// <summary>
    /// Converte direção para nome da animação
    /// </summary>
    private string GetDirectionName(Direction direction)
    {
        return direction switch
        {
            Direction.North => "north",
            Direction.NorthEast => "north", // Fallback para direção mais próxima
            Direction.East => "east",
            Direction.SouthEast => "south", // Fallback para direção mais próxima
            Direction.South => "south",
            Direction.SouthWest => "south", // Fallback para direção mais próxima
            Direction.West => "west",
            Direction.NorthWest => "north", // Fallback para direção mais próxima
            _ => "south" // Padrão
        };
    }

    /// <summary>
    /// Executa a animação no sprite
    /// </summary>
    private void PlayAnimation(ref AnimationComponent animation, AnimationState state, Direction direction, string animationName)
    {
        if (animation.Sprite == null)
            return;

        // Verifica se a animação existe
        if (animation.Sprite.SpriteFrames?.HasAnimation(animationName) == true)
        {
            animation.Sprite.Play(animationName);

            // Atualiza o componente diretamente
            animation.State = state;
            animation.Direction = direction;
            animation.CurrentAnimation = animationName;
            animation.IsPlaying = true;
        }
        else
        {
            // Log de erro se a animação não existir
            GD.PrintErr($"Animação '{animationName}' não encontrada no SpriteFrames.");

            // Tenta animação padrão
            var defaultAnimation = "idle_south";
            if (animation.Sprite.SpriteFrames?.HasAnimation(defaultAnimation) == true)
            {
                animation.Sprite.Play(defaultAnimation);

                animation.State = AnimationState.Idle;
                animation.Direction = Direction.South;
                animation.CurrentAnimation = defaultAnimation;
                animation.IsPlaying = true;
            }
        }
    }
}
