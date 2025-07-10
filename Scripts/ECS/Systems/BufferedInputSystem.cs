using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    /// <summary>
    /// Sistema que processa inputs buffered de jogadores remotos e os aplica ao InputComponent
    /// </summary>
    public class BufferedInputSystem(World world)
    {
        private readonly QueryDescription _remotePlayerQuery = new QueryDescription()
            .WithAll<InputComponent, BufferedInputComponent, RemotePlayerTag>();

        public void Update()
        {
            world.Query(in _remotePlayerQuery, (ref InputComponent input, ref BufferedInputComponent buffer) =>
            {
                // Tentar processar o próximo comando do buffer
                if (buffer.TryDequeueInput(out var command))
                {
                    input.InputDirection = command.Direction;
                    input.HasInput = command.Direction != Vector2I.Zero;
                    input.AttackPressed = command.Attack;
                    input.AttackJustPressed = command.Attack && !input.AttackPressed; // Simular JustPressed
                    
                    GD.Print($"Processing remote input: Direction={command.Direction}, Attack={command.Attack}");
                }
                else
                {
                    // Sem comandos no buffer - manter estado neutro
                    input.InputDirection = Vector2I.Zero;
                    input.HasInput = false;
                    input.AttackPressed = false;
                    input.AttackJustPressed = false;
                }
            });
        }
    }
}
