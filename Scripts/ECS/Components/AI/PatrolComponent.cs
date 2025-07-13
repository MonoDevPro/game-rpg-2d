using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.AI;

/// <summary>
/// Componente responsável pelos dados de patrulha dos NPCs
/// </summary>
public struct PatrolComponent : IComponent
{
    /// <summary>
    /// Waypoints da patrulha (posições no grid)
    /// </summary>
    public Vector2I[] WayPoints;

    /// <summary>
    /// Índice do waypoint atual
    /// </summary>
    public int CurrentWayPointIndex;

    /// <summary>
    /// Estado atual da patrulha
    /// </summary>
    public PatrolState State;

    /// <summary>
    /// Direção da patrulha atual
    /// </summary>
    public Direction PatrolDirection;

    /// <summary>
    /// Timer de espera atual
    /// </summary>
    public float WaitTimer;

    /// <summary>
    /// Duração da espera em cada waypoint
    /// </summary>
    public float WaitDuration;

    /// <summary>
    /// Velocidade da patrulha
    /// </summary>
    public float PatrolSpeed;

    /// <summary>
    /// Se a patrulha é em loop infinito
    /// </summary>
    public bool IsLooping;

    /// <summary>
    /// Se deve reverter direção no final (ping-pong)
    /// </summary>
    public bool ReverseOnEnd;

    /// <summary>
    /// Indica se está indo em direção reversa (ping-pong)
    /// </summary>
    public bool IsReversing;

    /// <summary>
    /// Waypoint inicial (para retorno)
    /// </summary>
    public Vector2I InitialWayPoint;

    /// <summary>
    /// Distância mínima para considerar waypoint alcançado
    /// </summary>
    public float WayPointTolerance;
}

/// <summary>
/// Estados possíveis da patrulha
/// </summary>
public enum PatrolState : byte
{
    /// <summary>
    /// Movendo para próximo waypoint
    /// </summary>
    Moving,

    /// <summary>
    /// Esperando em waypoint
    /// </summary>
    Waiting,

    /// <summary>
    /// Retornando ao ponto inicial
    /// </summary>
    Returning,

    /// <summary>
    /// Patrulha completada
    /// </summary>
    Completed,

    /// <summary>
    /// Patrulha pausada/interrompida
    /// </summary>
    Paused
}
