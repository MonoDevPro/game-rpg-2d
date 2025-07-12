using Arch.Core;
using Arch.System;
using Godot;

namespace GameRpg2D.Scripts.Infrastructure;

public sealed class EcsRunner
{
    public World World { get; private set; }
    
    private Group<float> _deltaGroup;
    
    public EcsRunner()
    {
        //Inicializar mundo ECS
        World = World.Create();
        
        // LOG: Inicialização do ECS
        GD.Print("[EcsRunner] Mundo ECS criado");
        

        _deltaGroup = new Group<float>(
            "ECS Systems",
            // This group will be updated every frame
            //new SystemExample(World)
        );
        
        _deltaGroup.Initialize();
        
        // LOG: Sistemas inicializados
        GD.Print("[EcsRunner] Sistemas ECS inicializados");
    }
    
    public void Update(double delta)
    {
        _deltaGroup.BeforeUpdate((float)delta);    // Calls .BeforeUpdate on all systems ( can be overriden )
        _deltaGroup.Update((float)delta);          // Calls .Update on all systems ( can be overriden )
        _deltaGroup.AfterUpdate((float)delta);     // Calls .AfterUpdate on all System ( can be overriden )
    }
    
    public void Dispose()
    {
        // Dispose of the ECS world when the node is removed from the scene tree
        _deltaGroup.Dispose();                     // Calls .Dispose on all systems ( can be overriden )
        World.Dispose();                           // Dispose of the ECS world
        _deltaGroup = null;                        // Clear the group reference
        World = null;                              // Clear the world reference
        
        GD.Print("ECS Runner exited and resources cleaned up.");
    }
}