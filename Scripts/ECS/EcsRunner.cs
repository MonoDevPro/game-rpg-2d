using Arch.Core;
using Arch.System;
using GameRpg2D.Scripts.ECS.Events;
using Godot;

namespace GameRpg2D.Scripts.ECS;

public partial class EcsRunner : Node
{
    public World World { get; private set; }
    
    public EventHandlers EventHandlers { get; private set; }
    
    private Group<float> _deltaGroup;
    
    public override void _Ready()
    {
        this.Name = "ECS Runner";
        
        SetProcessMode(ProcessModeEnum.Always);
        
        // Inicializar mundo ECS
        World = World.Create();
        
        // Registrar eventos
        EventHandlers = new EventHandlers();

        _deltaGroup = new Group<float>(
            "ECS Systems",
            new LocalInputSystem(World),
            new BufferedInputSystem(World),
            new AISystem(World),
            new MovementSystem(World),
            new AttackSystem(World),
            new AnimationSystem(World),
            new RenderSystem(World)
        );
        
        _deltaGroup.Initialize();
    }

    public override void _Process(double delta)
    {
        _deltaGroup.BeforeUpdate((float)delta);    // Calls .BeforeUpdate on all systems ( can be overriden )
        _deltaGroup.Update((float)delta);          // Calls .Update on all systems ( can be overriden )
        _deltaGroup.AfterUpdate((float)delta);     // Calls .AfterUpdate on all System ( can be overriden )
    }
    
    public override void _ExitTree()
    {
        // Dispose of the ECS world when the node is removed from the scene tree
        _deltaGroup.Dispose();                     // Calls .Dispose on all systems ( can be overriden )
        EventHandlers.Dispose();                   // Dispose of event handlers
        World.Dispose();                           // Dispose of the ECS world
        _deltaGroup = null;                        // Clear the group reference
        EventHandlers = null;                     // Clear the event handlers reference
        World = null;                              // Clear the world reference
        GD.Print("ECS Runner exited and resources cleaned up.");
    }
}