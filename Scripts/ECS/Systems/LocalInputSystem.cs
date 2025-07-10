using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    /// <summary>
    /// Sistema responsável por capturar input do dispositivo local para jogadores locais
    /// </summary>
    public class LocalInputSystem(World world)
    {
        private readonly QueryDescription _localPlayerQuery = new QueryDescription()
            .WithAll<InputComponent, LocalPlayerTag>();

        public void Update()
        {
            Vector2I inputDirection = Vector2I.Zero;
            bool hasInput = false;
            bool attackPressed = Input.IsActionPressed("attack");
            bool attackJustPressed = Input.IsActionJustPressed("attack");

            // Verificar entrada para as 8 direções
            if (Input.IsActionPressed("move_up"))
            {
                inputDirection.Y -= 1;
                hasInput = true;
            }
            if (Input.IsActionPressed("move_down"))
            {
                inputDirection.Y += 1;
                hasInput = true;
            }
            if (Input.IsActionPressed("move_left"))
            {
                inputDirection.X -= 1;
                hasInput = true;
            }
            if (Input.IsActionPressed("move_right"))
            {
                inputDirection.X += 1;
                hasInput = true;
            }

            // Normalizar direções diagonais para manter consistência
            if (inputDirection.X != 0 && inputDirection.Y != 0)
            {
                inputDirection = new Vector2I(
                    Mathf.Clamp(inputDirection.X, -1, 1),
                    Mathf.Clamp(inputDirection.Y, -1, 1)
                );
            }

            // Atualizar APENAS entidades com LocalPlayerTag
            world.Query(in _localPlayerQuery, (ref InputComponent input) =>
            {
                input.InputDirection = inputDirection;
                input.HasInput = hasInput;
                input.AttackPressed = attackPressed;
                input.AttackJustPressed = attackJustPressed;
            });
        }
    }
}
