using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Input;

/// <summary>
/// Componente responsável pelo input das entidades controláveis
/// </summary>
public struct InputComponent : IComponent
{
    /// <summary>
    /// Direção atual do movimento baseada no input
    /// </summary>
    public Direction MovementDirection;

    /// <summary>
    /// Indica se o comando de movimento está ativo
    /// </summary>
    public bool IsMovementPressed;

    /// <summary>
    /// Indica se o comando de ataque está ativo
    /// </summary>
    public bool IsAttackPressed;

    /// <summary>
    /// Indica se o comando de ataque foi pressionado neste frame
    /// </summary>
    public bool IsAttackJustPressed;

    /// <summary>
    /// Vetor de input bruto (para casos especiais)
    /// </summary>
    public Vector2 RawInput;

    /// <summary>
    /// Timestamp do último input de movimento
    /// </summary>
    public double LastMovementTime;

    /// <summary>
    /// Timestamp do último input de ataque
    /// </summary>
    public double LastAttackTime;
}
