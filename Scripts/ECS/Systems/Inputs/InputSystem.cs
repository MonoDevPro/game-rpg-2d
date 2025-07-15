using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Physics;
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

    private double _elapsedTime;

    public override void BeforeUpdate(in float delta)
    {
        base.BeforeUpdate(in delta);
        _elapsedTime += delta;
    }

    [Query]
    [All<MovementInputComponent, LocalPlayerTag>]
    private void UpdateMovementInput(ref MovementInputComponent mv)
    {
        // 1) Leitura de input bruto
        var up    = Input.IsActionPressed(_moveN);
        var down  = Input.IsActionPressed(_moveS);
        var left  = Input.IsActionPressed(_moveW);
        var right = Input.IsActionPressed(_moveE);

        var raw = new Vector2I(
            x: (right   ?  1 : 0) - (left  ? 1 : 0),
            y: (down    ?  1 : 0) - (up    ? 1 : 0)
        );

        // 2) Cálculos
        var isMoving            = raw.LengthSquared() > 0;
        var justStartedMovement = Input.IsActionJustPressed(_moveN)
                                  || Input.IsActionJustPressed(_moveS)
                                  || Input.IsActionJustPressed(_moveW)
                                  || Input.IsActionJustPressed(_moveE);
        
        // 3) Escrita no componente
        mv.IsMoving          = isMoving;
        mv.JustStarted       = justStartedMovement;
        mv.RawMovement       = raw;
    }
    
    [Query]
    [All<AttackInputComponent, LocalPlayerTag>]
    private void UpdateAttackInput(ref AttackInputComponent attackInput)
    {
        // 1) Ataque
        var isAttackPressed = Input.IsActionPressed(_attackAction);
        var isAttackJustPressed = Input.IsActionJustPressed(_attackAction, false);
        
        // 2) Atualiza o componente de ataque
        attackInput.JustAttacked = isAttackJustPressed;
        attackInput.IsAttacking = isAttackPressed;
    }
    
    [Query]
    [All<PointerInputComponent, NodeComponent, LocalPlayerTag>]
    private void UpdatePointerInput(ref PointerInputComponent pointerInput, in NodeComponent node)
    {
        // 1) Clique do mouse (para navegação)
        var currentPos = node.Node.GetGlobalMousePosition();
        var isClickJustPressed = Input.IsActionJustPressed(_clickAction);
        
        // 2) Atualiza o componente de input do ponteiro
        pointerInput.RawDelta = currentPos - pointerInput.PreviousWorldPosition;
        pointerInput.PreviousWorldPosition = currentPos;
        
        pointerInput.JustClicked = isClickJustPressed;
        pointerInput.ClickWorldPosition = pointerInput.JustClicked 
            ? currentPos 
            : pointerInput.ClickWorldPosition;
    }
}
