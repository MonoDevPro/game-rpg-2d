using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável por capturar input local do jogador
/// </summary>
public partial class LocalInputSystem : BaseSystem<World, float>
{
    public LocalInputSystem(World world) : base(world) { }

    /// <summary>
    /// Captura input de movimento para o player local
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CaptureMovementInput(
        ref LocalInputComponent input,
        ref MovementComponent movement)
    {
        // Captura input de movimento
        var inputDirection = Vector2I.Zero;
        
        if (Input.IsActionPressed(InputConstants.MoveUp))
            inputDirection.Y = -1;
        else if (Input.IsActionPressed(InputConstants.MoveDown))
            inputDirection.Y = 1;
        
        if (Input.IsActionPressed(InputConstants.MoveLeft))
            inputDirection.X = -1;
        else if (Input.IsActionPressed(InputConstants.MoveRight))
            inputDirection.X = 1;

        // Atualiza componente de input
        input.InputDirection = inputDirection;
        input.MoveJustPressed = inputDirection != Vector2I.Zero && !movement.IsMoving;
        
        // Processa movimento se não estiver se movendo
        if (!movement.IsMoving && input.InputDirection != Vector2I.Zero)
        {
            movement.Direction = input.InputDirection;
            movement.HasContinuousInput = true;
            movement.TimeSinceLastInput = 0.0f;
        }
        else if (input.InputDirection == Vector2I.Zero)
        {
            movement.HasContinuousInput = false;
        }
    }

    /// <summary>
    /// Captura input de ação para o player local
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CaptureActionInput(ref LocalInputComponent input)
    {
        // Captura input de ataque
        input.AttackPressed = Input.IsActionPressed(InputConstants.Attack);
        input.AttackJustPressed = Input.IsActionJustPressed(InputConstants.Attack);
    }

    /// <summary>
    /// Limpa flags de input que são apenas para um frame
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearFrameInputFlags(ref LocalInputComponent input)
    {
        // Limpa flags que são apenas para um frame
        input.MoveJustPressed = false;
        input.AttackJustPressed = false;
    }
}