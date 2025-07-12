using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

[Component]
public struct LocalInputComponent
{
    // Movimento
    public Vector2I InputDirection;
    public bool MoveJustPressed;
    
    // Ataque (apenas para animação)
    public bool AttackPressed;
    public bool AttackJustPressed;
    
    // Propriedade de conveniência
    public bool HasInput => InputDirection != Vector2I.Zero || AttackPressed;
}

[Component]
public struct InputComponent
{
    public Vector2I Direction;
    public Vector2I LastDirection;
    public bool IsPressed;
    public bool WasPressed;
    public float HoldTime;
    public bool HasChanged;
    
    public InputComponent()
    {
        Direction = Vector2I.Zero;
        LastDirection = Vector2I.Zero;
        IsPressed = false;
        WasPressed = false;
        HoldTime = 0.0f;
        HasChanged = false;
    }
}

[Component]
public struct BufferedInputComponent
{
    public Vector2I BufferedDirection;
    public float BufferTime;
    public float MaxBufferTime;
    public bool HasBufferedInput;
    
    public BufferedInputComponent(float maxBuffer = 0.2f)
    {
        BufferedDirection = Vector2I.Zero;
        BufferTime = 0.0f;
        MaxBufferTime = maxBuffer;
        HasBufferedInput = false;
    }
}

[Component]
public struct ActionInputComponent
{
    public bool InteractPressed;
    public bool InteractJustPressed;
    public bool MenuPressed;
    public bool MenuJustPressed;
    
    public ActionInputComponent()
    {
        InteractPressed = false;
        InteractJustPressed = false;
        MenuPressed = false;
        MenuJustPressed = false;
    }
}
