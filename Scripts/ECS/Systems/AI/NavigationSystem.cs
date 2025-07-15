using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Movement;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.AI;

/// <summary>
/// Sistema responsável por calcular e seguir caminhos de navegação
/// </summary>
public partial class NavigationSystem(World world) : BaseSystem<World, float>(world)
{
    /// <summary>
    /// Calcula e recalcula caminhos de navegação
    /// </summary>
    [Query]
    [All<NavigationComponent>]
    private void CalculatePaths([Data] in float deltaTime, ref NavigationComponent nav)
    {
        if (!nav.IsEnabled)
            return;
        
        // Atualiza o tempo desde o último recálculo
        nav.TimeSinceLastRepath += deltaTime;

        // Verifica se precisa calcular ou recalcular o caminho
        if (!nav.PathFound || nav.TimeSinceLastRepath >= nav.RepathInterval)
        {
            nav.TimeSinceLastRepath = 0;

            try
            {
                var currentWorldPosition = PositionHelper.GridToWorld(nav.GridPosition);
                var targetWorldPosition = PositionHelper.GridToWorld(nav.TargetGridPosition);

                nav.Agent.SetTargetPosition(targetWorldPosition);

                GD.Print($"[NavigationSystem] Destino: {targetWorldPosition}, Grid: {nav.TargetGridPosition}");

                // Verifica se o destino é alcançável (sem aguardar frame)
                if (!nav.Agent.IsTargetReachable())
                {
                    GD.Print($"[NavigationSystem] AVISO: Destino {nav.TargetGridPosition} não é alcançável");
                    GD.Print($"[NavigationSystem] Isso pode indicar que não há NavigationRegion2D configurado");

                    // Se não há região de navegação, use movimento direto
                    nav.PathFound = false;
                    return;
                }

                // Obtém o caminho
                Vector2[] pathWorld = nav.Agent.GetCurrentNavigationPath();
                GD.Print($"[NavigationSystem] Caminho bruto obtido com {pathWorld.Length} pontos");

                if (pathWorld.Length == 0)
                {
                    GD.Print($"[NavigationSystem] ERRO: Caminho vazio retornado");
                    nav.PathFound = false;
                    return;
                }

                nav.PathGridPositions.Clear();
                foreach (var point in pathWorld)
                {
                    var gridPoint = PositionHelper.WorldToGrid(point);
                    nav.PathGridPositions.Add(gridPoint);
                }
                
                nav.PathFound = nav.PathGridPositions.Count > 0;

                GD.Print($"[NavigationSystem] Caminho encontrado com {nav.PathGridPositions.Count} pontos");
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"[NavigationSystem] Erro ao calcular caminho: {ex.Message}");
                nav.PathFound = false; // Desativa navegação
            }
        }
    }

    /// <summary>
    /// Segue o caminho calculado
    /// </summary>
    [Query]
    [All<NavigationComponent>]
    private void FollowPath([Data] in float deltaTime, ref NavigationComponent navigation)
    {
        if (!navigation.IsEnabled || !navigation.PathFound)
            return;

        // Obtém o próximo ponto do caminho
        var nextGridPosition = navigation.PathGridPositions[0];
        var currentGridPosition = navigation.GridPosition;
        GD.Print($"[NavigationSystem] Seguindo para próximo ponto: {nextGridPosition}, atual: {navigation.GridPosition}");

        if (currentGridPosition.DistanceTo(nextGridPosition) <= navigation.ReachGridTolerance)
        {
            navigation.PathGridPositions.RemoveAt(0);
            if (navigation.PathGridPositions.Count == 0)
            {
                // chegamos ao destino final
                navigation.PathFound = false;
                navigation.IsEnabled = false;
                return;
            }
            
            nextGridPosition = navigation.PathGridPositions[0];
        }

        var directionVector = nextGridPosition - currentGridPosition;

        var direction = PositionHelper.VectorToDirection(directionVector);

        if (direction == Direction.None)
        {
            GD.PrintErr($"[NavigationSystem] ERRO: Direção inválida calculada para {directionVector}. Verifique o grid e a posição atual.");
            return;
        }

        GD.Print($"[NavigationSystem] Próxima direção calculada: {direction} para {nextGridPosition}");

        navigation.TargetNextDirection = direction;
        navigation.TargetNextGridPosition = nextGridPosition;
    }

    /// <summary>
    /// Desenha caminhos de debug para entidades com componente de debug
    /// </summary>
    [Query]
    [All<NavigationComponent, DebugNavigationComponent>]
    private void DrawDebugPaths([Data] in float deltaTime, in NavigationComponent navigation, in DebugNavigationComponent debug, in NodeComponent node)
    {
        if (!debug.DebugEnabled || !navigation.PathFound || navigation.PathGridPositions == null || navigation.PathGridPositions.Count == 0)
            return;

        // A implementação visual do debug vai depender de como você quer visualizar os caminhos
        // Aqui vamos apenas registrar informações
        GD.Print($"[NavigationSystem] Debug: Entidade {node.Node.GetInstanceId()} tem caminho com {navigation.PathGridPositions.Count} pontos");

        // Aqui você pode implementar a visualização do caminho
        // Exemplo: desenhar linhas entre os pontos do caminho
    }

}