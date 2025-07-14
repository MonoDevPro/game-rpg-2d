using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Movement;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessMovementAnimation([Data] in float deltaTime, Entity entity, ref AnimationComponent animation, in MovementComponent movement)
    {
        // IMPORTANTE: Só processa movimento se NÃO estiver atacando
        // Verifica se a entidade tem componente de ataque
        if (World.Has<AttackComponent>(entity))
        {
            ref var attack = ref World.Get<AttackComponent>(entity);
            
            // Se está atacando, não processa animação de movimento
            if (attack.IsAttacking)
                return;
        }

        // Determina o estado da animação baseado no movimento
        var newState = movement.IsMoving ? AnimationState.Move : AnimationState.Idle;
        var newDirection = movement.CurrentDirection;

        // Só atualiza se houve mudança
        if (animation.State != newState || animation.Direction != newDirection)
        {
            var animationName = GetAnimationName(newState, newDirection);

            // Calcula duração customizada baseada no tipo de animação
            float? customDuration = null;
            if (newState == AnimationState.Move && World.Has<MovementComponent>(entity))
            {
                var movementComponent = World.Get<MovementComponent>(entity);
                customDuration = AnimationConfig.GetAnimationDuration(newState, movementSpeed: movementComponent.Speed);
            }

            // Publica evento de mudança de animação
            GameEventBus.PublishAnimationChanged(new AnimationChangedEvent(
                entityId: (uint)entity.Id,
                oldState: animation.State,
                newState: newState,
                direction: newDirection,
                animationName: animationName
            ));

            PlayAnimation(ref animation, newState, newDirection, animationName, customDuration);
        }
    }

    /// <summary>
    /// Processa animações baseadas no ataque
    /// </summary>
    [Query]
    [All<AnimationComponent, AttackComponent>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessAttackAnimation(Entity entity, [Data] in float deltaTime, ref AnimationComponent animation, in AttackComponent attack)
    {
        // Se está atacando, prioriza animação de ataque
        if (attack.IsAttacking)
        {
            var newState = AnimationState.Attack;
            var newDirection = attack.AttackDirection;

            // Só atualiza se houve mudança real
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

                // Sincroniza velocidade da animação com duração do ataque
                var customDuration = AnimationConfig.GetAnimationDuration(newState, attackSpeed: attack.AttackSpeed);
                PlayAnimation(ref animation, newState, newDirection, animationName, customDuration);
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

        // Proteção contra nomes vazios
        if (string.IsNullOrEmpty(stateName))
        {
            GD.PrintErr($"Estado de animação inválido: {state}");
            stateName = "idle";
        }

        if (string.IsNullOrEmpty(directionName))
        {
            GD.PrintErr($"Direção de animação inválida: {direction}");
            directionName = "south";
        }

        var animationName = $"{stateName}_{directionName}";

        // Debug para identificar nomes vazios
        if (string.IsNullOrEmpty(animationName) || animationName == "_")
        {
            GD.PrintErr($"Nome de animação vazio gerado! Estado: '{stateName}', Direção: '{directionName}'");
            animationName = "idle_south"; // Fallback seguro
        }

        return animationName;
    }

    /// <summary>
    /// Converte direção para nome da animação
    /// </summary>
    private string GetDirectionName(Direction direction)
    {
        var directionName = direction switch
        {
            Direction.North => "north",
            Direction.NorthEast => "north", // Fallback para direção mais próxima
            Direction.East => "east",
            Direction.SouthEast => "south", // Fallback para direção mais próxima
            Direction.South => "south",
            Direction.SouthWest => "south", // Fallback para direção mais próxima
            Direction.West => "west",
            Direction.NorthWest => "north", // Fallback para direção mais próxima
            Direction.None => "south", // Fallback para None
            _ => "south" // Padrão para qualquer valor não mapeado
        };

        // Proteção extra contra string vazia
        if (string.IsNullOrEmpty(directionName))
        {
            GD.PrintErr($"GetDirectionName retornou string vazia para direção: {direction}");
            directionName = "south";
        }

        return directionName;
    }

    /// <summary>
    /// Executa a animação no sprite
    /// </summary>
    private void PlayAnimation(ref AnimationComponent animation, AnimationState state, Direction direction, string animationName, float? customDuration = null)
    {
        if (animation.Sprite == null)
        {
            GD.PrintErr("AnimationComponent.Sprite é null!");
            return;
        }

        // Proteção contra nome vazio
        if (string.IsNullOrEmpty(animationName))
        {
            GD.PrintErr($"Tentativa de reproduzir animação com nome vazio! Estado: {state}, Direção: {direction}");
            animationName = "idle_south"; // Fallback seguro
        }

        // Verifica se a animação existe
        if (animation.Sprite.SpriteFrames?.HasAnimation(animationName) == true)
        {
            // Calcula velocidade da animação baseada na duração customizada
            float speedScale = 1.0f; // Velocidade padrão

            if (customDuration.HasValue && customDuration.Value > 0)
            {
                var spriteFrames = animation.Sprite.SpriteFrames;
                var frameCount = spriteFrames.GetFrameCount(animationName);
                var originalSpeed = spriteFrames.GetAnimationSpeed(animationName);

                // Calcula duração original da animação
                var originalDuration = frameCount / originalSpeed;

                // Calcula nova velocidade para atingir a duração desejada
                speedScale = (float)(originalDuration / customDuration.Value);
            }

            // Reproduz a animação
            animation.Sprite.Play(animationName);

            // Aplica a velocidade customizada
            animation.Sprite.SpeedScale = speedScale;

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
                animation.Sprite.SpeedScale = 1.0f;

                animation.State = AnimationState.Idle;
                animation.Direction = Direction.South;
                animation.CurrentAnimation = defaultAnimation;
                animation.IsPlaying = true;
            }
            else
            {
                GD.PrintErr($"Animação padrão '{defaultAnimation}' também não encontrada!");
            }
        }
    }
}
