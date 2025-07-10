using Arch.Core;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    public class MovementSystem(World world)
    {
        private readonly QueryDescription _movementQuery = new QueryDescription()
            .WithAll<PositionComponent, MovementComponent, InputComponent>();

        public void Update(float deltaTime)
        {
            world.Query(in _movementQuery, (ref PositionComponent position, ref MovementComponent movement, ref InputComponent input) =>
            {
                // Atualizar timer desde o último input
                if (input.HasInput)
                {
                    movement.TimeSinceLastInput = 0.0f;
                    movement.HasContinuousInput = true;
                }
                else
                {
                    movement.TimeSinceLastInput += deltaTime;
                    
                    // Parar movimento apenas após o delay
                    if (movement.TimeSinceLastInput >= movement.InputStopDelay)
                    {
                        movement.HasContinuousInput = false;
                    }
                }
                
                // Iniciar novo movimento se há input e não está se movendo
                if (movement.HasContinuousInput && input.InputDirection != Vector2I.Zero && !movement.IsMoving)
                {
                    StartMovement(ref position, ref movement, input.InputDirection);
                }
                
                // Continuar movimento atual se está se movendo
                if (movement.IsMoving)
                {
                    UpdateMovement(ref position, ref movement, deltaTime, input.InputDirection);
                }
            });
        }

        private void StartMovement(ref PositionComponent position, ref MovementComponent movement, Vector2I direction)
        {
            // Calcular nova posição no grid
            Vector2I newGridPosition = position.GridPosition + direction;
            
            // Configurar movimento
            movement.Direction = direction;
            movement.IsMoving = true;
            movement.MoveProgress = 0.0f;
            movement.StartWorldPosition = position.WorldPosition;
            movement.TargetWorldPosition = new Vector2(newGridPosition.X * GameConstants.GRID_SIZE, newGridPosition.Y * GameConstants.GRID_SIZE);
            
            // Atualizar posição no grid
            position.GridPosition = newGridPosition;
        }

        private void UpdateMovement(ref PositionComponent position, ref MovementComponent movement, float deltaTime, Vector2I inputDirection)
        {
            // Incrementar progresso do movimento
            movement.MoveProgress += movement.MoveSpeed * deltaTime;
            
            if (movement.MoveProgress >= 1.0f)
            {
                // Movimento completo
                movement.MoveProgress = 1.0f;
                position.WorldPosition = movement.TargetWorldPosition;
                
                // Decidir se continua o movimento
                if (movement.HasContinuousInput && inputDirection != Vector2I.Zero)
                {
                    // Continuar movimento na mesma direção ou nova direção
                    Vector2I nextDirection = inputDirection;
                    
                    // Calcular próxima posição
                    Vector2I nextGridPosition = position.GridPosition + nextDirection;
                    
                    // Configurar próximo movimento
                    movement.Direction = nextDirection;
                    movement.MoveProgress = 0.0f;
                    movement.StartWorldPosition = position.WorldPosition;
                    movement.TargetWorldPosition = new Vector2(nextGridPosition.X * GameConstants.GRID_SIZE, nextGridPosition.Y * GameConstants.GRID_SIZE);
                    
                    // Atualizar posição no grid
                    position.GridPosition = nextGridPosition;
                }
                else
                {
                    // Parar movimento
                    movement.IsMoving = false;
                }
            }
            else
            {
                // Interpolação suave usando ease-out para movimento mais natural
                float easedProgress = EaseOutQuad(movement.MoveProgress);
                position.WorldPosition = movement.StartWorldPosition.Lerp(movement.TargetWorldPosition, easedProgress);
            }
        }

        private float EaseOutQuad(float t)
        {
            return 1.0f - (1.0f - t) * (1.0f - t);
        }
    }
}
