/*
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Physics;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Pure;

/// <summary>
/// Componente de animação puro - sem dependências de Godot
/// </summary>
public struct PureAnimationComponent : IComponent
{
    /// <summary>
    /// Estado atual da animação
    /// </summary>
    public AnimationState State;

    /// <summary>
    /// Direção atual da animação
    /// </summary>
    public Direction Direction;

    /// <summary>
    /// Nome da animação atual
    /// </summary>
    public string CurrentAnimation;

    /// <summary>
    /// Indica se a animação está tocando
    /// </summary>
    public bool IsPlaying;

    /// <summary>
    /// Velocidade da animação
    /// </summary>
    public float Speed;

    /// <summary>
    /// Timestamp da última mudança
    /// </summary>
    public double LastChangeTime;

    /// <summary>
    /// Duração customizada da animação (se aplicável)
    /// </summary>
    public float? CustomDuration;

    /// <summary>
    /// ID da entidade (para bridge lookup)
    /// </summary>
    public uint EntityId;
}

/// <summary>
/// Componente de renderização puro
/// </summary>
public struct PureRenderComponent : IComponent
{
    /// <summary>
    /// Posição no mundo
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Escala do sprite
    /// </summary>
    public Vector2 Scale;

    /// <summary>
    /// Rotação em radianos
    /// </summary>
    public float Rotation;

    /// <summary>
    /// Visibilidade
    /// </summary>
    public bool Visible;

    /// <summary>
    /// Ordem de renderização (z-index)
    /// </summary>
    public int ZIndex;

    /// <summary>
    /// ID da entidade
    /// </summary>
    public uint EntityId;
}

/// <summary>
/// Componente de física puro
/// </summary>
public struct PurePhysicsComponent : IComponent
{
    /// <summary>
    /// Posição atual
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Tamanho da hitbox
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// Máscara de colisão
    /// </summary>
    public uint CollisionMask;

    /// <summary>
    /// Layer de colisão
    /// </summary>
    public uint CollisionLayer;

    /// <summary>
    /// Estado de colisão atual
    /// </summary>
    public bool IsColliding;

    /// <summary>
    /// Direções bloqueadas
    /// </summary>
    public CollisionDirections BlockedDirections;

    /// <summary>
    /// Última posição válida
    /// </summary>
    public Vector2 LastValidPosition;

    /// <summary>
    /// Raio de detecção
    /// </summary>
    public float DetectionRadius;

    /// <summary>
    /// ID da entidade
    /// </summary>
    public uint EntityId;
}

/// <summary>
/// Componente de input puro
/// </summary>
public struct PureInputComponent : IComponent
{
    /// <summary>
    /// Vetor de movimento (normalizado)
    /// </summary>
    public Vector2 InputVector;

    /// <summary>
    /// Botão de ataque pressionado
    /// </summary>
    public bool AttackPressed;

    /// <summary>
    /// Botão de interação pressionado
    /// </summary>
    public bool InteractPressed;

    /// <summary>
    /// Botão de pausa pressionado
    /// </summary>
    public bool PausePressed;

    /// <summary>
    /// Input de direção específica (para animação)
    /// </summary>
    public Direction DirectionInput;

    /// <summary>
    /// Timestamp do último input
    /// </summary>
    public double LastInputTime;

    /// <summary>
    /// ID da entidade
    /// </summary>
    public uint EntityId;
}
*/
