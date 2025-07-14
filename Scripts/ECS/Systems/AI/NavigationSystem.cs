using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Movement;
using Godot;
using GameRpg2D.Scripts.Core.Enums;

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
    [All<NavigationComponent, MovementComponent, NodeComponent>]
    private void CalculatePaths([Data] in float deltaTime, ref NavigationComponent nav)
    {
        // Atualiza o tempo desde o último recálculo
        nav.TimeSinceLastRepath += deltaTime;
        
        // Verifica se precisa calcular ou recalcular o caminho
        if (!nav.PathFound || nav.TimeSinceLastRepath >= nav.RepathInterval)
        {
            nav.TimeSinceLastRepath = 0;
            
            try
            {
                // Atualiza a posição atual do agente
                var currentWorldPosition = PositionHelper.GridToWorld(nav.GridPosition);
                GD.Print($"[NavigationSystem] Posição atual: {currentWorldPosition}, Grid: {nav.GridPosition}");
                
                // Configura o destino no agente de navegação
                var targetWorldPosition = PositionHelper.GridToWorld(nav.TargetGridPosition);
                nav.Agent.SetTargetPosition(targetWorldPosition);
                
                GD.Print($"[NavigationSystem] Destino: {targetWorldPosition}, Grid: {nav.TargetGridPosition}");
                
                // Verifica se o destino é alcançável
                if (!nav.Agent.IsTargetReachable())
                {
                    GD.Print($"[NavigationSystem] ERRO: Destino {nav.TargetGridPosition} não é alcançável");
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
                
                // Converte o caminho para grid
                nav.PathGridPositions = [];
                foreach (var point in pathWorld)
                {
                    var gridPoint = PositionHelper.WorldToGrid(point);
                    nav.PathGridPositions.Add(gridPoint);
                    GD.Print($"[NavigationSystem] Ponto do caminho: {point} -> Grid: {gridPoint}");
                }
                
                nav.PathFound = true;
                
                GD.Print($"[NavigationSystem] Caminho encontrado com {nav.PathGridPositions.Count} pontos");
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"[NavigationSystem] Erro ao calcular caminho: {ex.Message}");
                nav.PathFound = false;
                nav.TargetNextDirection = Direction.None;
                nav.TargetNextGridPosition = Vector2I.Zero;
            }
        }
    }
    
    /// <summary>
    /// Segue o caminho calculado
    /// </summary>
    [Query]
    [All<NavigationComponent, MovementComponent, NodeComponent>]
    private void FollowPath([Data] in float deltaTime, ref NavigationComponent navigation)
    {
        // Se não temos um caminho ou já estamos nos movendo, não faz nada
        if (!navigation.PathFound || navigation.PathGridPositions == null || navigation.PathGridPositions.Count == 0)
        {
            GD.Print($"Ignorando navegação. PathFound:{navigation.PathFound} PathGridCount:{navigation.PathGridPositions?.Count} pontos no caminho.");
            return;
        }
        
        // Obtém o próximo ponto do caminho
        var nextGridPosition = navigation.PathGridPositions[0];
        var currentGridPosition = navigation.GridPosition;
        GD.Print($"[NavigationSystem] Seguindo para próximo ponto: {nextGridPosition}, atual: {navigation.GridPosition}");
        
        // Verifica se já estamos no próximo ponto
        if (currentGridPosition.DistanceTo(nextGridPosition) <= navigation.ReachGridTolerance)
        {
            GD.Print($"[NavigationSystem] Já estamos no próximo ponto. Removendo-o do caminho.");
            navigation.PathGridPositions.RemoveAt(0);
            
            // Se acabaram os pontos, terminamos
            if (navigation.PathGridPositions.Count == 0)
            {
                GD.Print($"[NavigationSystem] Chegou ao destino final");
                return;
            }
            
            // Contínua para o próximo ponto
            nextGridPosition = navigation.PathGridPositions[0];
        }
        
        // Calcula a direção para o próximo ponto
        var direction = PositionHelper.VectorToDirection(nextGridPosition - currentGridPosition);
        
        if (direction == Direction.None)
        {
            GD.Print($"[NavigationSystem] ERRO: Não foi possível determinar direção para {nextGridPosition} a partir de {currentGridPosition}");
            return;
        }
        
        GD.Print($"[NavigationSystem] Iniciando movimento na direção: {direction} para {nextGridPosition}");
        
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