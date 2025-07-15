using Arch.Core;
using Arch.System;
using GameRpg2D.Scripts.ECS.Systems.Animation;
using GameRpg2D.Scripts.ECS.Systems.Combat;
using GameRpg2D.Scripts.ECS.Systems.Inputs;
using GameRpg2D.Scripts.ECS.Systems.Movement;
using GameRpg2D.Scripts.ECS.Systems.Physics;
using Godot;

namespace GameRpg2D.Scripts.ECS.Infrastructure;

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
            // Ordem de execução dos sistemas
            new InputSystem(World),   // 1. Processa input
            new FacingSystem(World),                // 2. Atualiza direção de movimento
            new CollisionSystem(World),             // 3. Valida colisões
            new MovementSystem(World),              // 4. Processa movimento
            new AttackSystem(World),                // 5. Processa ataque
            new TransformSystem(World),             // 6. Atualiza transformações
            new AnimationSystem(World)              // 7. Atualiza animações
        );

        _deltaGroup.Initialize();

        // LOG: Sistemas inicializados
        GD.Print("[EcsRunner] Sistemas ECS inicializados");
    }

    public void Update(double delta)
    {
        _deltaGroup.BeforeUpdate((float)delta);    // Calls .BeforeUpdate on all systems ( can be overriden )
        _deltaGroup.Update((float)delta);          // Calls .Update on all systems ( can be overriden )
        _deltaGroup.AfterUpdate((float)delta);     // Calls .AfterUpdate on all systems (can be overridden)
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