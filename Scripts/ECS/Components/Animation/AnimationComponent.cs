using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Animation;

/// <summary>
/// Componente responsável pelo controle de animações das entidades
/// </summary>
public struct AnimationComponent : IComponent
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
    /// Referência ao sprite animado
    /// </summary>
    public AnimatedSprite2D Sprite;

    /// <summary>
    /// Nome da animação atual
    /// </summary>
    public string CurrentAnimation;

    /// <summary>
    /// Indica se a animação está tocando
    /// </summary>
    public bool IsPlaying;
}
