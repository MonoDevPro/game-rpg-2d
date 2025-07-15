using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Inputs;

public struct PointerInputComponent : IComponent
{
    public bool    JustClicked;             // Clique detectado neste frame
    public Vector2 ClickWorldPosition;      // Posição do clique em mundo
    public Vector2 PreviousWorldPosition;   // Posição anterior do ponteiro em mundo
    public Vector2 RawDelta;                // Caso queira arrastar, scroll etc.
}