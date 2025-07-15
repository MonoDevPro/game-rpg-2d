namespace GameRpg2D.Scripts.ECS.Components.Inputs;

public struct AttackInputComponent : IComponent
{
    public bool   IsAttacking;    // Botão segurado
    public bool   JustAttacked;   // Pressionado neste frame
    public double LastAttackTime; // Timestamp para cooldown
}