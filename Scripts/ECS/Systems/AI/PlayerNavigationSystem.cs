using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.AI;

/// <summary>
/// Sistema responsável por integrar cliques do mouse com navegação inteligente
/// Permite navegação por clique enquanto mantém movimento direto por teclado
/// </summary>
public partial class PlayerNavigationSystem(World world) : BaseSystem<World, float>(world)
{
    /// <summary>
    /// Processa cliques do mouse para navegação
    /// </summary>
    [Query]
    [All<InputComponent, NavigationComponent, MovementComponent, NodeComponent, LocalPlayerTag>]
    private void ProcessMouseNavigation([Data] in float deltaTime,
        in InputComponent input,
        ref NavigationComponent navigation,
        ref MovementComponent movement,
        in NodeComponent node)
    {
        // Atualiza posição atual do navegation component
        navigation.GridPosition = movement.GridPosition;

        // Verifica se houve clique do mouse
        if (input.IsClickJustPressed)
        {
            navigation.IsEnabled = true;
            navigation.PathFound = false;
            navigation.PathGridPositions?.Clear();
            
            // Obtém posição do mouse no mundo
            var mousePosition = node.Node.GetGlobalMousePosition();
            var targetGridPosition = PositionHelper.WorldToGrid(mousePosition);

            GD.Print($"[PlayerNavigationSystem] Clique detectado em {mousePosition} -> Grid: {targetGridPosition}");

            if (movement.GridPosition.DistanceTo(targetGridPosition) > navigation.ReachGridTolerance )
                navigation.TargetGridPosition = targetGridPosition;
            else
                navigation.IsEnabled = false;
        }
        // Teclado (WASD) cancela navegação atual
        else if (input.IsMovementPressed && input.MovementDirection != Direction.None)
        {
            CancelNavigation(ref navigation, ref movement);
            GD.Print($"[PlayerNavigationSystem] Navegação cancelada por input de teclado");
        }
    }
    [Query]
    [All<NavigationComponent, MovementComponent, InputComponent, LocalPlayerTag>]
    private void ApplyNavigationToMovement(
        [Data] in float deltaTime,
        in NavigationComponent navigation,
        ref MovementComponent movement,
        in InputComponent input)
    {
        // 1) Se houver input de teclado, sai: teclado tem prioridade.
        if (input.IsMovementPressed)
            return;

        // 2) Se não há caminho pronto (PathFound == false) ou já estamos em movimento, sai.
        //    Isso evita sobrescrever um movimento em andamento.
        if (!navigation.PathFound || movement.IsMoving)
            return;

        // 3) Se chegamos até aqui, temos um caminho calculado E não há movimento ativo:
        //    vamos aplicar a próxima direção.
        if (navigation.TargetNextDirection != Direction.None)
        {
            // 3.1) Computa a próxima posição-alvo em grid,
            //      somando a direção ao grid atual.
            var newTargetGrid = navigation.GridPosition
                                + PositionHelper.DirectionToVector(navigation.TargetNextDirection);

            // 3.2) Ajusta o MovementComponent para iniciar a transição:
            movement.CurrentDirection   = navigation.TargetNextDirection;
            movement.TargetGridPosition = newTargetGrid;
            movement.TargetWorldPosition= PositionHelper.GridToWorld(newTargetGrid);
            movement.StartWorldPosition = movement.WorldPosition;
            movement.IsMoving           = true;
            movement.MoveProgress       = 0.0f;

            // 3.3) Flags de estado:
            movement.HasContinuousInput   = true;  // movimento automático contínuo
            movement.IsNavigationMovement = true;  // marca que veio de navegação

            GD.Print($"[PlayerNavigationSystem] Iniciando movimento na direção {navigation.TargetNextDirection}");
        }
    }


    /// <summary>
    /// Cancela navegação atual
    /// </summary>
    private void CancelNavigation(ref NavigationComponent navigation, ref MovementComponent movement)
    {
        navigation.IsEnabled = false;
        navigation.PathFound = false;
        navigation.PathGridPositions?.Clear();
    }
}
