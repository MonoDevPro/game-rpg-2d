namespace GameRpg2D.Scripts.ECS.Components.Combat;

public struct AttackStateComponent : IComponent
{
    /// <summary>Progresso do ataque em execução (0.0 a 1.0).</summary>
    public double Progress;
    
    /// <summary>
    /// Indica se o ataque está ativo.
    /// </summary>
    public bool IsActive;
    
    /// <summary>
    /// Tempo de início do ataque (timestamp).
    /// </summary>
    public double StartTime;
    
}