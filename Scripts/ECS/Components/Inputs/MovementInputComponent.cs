using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Inputs;

public struct MovementInputComponent : IComponent
{
    public bool      IsMoving;              // Está segurando a tecla
    public bool      JustStarted;           // Foi pressionado neste frame
    public Vector2I RawMovement;             // Input bruto (caso precise de mais controle)
}