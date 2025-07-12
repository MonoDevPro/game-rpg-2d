using Arch.Bus;
using Godot;

namespace GameRpg2D.Scripts.ECS.Events;


public partial class EventHandlers
{
    
    public EventHandlers()
    {
        // Aqui você pode registrar outros manipuladores de eventos, se necessário
        Hook();
    }
    
    public void Dispose()
    {
        // Aqui você pode desconectar os manipuladores de eventos, se necessário
        Unhook();
    }
    
    [Event(0x0)]
    public void HandlePlayerMoveEvent(ref MovementCompletedEvent playerMoveEvent)
    {
        // Aqui você pode implementar a lógica para lidar com o movimento do jogador
        // Por exemplo, atualizar a posição do jogador no mundo ECS
        GD.Print($"Order: {0} Player {playerMoveEvent.Entity.Id} moved in direction {playerMoveEvent.NewGridPosition}");
    }
    
    [Event(0x1)]
    public void HandlePlayerMoveEvent2(ref MovementCompletedEvent playerMoveEvent)
    {
        // Aqui você pode implementar a lógica para lidar com o movimento do jogador
        // Por exemplo, atualizar a posição do jogador no mundo ECS
        GD.Print($"Order: {1}Player {playerMoveEvent.Entity.Id} moved in direction {playerMoveEvent.NewGridPosition}");
    }
}