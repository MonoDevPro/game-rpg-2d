using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Utilities;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    public class AnimationSystem(World world)
    {
        private readonly QueryDescription _animationQuery = new QueryDescription()
            .WithAll<MovementComponent, AnimationComponent, AttackComponent>();

        public void Update()
        {
            world.Query(in _animationQuery, (ref MovementComponent movement, ref AnimationComponent animation, ref AttackComponent attack) =>
            {
                if (!IsValidAnimatedSprite(animation.AnimatedSprite))
                    return;

                // Detectar mudança de estado de ataque para forçar atualização
                bool wasAttacking = animation.CurrentAnimation.StartsWith("attack_");
                bool isCurrentlyAttacking = attack.IsAttacking;

                // Prioridade: Ataque > Movimento > Idle
                if (isCurrentlyAttacking)
                {
                    UpdateAttackAnimation(ref animation, ref attack);
                    // Atualizar direção de movimento mesmo durante ataque para quando parar
                    if (movement.IsMoving)
                    {
                        animation.LastDirection = movement.Direction;
                    }
                }
                else
                {
                    // Se estava atacando e agora não está mais, forçar atualização
                    if (wasAttacking)
                    {
                        ForceUpdateToMovementAnimation(ref animation, ref movement);
                    }
                    else
                    {
                        UpdateMovementAnimation(ref animation, ref movement);
                    }
                }
            });
        }

        private bool IsValidAnimatedSprite(AnimatedSprite2D sprite)
        {
            return sprite != null && GodotObject.IsInstanceValid(sprite);
        }

        private void ForceUpdateToMovementAnimation(ref AnimationComponent animation, ref MovementComponent movement)
        {
            // Forçar atualização da animação após o ataque terminar
            string newAnimation;
            
            if (movement.IsMoving)
            {
                newAnimation = DirectionUtils.GetAnimationName(AnimationType.Move, movement.Direction);
                animation.IsMoving = true;
                animation.LastDirection = movement.Direction;
            }
            else
            {
                // Usar a última direção para determinar a animação idle
                Vector2I idleDirection = animation.LastDirection != Vector2I.Zero ? animation.LastDirection : GameConstants.Directions.DOWN;
                newAnimation = DirectionUtils.GetAnimationName(AnimationType.Idle, idleDirection);
                animation.IsMoving = false;
            }
            
            PlayAnimation(ref animation, newAnimation, GameConstants.DEFAULT_ANIMATION_SPEED_SCALE);
            GD.Print($"Forced animation update after attack: {newAnimation}");
        }

        private void UpdateAttackAnimation(ref AnimationComponent animation, ref AttackComponent attack)
        {
            string attackAnimation = DirectionUtils.GetAnimationName(AnimationType.Attack, attack.AttackDirection);
            
            if (attackAnimation != animation.CurrentAnimation)
            {
                // Calcular velocidade baseada na duração do ataque
                float speedScale = GameConstants.DEFAULT_ANIMATION_SPEED_SCALE / attack.AttackDuration;
                PlayAnimation(ref animation, attackAnimation, speedScale);
            }
        }

        private void UpdateMovementAnimation(ref AnimationComponent animation, ref MovementComponent movement)
        {
            // Determinar se está se movendo
            bool isCurrentlyMoving = movement.IsMoving;
            
            // Se mudou o estado de movimento ou a direção, atualizar animação
            if (isCurrentlyMoving != animation.IsMoving || 
                (isCurrentlyMoving && movement.Direction != animation.LastDirection))
            {
                UpdateAnimation(ref animation, movement.Direction, isCurrentlyMoving);
            }
            
            // Atualizar estado
            animation.IsMoving = isCurrentlyMoving;
            if (isCurrentlyMoving)
            {
                animation.LastDirection = movement.Direction;
            }
        }

        private void UpdateAnimation(ref AnimationComponent animation, Vector2I direction, bool isMoving)
        {
            AnimationType animationType = isMoving ? AnimationType.Move : AnimationType.Idle;
            Vector2I targetDirection = isMoving ? direction : 
                (animation.LastDirection != Vector2I.Zero ? animation.LastDirection : GameConstants.Directions.DOWN);
            
            string newAnimation = DirectionUtils.GetAnimationName(animationType, targetDirection);
            
            // Só trocar animação se for diferente da atual
            if (newAnimation != animation.CurrentAnimation)
            {
                PlayAnimation(ref animation, newAnimation, GameConstants.DEFAULT_ANIMATION_SPEED_SCALE);
            }
            else if (!animation.AnimatedSprite.IsPlaying())
            {
                // Garantir que a animação está tocando
                animation.AnimatedSprite.Play(newAnimation);
            }
        }

        private void PlayAnimation(ref AnimationComponent animation, string animationName, float speedScale)
        {
            animation.CurrentAnimation = animationName;
            animation.AnimatedSprite.Play(animationName);
            animation.AnimatedSprite.SpeedScale = speedScale;
        }
    }
}
