using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Input;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Input;

/// <summary>
/// Sistema responsável por processar o input apenas de jogadores locais
/// </summary>
public partial class InputSystem : BaseSystem<World, float>
{
    public InputSystem(World world) : base(world) { }


    private readonly StringName _attackAction = "attack";

    [Query]
    [All<InputComponent, LocalPlayerTag>]
    private void UpdateInput([Data] in float deltaTime, ref InputComponent input, in LocalPlayerTag playerTag)
    {
        // Captura input de movimento
        var rawInput = Vector2.Zero;

        if (Godot.Input.IsActionPressed("move_north"))
            rawInput.Y -= 1;
        if (Godot.Input.IsActionPressed("move_south"))
            rawInput.Y += 1;
        if (Godot.Input.IsActionPressed("move_west"))
            rawInput.X -= 1;
        if (Godot.Input.IsActionPressed("move_east"))
            rawInput.X += 1;

        // Determina direção baseada no input
        var direction = GetDirectionFromInput(rawInput);
        var isMovementPressed = rawInput.LengthSquared() > 0;
        var isMovementJustPressed = isMovementPressed && !input.IsMovementPressed; // Detecta início do movimento

        // Captura input de ataque
        var isAttackPressed = Godot.Input.IsActionPressed(_attackAction);
        var isAttackJustPressed = Godot.Input.IsActionJustPressed(_attackAction);        // Atualiza timestamps (usando ticks em milissegundos)
        var currentTime = Time.GetTicksMsec() / 1000.0; // Converte para segundos
        var lastMovementTime = isMovementPressed ? currentTime : input.LastMovementTime;
        var lastAttackTime = isAttackJustPressed ? currentTime : input.LastAttackTime;

        // Atualiza o componente diretamente
        input.MovementDirection = direction;
        input.IsMovementPressed = isMovementPressed;
        input.IsMovementJustPressed = isMovementJustPressed;
        input.IsAttackPressed = isAttackPressed;
        input.IsAttackJustPressed = isAttackJustPressed;
        input.RawInput = rawInput;
        input.LastMovementTime = lastMovementTime;
        input.LastAttackTime = lastAttackTime;
    }

    private Direction GetDirectionFromInput(Vector2 input)
    {
        if (input.LengthSquared() == 0)
            return Direction.None;

        // Normaliza o input para determinar a direção
        var angle = Mathf.Atan2(input.Y, input.X);
        var degrees = Mathf.RadToDeg(angle);

        // Ajusta para valores positivos (0-360)
        if (degrees < 0)
            degrees += 360;

        // Determina a direção baseada no ângulo
        // Considera 8 direções com tolerância de 22.5 graus para cada
        return degrees switch
        {
            >= 337.5f or < 22.5f => Direction.East,
            >= 22.5f and < 67.5f => Direction.SouthEast,
            >= 67.5f and < 112.5f => Direction.South,
            >= 112.5f and < 157.5f => Direction.SouthWest,
            >= 157.5f and < 202.5f => Direction.West,
            >= 202.5f and < 247.5f => Direction.NorthWest,
            >= 247.5f and < 292.5f => Direction.North,
            >= 292.5f and < 337.5f => Direction.NorthEast,
            _ => Direction.None
        };
    }
}
