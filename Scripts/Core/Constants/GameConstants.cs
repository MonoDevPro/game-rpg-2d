namespace GameRpg2D.Scripts.Core.Constants;

/// <summary>
/// Constantes relacionadas ao movimento e grid do jogo
/// </summary>
public static class GameConstants
{
    // Grid
    public const int GRID_SIZE = 32; // Tamanho de cada célula do grid em pixels
    
    // Sistema de Movimento
    public const float DEFAULT_MOVEMENT_SPEED = 100f; // Velocidade de movimento padrão em pixels por segundo
    
    // Sistema de Ataque
    public const float DEFAULT_ATTACK_SPEED = 0.4f; // Velocidade de ataque padrão em segundos
    public const float DEFAULT_ATTACK_COOLDOWN = 1f; // Tempo de recarga do ataque em segundos
}