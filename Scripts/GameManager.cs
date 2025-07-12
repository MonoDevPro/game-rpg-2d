using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Entities;
using GameRpg2D.Scripts.Infrastructure;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS;
using Godot;
using MovementSystem = GameRpg2D.Scripts.ECS.MovementSystem;

namespace GameRpg2D.Scripts
{
    public partial class GameManager : Node
    {
        public static GameManager Instance { get; private set; }
        
        // ECS
        public EcsRunner Ecs { get; private set; }
        
        public override void _Ready()
        {
            // Configurar singleton
            if (Instance == null)
            {
                Instance = this;
                SetProcessMode(ProcessModeEnum.Always);
                
                // Inicializar ECS
                Ecs = new EcsRunner();
                AddChild(Ecs);
                
                // Pré-carregar recursos importantes
                AssetService.Instance.PreloadCharacterSprites();
                
                // Criar NPCs de teste para verificar sincronização
                SpawnTestNPCs();
                
                GD.Print("GameManager inicializado com sistemas ECS multi-jogador");
            }
            else
            {
                QueueFree();
                GD.PrintErr("Tentativa de inicializar GameManager quando já existe uma instância ativa.");
            }
        }

        public override void _ExitTree()
        {
            // Limpar recursos
            
            Ecs.World.Dispose();
            
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #region Entity Creation Helpers
        
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
            var gi = CreateVisualNPC("GuardIdle", new Vector2I(3, 3), NpcBehaviourType.Idle, Vocation.Mage, Gender.Male, 1);
            GD.Print("Created VISUAL Idle NPC at (3, 3)");
            
            // NPC 2: Wanderer Archer feminino
            CreateVisualNPC("Wanderer", new Vector2I(-2, 5), NpcBehaviourType.Wander, Vocation.Archer, Gender.Female, 2);
            GD.Print("Created VISUAL Wandering NPC at (-2, 5)");
            
            // NPC 3: Patrol Mage masculino
            CreateVisualNPC("Patrol", new Vector2I(8, -1), NpcBehaviourType.Patrol, Vocation.Mage, Gender.Male, 3);
            GD.Print("Created VISUAL Patrol NPC at (8, -1)");
            
            // NPC 4: Goblin Agressivo
            CreateVisualNPC("Goblin", new Vector2I(-5, -3), NpcBehaviourType.Aggressive, Vocation.Archer, Gender.Male, 4);
            GD.Print("Created VISUAL Aggressive NPC at (-5, -3)");
            
            // NPC 5: Scout Archer feminino
            CreateVisualNPC("Scout", new Vector2I(6, 7), NpcBehaviourType.Wander, Vocation.Archer, Gender.Female, 5);
            GD.Print("Created VISUAL Second Wandering NPC at (6, 7)");
            
            GD.Print($"Total entities in world: {Ecs.World.Size}");
        }
        
        /// <summary>
        /// Cria um NPC visual real no mundo com comportamento específico
        /// </summary>
        public Entity CreateVisualNPC(string npcType, Vector2I startPosition, NpcBehaviourType behaviourType, Vocation vocation = Vocation.Mage, Gender gender = Gender.Male, int npcId = 0)
        {
            // Carregar a cena do NPC
            var npcScene = GD.Load<PackedScene>("res://Scenes/npc.tscn");
            var npcInstance = npcScene.Instantiate<NPC>();
            
            // Configurar propriedades do NPC
            npcInstance.NPCType = npcType;
            npcInstance.NPCId = npcId;
            npcInstance.BehaviourType = behaviourType;
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
