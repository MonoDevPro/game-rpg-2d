using System;
using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Constants;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    /// <summary>
    /// Sistema de IA que controla comportamento de NPCs
    /// </summary>
    public class AISystem(World world)
    {
        private readonly QueryDescription _npcQuery = new QueryDescription()
            .WithAll<InputComponent, BehaviorComponent, NPCTag, PositionComponent>();

        private readonly Random _random = new();

        public void Update(float deltaTime)
        {
            world.Query(in _npcQuery, (ref InputComponent input, ref BehaviorComponent behavior, ref PositionComponent position) =>
            {
                behavior.StateTimer += deltaTime;
                behavior.TimeSinceLastAction += deltaTime;

                Vector2I decidedDirection = Vector2I.Zero;
                bool shouldAttack = false;

                switch (behavior.BehaviorType)
                {
                    case NPCBehaviorType.Idle:
                        decidedDirection = HandleIdleBehavior(ref behavior);
                        break;
                        
                    case NPCBehaviorType.Wander:
                        decidedDirection = HandleWanderBehavior(ref behavior, position);
                        break;
                        
                    case NPCBehaviorType.Patrol:
                        decidedDirection = HandlePatrolBehavior(ref behavior, position);
                        break;
                        
                    case NPCBehaviorType.Aggressive:
                        (decidedDirection, shouldAttack) = HandleAggressiveBehavior(ref behavior, position);
                        break;
                }

                // Aplicar decisão ao InputComponent
                input.InputDirection = decidedDirection;
                input.HasInput = decidedDirection != Vector2I.Zero;
                input.AttackPressed = shouldAttack;
                input.AttackJustPressed = shouldAttack && !input.AttackPressed;
                
                if (decidedDirection != Vector2I.Zero)
                {
                    behavior.LastDirection = decidedDirection;
                }
            });
        }

        private Vector2I HandleIdleBehavior(ref BehaviorComponent behavior)
        {
            // Ficar parado na maior parte do tempo, ocasionalmente mover-se
            if (behavior.StateTimer > behavior.IdleTime)
            {
                behavior.StateTimer = 0.0f;
                // 20% chance de se mover aleatoriamente
                if (_random.NextDouble() < 0.2)
                {
                    return GetRandomDirection();
                }
            }
            return Vector2I.Zero;
        }

        private Vector2I HandleWanderBehavior(ref BehaviorComponent behavior, PositionComponent position)
        {
            // Alternar entre mover e parar
            if (behavior.StateTimer > behavior.MoveTime)
            {
                behavior.StateTimer = 0.0f;
                return GetRandomDirection();
            }
            
            return Vector2I.Zero;
        }

        private Vector2I HandlePatrolBehavior(ref BehaviorComponent behavior, PositionComponent position)
        {
            // Implementação simples de patrulha
            if (behavior.PatrolTarget == Vector2I.Zero)
            {
                // Definir primeiro target de patrulha
                behavior.PatrolTarget = position.GridPosition + GetRandomDirection() * 3;
            }

            // Mover em direção ao target
            Vector2I direction = (behavior.PatrolTarget - position.GridPosition).Sign();
            
            // Se chegou ao target, definir novo target
            if (position.GridPosition == behavior.PatrolTarget)
            {
                behavior.PatrolTarget = position.GridPosition + GetRandomDirection() * 3;
            }

            return direction;
        }

        private (Vector2I direction, bool attack) HandleAggressiveBehavior(ref BehaviorComponent behavior, PositionComponent position)
        {
            // Tentar encontrar o jogador (implementação básica)
            // Por enquanto, comportamento similar ao wander mas com ataques ocasionais
            Vector2I moveDirection = Vector2I.Zero;
            bool shouldAttack = false;

            if (behavior.StateTimer > behavior.MoveTime)
            {
                behavior.StateTimer = 0.0f;
                moveDirection = GetRandomDirection();
            }

            // Atacar ocasionalmente
            if (behavior.TimeSinceLastAction > behavior.ActionCooldown)
            {
                behavior.TimeSinceLastAction = 0.0f;
                shouldAttack = _random.NextDouble() < 0.3; // 30% chance de atacar
            }

            return (moveDirection, shouldAttack);
        }

        private Vector2I GetRandomDirection()
        {
            var directions = new[]
            {
                GameConstants.Directions.UP,
                GameConstants.Directions.DOWN,
                GameConstants.Directions.LEFT,
                GameConstants.Directions.RIGHT,
                Vector2I.Zero // Incluir possibilidade de não se mover
            };

            return directions[_random.Next(directions.Length)];
        }
    }
}
