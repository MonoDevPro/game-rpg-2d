using Arch.Core;
using GameRpg2D.Scripts.ECS.Systems;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Entities;
using GameRpg2D.Scripts.Infrastructure;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts
{
    public partial class GameManager : Node
    {
        public static GameManager Instance { get; private set; }
        
        public World World { get; private set; }
        
        private LocalInputSystem _localInputSystem;
        private BufferedInputSystem _bufferedInputSystem;
        private AISystem _aiSystem;
        private MovementSystem _movementSystem;
        private AttackSystem _attackSystem;
        private AnimationSystem _animationSystem;
        private RenderSystem _renderSystem;
        
        private CanvasLayer _debugLayer;

        public override void _Ready()
        {
            // Configurar singleton
            if (Instance == null)
            {
                Instance = this;
                SetProcessMode(ProcessModeEnum.Always);
                
                // Inicializar mundo ECS
                World = World.Create();
                
                // Inicializar sistemas na ordem correta do pipeline
                _localInputSystem = new LocalInputSystem(World);
                _bufferedInputSystem = new BufferedInputSystem(World);
                _aiSystem = new AISystem(World);
                _movementSystem = new MovementSystem(World);
                _attackSystem = new AttackSystem(World);
                _animationSystem = new AnimationSystem(World);
                _renderSystem = new RenderSystem(World);
                
                // Pré-carregar recursos importantes
                AssetService.Instance.PreloadCharacterSprites();
                
                // Criar NPCs de teste para verificar sincronização
                SpawnTestNPCs();
                
                GD.Print("GameManager inicializado com sistemas ECS multi-jogador");
            }
            else
            {
                QueueFree();
            }
        }

        public override void _Process(double delta)
        {
            if (World == null) return;
            
            // Pipeline de sistemas na ordem correta:
            // 1. Coleta de inputs de diferentes fontes
            _localInputSystem.Update();                    // Input do dispositivo local
            _bufferedInputSystem.Update();                 // Inputs buffered de remotos
            _aiSystem.Update((float)delta);                // IA para NPCs
            
            // 2. Processamento de gameplay
            _movementSystem.Update((float)delta);          // Movimento baseado em inputs
            _attackSystem.Update((float)delta);            // Sistema de ataques
            
            // 3. Apresentação
            _animationSystem.Update();                     // Animações baseadas em estado
            _renderSystem.Update();                        // Renderização final
        }

        public override void _ExitTree()
        {
            // Limpar recursos
            World?.Dispose();
            
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #region Entity Creation Helpers
        
        /// <summary>
        /// Cria um jogador remoto conectado via rede
        /// </summary>
        public Entity CreateRemotePlayer(int playerId, Vector2I startPosition, Vocation vocation, Gender gender)
        {
            // TODO: Criar node visual para jogador remoto (por enquanto sem node)
            return World.Create(
                new PositionComponent(startPosition),
                new MovementComponent(GameConstants.DEFAULT_MOVE_SPEED, GameConstants.INPUT_STOP_DELAY),
                new InputComponent(),
                new BufferedInputComponent(maxBufferSize: 60), // Buffer maior para rede
                new AttackComponent(GameConstants.DEFAULT_ATTACK_DURATION, GameConstants.DEFAULT_ATTACK_RANGE, GameConstants.DEFAULT_ATTACK_COOLDOWN),
                new RemotePlayerTag(playerId)
                // AnimationComponent e NodeComponent serão adicionados quando criar visual
            );
        }

        /// <summary>
        /// Cria um NPC com comportamento específico
        /// </summary>
        public Entity CreateNPC(string npcType, Vector2I startPosition, NPCBehaviorType behaviorType, int npcId = 0)
        {
            return World.Create(
                new PositionComponent(startPosition),
                new MovementComponent(GameConstants.DEFAULT_MOVE_SPEED * 0.8f, GameConstants.INPUT_STOP_DELAY), // NPCs um pouco mais lentos
                new InputComponent(),
                new BehaviorComponent(behaviorType),
                new AttackComponent(GameConstants.DEFAULT_ATTACK_DURATION * 1.2f, GameConstants.DEFAULT_ATTACK_RANGE, GameConstants.DEFAULT_ATTACK_COOLDOWN * 1.5f), // NPCs atacam mais devagar
                new NPCTag(npcType, npcId)
                // TODO: Adicionar visual quando necessário
            );
        }

        /// <summary>
        /// Adiciona input remoto ao buffer de um jogador específico
        /// </summary>
        public void AddRemoteInput(int playerId, Vector2I direction, bool attack, double timestamp)
        {
            var remotePlayerQuery = new QueryDescription()
                .WithAll<BufferedInputComponent, RemotePlayerTag>();

            World.Query(in remotePlayerQuery, (ref BufferedInputComponent buffer, ref RemotePlayerTag tag) =>
            {
                if (tag.PlayerId == playerId)
                {
                    buffer.EnqueueInput(direction, attack, timestamp);
                    GD.Print($"Added remote input for player {playerId}: {direction}, attack: {attack}");
                }
            });
        }

        /// <summary>
        /// Cria NPCs de teste para verificar sincronização do sistema multi-jogador
        /// </summary>
        private void SpawnTestNPCs()
        {
            GD.Print("Spawning VISUAL test NPCs...");
            
            // Aguardar um frame para garantir que a cena está carregada
            CallDeferred(nameof(SpawnVisualNPCs));
        }
        
        private void SpawnVisualNPCs()
        {
            // NPC 1: Guarda Idle próximo ao spawn do player
            CreateVisualNPC("GuardIdle", new Vector2I(3, 3), NPCBehaviorType.Idle, Vocation.Mage, Gender.Male, 1);
            GD.Print("Created VISUAL Idle NPC at (3, 3)");
            
            // NPC 2: Wanderer Archer feminino
            CreateVisualNPC("Wanderer", new Vector2I(-2, 5), NPCBehaviorType.Wander, Vocation.Archer, Gender.Female, 2);
            GD.Print("Created VISUAL Wandering NPC at (-2, 5)");
            
            // NPC 3: Patrol Mage masculino
            CreateVisualNPC("Patrol", new Vector2I(8, -1), NPCBehaviorType.Patrol, Vocation.Mage, Gender.Male, 3);
            GD.Print("Created VISUAL Patrol NPC at (8, -1)");
            
            // NPC 4: Goblin Agressivo
            CreateVisualNPC("Goblin", new Vector2I(-5, -3), NPCBehaviorType.Aggressive, Vocation.Archer, Gender.Male, 4);
            GD.Print("Created VISUAL Aggressive NPC at (-5, -3)");
            
            // NPC 5: Scout Archer feminino
            CreateVisualNPC("Scout", new Vector2I(6, 7), NPCBehaviorType.Wander, Vocation.Archer, Gender.Female, 5);
            GD.Print("Created VISUAL Second Wandering NPC at (6, 7)");
            
            // Criar um jogador remoto de teste (simulando conexão de rede) - ainda sem visual
            var remotePlayer = CreateRemotePlayer(100, new Vector2I(10, 10), Vocation.Archer, Gender.Female);
            GD.Print("Created Remote Player (ID: 100) at (10, 10)");
            
            // Simular alguns inputs para o jogador remoto
            CallDeferred(nameof(SimulateRemotePlayerInputs));
            
            GD.Print($"Total entities in world: {World.Size}");
        }
        
        /// <summary>
        /// Simula inputs de um jogador remoto para testar o sistema buffered
        /// </summary>
        private void SimulateRemotePlayerInputs()
        {
            // Esperar um pouco e depois enviar alguns comandos
            var timer = GetTree().CreateTimer(2.0f);
            timer.Timeout += () =>
            {
                // Simular movimento para direita
                AddRemoteInput(100, GameConstants.Directions.RIGHT, false, Time.GetUnixTimeFromSystem());
                
                // Simular ataque após meio segundo
                var attackTimer = GetTree().CreateTimer(0.5f);
                attackTimer.Timeout += () =>
                {
                    AddRemoteInput(100, Vector2I.Zero, true, Time.GetUnixTimeFromSystem());
                };
                
                // Simular movimento para baixo
                var moveTimer = GetTree().CreateTimer(1.0f);
                moveTimer.Timeout += () =>
                {
                    AddRemoteInput(100, GameConstants.Directions.DOWN, false, Time.GetUnixTimeFromSystem());
                };
            };
        }
        
        /// <summary>
        /// Cria um NPC visual real no mundo com comportamento específico
        /// </summary>
        public Entity CreateVisualNPC(string npcType, Vector2I startPosition, NPCBehaviorType behaviorType, Vocation vocation = Vocation.Mage, Gender gender = Gender.Male, int npcId = 0)
        {
            // Carregar a cena do NPC
            var npcScene = GD.Load<PackedScene>("res://Scenes/npc.tscn");
            var npcInstance = npcScene.Instantiate<NPC>();
            
            // Configurar propriedades do NPC
            npcInstance.NPCType = npcType;
            npcInstance.NPCId = npcId;
            npcInstance.BehaviorType = behaviorType;
            npcInstance.NPCVocation = vocation;
            npcInstance.NPCGender = gender;
            npcInstance.StartGridPosition = startPosition;
            
            // Adicionar ao mundo (assumindo que existe um nó Main na cena)
            var mainNode = GetTree().CurrentScene;
            mainNode.AddChild(npcInstance);
            
            // O NPC criará sua própria entidade ECS no _Ready()
            return npcInstance.GetEntity();
        }
        #endregion
    }
}
