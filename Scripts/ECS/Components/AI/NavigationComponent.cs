using System.Collections.Generic;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.AI;

public struct DebugNavigationComponent(
    bool debugEnabled = true,
    bool debugUseCustom = true,
    Color debugPathColor = default)
    : IComponent
{
    public bool DebugEnabled = debugEnabled;
    public bool DebugUseCustom = debugUseCustom;
    public Color DebugPathColor = debugPathColor == default
        ? Colors.Red
        : debugPathColor;
}

public struct NavigationComponent : IComponent
{
    public NavigationAgent2D Agent;                         // Agente do Godot
    public Vector2I GridPosition;                           // Posição atual no grid
    public Vector2I TargetGridPosition;                     // Destino no grid
    public Direction TargetNextDirection;                   // Próxima direção do destino
    public Vector2I TargetNextGridPosition;                 // Próxima posição no grid do destino
    public List<Vector2I> PathGridPositions;                // Lista de nós do caminho
    public bool PathFound;                                  // Se encontrou rota
    public bool IsEnabled;                                  // Se o componente está habilitado
    public float TimeSinceLastRepath;                       // Tempo desde último recálculo

    /// <summary>
    /// Tolerância em unidades de grid para considerar que um nó foi alcançado.
    /// </summary>
    public int ReachGridTolerance;                          // Tolerância de alcance em grid
    public int RepathInterval;                              // Intervalo de recálculo do caminho em segundos

    public NavigationComponent(
        NavigationAgent2D navigationAgent,
        Vector2I gridPosition,
        Vector2I targetGridPosition,

        // Configs
        int reachGridTolerance = 1,
        int repathInterval = 1,
        bool isEnabled = false
        )
    {
        Agent = navigationAgent;
        GridPosition = gridPosition;
        TargetGridPosition = targetGridPosition;
        ReachGridTolerance = reachGridTolerance;
        RepathInterval = repathInterval;
        TargetNextDirection = Direction.None;
        TargetNextGridPosition = Vector2I.Zero;
        PathGridPositions = [];
        PathFound = false;
        IsEnabled = isEnabled;
        TimeSinceLastRepath = 0.0f;

        Agent.PathDesiredDistance = ReachGridTolerance;
    }
}