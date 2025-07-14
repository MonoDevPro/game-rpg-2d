/*
using Arch.Core;
using GameRpg2D.Scripts.ECS.Bridges;
using GameRpg2D.Scripts.ECS.Components.Pure;
using GameRpg2D.Scripts.ECS.Entities;
using GameRpg2D.Scripts.ECS.Systems.Pure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Infrastructure;

/// <summary>
/// Manager para sistemas ECS puros com bridges
/// </summary>
public class PureEcsManager
{
    private readonly World _world;
    private readonly GodotRenderBridge _renderBridge;
    private readonly GodotPhysicsBridge _physicsBridge;
    private readonly GodotInputBridge _inputBridge;
    
    // Sistemas puros
    private readonly PureAnimationSystem _animationSystem;
    private readonly PureInputSystem _inputSystem;
    private readonly PurePhysicsSystem _physicsSystem;

    public PureEcsManager(World world)
    {
        _world = world;
        
        // Inicializa bridges
        _renderBridge = new GodotRenderBridge();
        _physicsBridge = new GodotPhysicsBridge();
        _inputBridge = new GodotInputBridge();
        
        // Inicializa sistemas puros
        _animationSystem = new PureAnimationSystem(_world, _renderBridge);
        _inputSystem = new PureInputSystem(_world, _inputBridge);
        _physicsSystem = new PurePhysicsSystem(_world, _physicsBridge);
    }

    /// <summary>
    /// Registra uma entidade no sistema puro
    /// </summary>
    public void RegisterEntity(BaseBody entity)
    {
        var entityId = (uint)entity.GetEntity().Id;
        
        // Registra sprite se disponível
        var sprite = entity.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (sprite != null)
        {
            _renderBridge.RegisterSprite(entityId, sprite);
            
            // Adiciona componente puro de animação
            _world.Add(entity.GetEntity(), new PureAnimationComponent
            {
                State = Core.Enums.AnimationState.Idle,
                Direction = Core.Enums.Direction.South,
                CurrentAnimation = "idle_south",
                IsPlaying = true,
                Speed = 1.0f,
                EntityId = entityId
            });
        }
        
        // Registra body para física
        _physicsBridge.RegisterBody(entityId, entity);
        
        // Adiciona componente puro de física
        _world.Add(entity.GetEntity(), new PurePhysicsComponent
        {
            Position = entity.GlobalPosition,
            Size = Vector2.One * Core.Constants.GameConstants.GRID_SIZE,
            CollisionMask = entity.CollisionMask,
            CollisionLayer = entity.CollisionLayer,
            IsColliding = false,
            BlockedDirections = Components.Physics.CollisionDirections.None,
            LastValidPosition = entity.GlobalPosition,
            DetectionRadius = Core.Constants.GameConstants.GRID_SIZE / 2.0f,
            EntityId = entityId
        });
        
        // Adiciona componente de render
        _world.Add(entity.GetEntity(), new PureRenderComponent
        {
            Position = entity.GlobalPosition,
            Scale = entity.Scale,
            Rotation = entity.Rotation,
            Visible = entity.Visible,
            ZIndex = entity.ZIndex,
            EntityId = entityId
        });
        
        // Se é player, adiciona input
        if (entity is Player)
        {
            _world.Add(entity.GetEntity(), new PureInputComponent
            {
                EntityId = entityId
            });
        }
    }

    /// <summary>
    /// Remove uma entidade do sistema puro
    /// </summary>
    public void UnregisterEntity(BaseBody entity)
    {
        var entityId = (uint)entity.GetEntity().Id;
        
        _renderBridge.UnregisterSprite(entityId);
        _physicsBridge.UnregisterBody(entityId);
        
        // Remove componentes puros
        var ent = entity.GetEntity();
        if (_world.Has<PureAnimationComponent>(ent))
            _world.Remove<PureAnimationComponent>(ent);
        if (_world.Has<PurePhysicsComponent>(ent))
            _world.Remove<PurePhysicsComponent>(ent);
        if (_world.Has<PureRenderComponent>(ent))
            _world.Remove<PureRenderComponent>(ent);
        if (_world.Has<PureInputComponent>(ent))
            _world.Remove<PureInputComponent>(ent);
    }

    /// <summary>
    /// Atualiza todos os sistemas puros
    /// </summary>
    public void Update(float deltaTime)
    {
        // Ordem de execução otimizada
        _inputSystem.Update(deltaTime);      // Input primeiro
        _physicsSystem.Update(deltaTime);    // Física para validar movimento
        _animationSystem.Update(deltaTime);  // Animação baseada no estado final
    }

    /// <summary>
    /// Obtém acesso aos bridges para debugging
    /// </summary>
    public (IRenderBridge render, IPhysicsBridge physics, IInputBridge input) GetBridges()
    {
        return (_renderBridge, _physicsBridge, _inputBridge);
    }
}

/// <summary>
/// Extensão do EcsRunner para incluir sistemas puros
/// </summary>
public partial class HybridEcsRunner : EcsRunner
{
    private PureEcsManager _pureManager;
    
    public override void Initialize()
    {
        base.Initialize();
        _pureManager = new PureEcsManager(World);
    }

    /// <summary>
    /// Registra entidade tanto no sistema híbrido quanto no puro
    /// </summary>
    public void RegisterHybridEntity(BaseBody entity)
    {
        // Primeiro registra no sistema tradicional (se necessário)
        // Depois registra no sistema puro
        _pureManager.RegisterEntity(entity);
    }

    public override void Update(double deltaTime)
    {
        var dt = (float)deltaTime;
        
        // Executa sistemas tradicionais primeiro
        base.Update(deltaTime);
        
        // Depois executa sistemas puros
        _pureManager.Update(dt);
    }

    public override void Dispose()
    {
        _pureManager = null;
        base.Dispose();
    }
}
*/
