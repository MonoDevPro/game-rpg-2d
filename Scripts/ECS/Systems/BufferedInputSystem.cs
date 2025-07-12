using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Utilities;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável por gerenciar input bufferizado para movimento mais fluido
/// </summary>
public partial class BufferedInputSystem : BaseSystem<World, float>
{
    public BufferedInputSystem(World world) : base(world) { }

    /// <summary>
    /// Processa input bufferizado para movimento mais responsivo
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessBufferedInput(
        [Data] in float deltaTime,
        ref BufferedInputComponent bufferedInput,
        ref MovementComponent movement,
        in LocalInputComponent input)
    {
        // Atualiza timer do buffer
        if (bufferedInput.HasBufferedInput)
        {
            bufferedInput.BufferTime += deltaTime;
            
            // Remove buffer se passou do tempo limite
            if (bufferedInput.BufferTime >= bufferedInput.MaxBufferTime)
            {
                bufferedInput.HasBufferedInput = false;
                bufferedInput.BufferedDirection = Vector2I.Zero;
                bufferedInput.BufferTime = 0.0f;
            }
        }

        // Se tem input novo, buffering
        if (input.MoveJustPressed && input.InputDirection != Vector2I.Zero)
        {
            bufferedInput.BufferedDirection = input.InputDirection;
            bufferedInput.BufferTime = 0.0f;
            bufferedInput.HasBufferedInput = true;
        }

        // Aplica movimento bufferizado se não estiver se movendo
        if (!movement.IsMoving && bufferedInput.HasBufferedInput)
        {
            var currentGridPos = GridUtils.WorldToGrid(new Vector2(0, 0)); // Precisa da posição atual
            var targetGridPos = currentGridPos + bufferedInput.BufferedDirection;
            
            movement.Direction = bufferedInput.BufferedDirection;
            movement.StartGridPosition = currentGridPos;
            movement.TargetGridPosition = targetGridPos;
            movement.Progress = 0.0f;
            
            // Limpa buffer
            bufferedInput.HasBufferedInput = false;
            bufferedInput.BufferedDirection = Vector2I.Zero;
            bufferedInput.BufferTime = 0.0f;
        }
    }

    /// <summary>
    /// Gerencia input contínuo para movimento suave
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessContinuousInput(
        [Data] in float deltaTime,
        ref MovementComponent movement,
        in LocalInputComponent input,
        in MovementConfigComponent config)
    {
        // Se o movimento acabou e ainda tem input, inicia novo movimento
        if (!movement.IsMoving && input.InputDirection != Vector2I.Zero && movement.HasContinuousInput)
        {
            movement.Direction = input.InputDirection;
            movement.Progress = 0.0f;
            movement.TimeSinceLastInput = 0.0f;
        }
    }
}