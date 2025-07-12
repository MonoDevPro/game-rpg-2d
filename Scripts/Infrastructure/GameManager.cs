using Godot;

namespace GameRpg2D.Scripts.Infrastructure;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    public EcsRunner EcsRunner { get; private set; }
    
    public override void _Ready()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
            
            // Inicializar ECS Runner
            EcsRunner = new EcsRunner();
                
            // Pré-carregar recursos importantes
            AssetService.Instance.PreloadCharacterSprites();
            
            GD.Print("GameManager inicializado com sucesso.");
        }
        else
        {
            QueueFree();
            GD.PrintErr("Tentativa de inicializar GameManager quando já existe uma instância ativa.");
        }
    }
    
    public override void _Process(double delta)
    {
        // Atualizar ECS Runner
        EcsRunner?.Update(delta);
    }
    
    public override void _ExitTree()
    {
        // Limpar recursos
        if (Instance == this)
        {
            Instance = null;
            EcsRunner?.Dispose();
        }
    }

}