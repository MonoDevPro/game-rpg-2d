using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Inputs;

/// <summary>
/// Sistema responsável por processar o input apenas de jogadores locais
/// </summary>
public partial class InputSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly StringName _attackAction = "attack";
    private readonly StringName _clickAction = "click";
    private readonly StringName _moveN = "move_north";
    private readonly StringName _moveS = "move_south";
    private readonly StringName _moveW = "move_west";
    private readonly StringName _moveE = "move_east";

    private double _elapsedTime = 0.0;

    public override void BeforeUpdate(in float delta)
    {
        base.BeforeUpdate(in delta);
        _elapsedTime += delta;
    }


    [Query]
    [All<InputComponent, LocalPlayerTag>]
    private void UpdateInput(ref InputComponent input)
    {
        // 1) Movimento
        var rawInput = Vector2.Zero;
        if (Input.IsActionPressed(_moveN)) rawInput.Y -= 1;
        if (Input.IsActionPressed(_moveS)) rawInput.Y += 1;
        if (Input.IsActionPressed(_moveW)) rawInput.X -= 1;
        if (Input.IsActionPressed(_moveE)) rawInput.X += 1;

        var direction = GetDirectionFromInput(rawInput);
        var isMovementPressed = rawInput.LengthSquared() > 0;
        var isMovementJustPressed = isMovementPressed && !input.IsMovementPressed;

        // 2) Ataque
        var isAttackPressed = Input.IsActionPressed(_attackAction);
        var isAttackJustPressed = Input.IsActionJustPressed(_attackAction);

        // 3) Clique do mouse (para navegação)
        var isClickJustPressed = Input.IsActionJustPressed(_clickAction);

        // 4) Timestamps usando acumulador
        var lastMovementTime = isMovementPressed ? _elapsedTime : input.LastMovementTime;
        var lastAttackTime = isAttackJustPressed ? _elapsedTime : input.LastAttackTime;

        // 5) Atualiza componente
        input.MovementDirection = direction;
        input.IsMovementPressed = isMovementPressed;
        input.IsMovementJustPressed = isMovementJustPressed;
        input.IsAttackPressed = isAttackPressed;
        input.IsAttackJustPressed = isAttackJustPressed;
        input.IsClickJustPressed = isClickJustPressed;
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
