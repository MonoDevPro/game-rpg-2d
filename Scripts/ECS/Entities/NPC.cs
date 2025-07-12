using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities
{
    public partial class NPC : CharacterBody2D
    {
        // ECS
        private Entity _entity;
        private World _world;
        
        // Resources
        private AnimatedSprite2D _sprite;
        
        #region Parameters
        [Export] public string NPCType = "Guard";
        [Export] public int NPCId;
        [Export] public NpcBehaviourType BehaviourType = NpcBehaviourType.Idle;
        [Export] public Vocation NPCVocation = Vocation.Mage;
        [Export] public Gender NPCGender = Gender.Male;
        [Export] public float MoveSpeed = GameConstants.DEFAULT_MOVE_SPEED * 0.8f;
        [Export] public Vector2I StartGridPosition = Vector2I.Zero;
        #endregion
        
        #region Validation
        private bool ValidateResources()
        {
            if (_world == null)
            {
                GD.PrintErr($"NPC {NPCType}: O mundo ECS não está ativo.");
                return false;
            }
            
            if (_sprite == null)
            {
                GD.PrintErr($"NPC {NPCType}: AnimatedSprite2D não está definido.");
                return false;
            }
            
            return true;
        }
        #endregion

        public override void _Ready()
        {
            _world = GameManager.Instance.Ecs.World;
            _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            
            // Configurar sprite
            _sprite.SpriteFrames = AssetService.Instance.GetSpriteFrames(NPCVocation, NPCGender);
            _sprite.Play("idle_down"); // Iniciar com animação idle para baixo
            
            if (!ValidateResources())
            {
                GD.PrintErr($"NPC {NPCType}: Recursos inválidos.");
                return;
            }
            
            // Criar entidade ECS para o NPC
            _entity = _world.Create(
                new NodeComponent(this),
                new PositionComponent(StartGridPosition),
                new AttackComponent(),
                new AttackConfigComponent(GameConstants.DEFAULT_ATTACK_DURATION, GameConstants.DEFAULT_ATTACK_COOLDOWN),
                new MovementComponent(),
                new MovementConfigComponent(MoveSpeed),
                new BehaviourComponent(BehaviourType),
                new AIStateComponent(),
                new AnimationComponent(),
                new AnimatedSpriteComponent(_sprite),
                new NpcTag(NPCType, NPCId)
            );

            // Posicionar o nó na posição inicial do grid
            Position = StartGridPosition * GameConstants.GRID_SIZE;
            
            if (!_world.IsAlive(_entity))
            {
                GD.PrintErr($"NPC {NPCType}: Entidade não foi criada corretamente.");
                return;
            }
            
            GD.Print($"NPC {NPCType} (ID: {NPCId}) spawned at {StartGridPosition}");
        }
        
        public override void _ExitTree()
        {
            if (_world != null && _world.IsAlive(_entity))
            {
                _world.Destroy(_entity);
            }
        }
        
        public Entity GetEntity()
        {
            return _entity;
        }
    }
}
