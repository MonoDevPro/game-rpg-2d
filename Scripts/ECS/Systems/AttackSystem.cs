using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Utilities;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    public class AttackSystem(World world)
    {
        private readonly QueryDescription _attackQuery = new QueryDescription()
            .WithAll<AttackComponent, InputComponent, MovementComponent, AnimationComponent>();

        public void Update(float deltaTime)
        {
            world.Query(in _attackQuery, (ref AttackComponent attack, ref InputComponent input, ref MovementComponent movement, ref AnimationComponent animation) =>
            {
                // Atualizar timers
                attack.TimeSinceLastAttack += deltaTime;
                attack.ComboTimer += deltaTime;
                
                // Reset combo se passou do tempo limite E não está pressionando ataque (APENAS UMA VEZ)
                if (attack.ComboTimer > attack.ComboWindow && !input.AttackPressed && attack.CanCombo)
                {
                    attack.ComboCount = 0;
                    attack.CanCombo = false;
                    attack.LockedAttackDirection = Vector2I.Zero;
                    GD.Print("Combo expired - direction unlocked");
                }
                
                // Reset direção travada quando solta o botão (APENAS UMA VEZ)
                if (!input.AttackPressed && attack.LockedAttackDirection != Vector2I.Zero)
                {
                    attack.LockedAttackDirection = Vector2I.Zero;
                    GD.Print("Attack button released - direction unlocked for next attack");
                }

                // Verificar se pode iniciar ataque
                bool shouldStartAttack = false;
                
                // Primeiro ataque: apenas com JustPressed
                if (input.AttackJustPressed && CanStartAttack(ref attack, ref movement))
                {
                    shouldStartAttack = true;
                }
                // Ataques contínuos: se está pressionado, não está atacando, passou cooldown e tem combo ativo
                else if (input.AttackPressed && !attack.IsAttacking && 
                         attack.TimeSinceLastAttack >= attack.AttackCooldown && 
                         attack.CanCombo)
                {
                    shouldStartAttack = true;
                }

                if (shouldStartAttack)
                {
                    StartAttack(ref attack, ref input, ref movement, ref animation);
                }

                // Atualizar ataque em progresso
                if (attack.IsAttacking)
                {
                    UpdateAttack(ref attack, ref movement, deltaTime);
                }
            });
        }

        private bool CanStartAttack(ref AttackComponent attack, ref MovementComponent movement)
        {
            // Pode atacar se:
            // 1. Não está atacando atualmente
            // 2. Passou o tempo de cooldown
            // 3. Movimento não interfere mais no ataque (removido essa restrição)
            return !attack.IsAttacking && 
                   attack.TimeSinceLastAttack >= attack.AttackCooldown;
        }

        private void StartAttack(ref AttackComponent attack, ref InputComponent input, ref MovementComponent movement, ref AnimationComponent animation)
        {
            Vector2I attackDirection;
            
            GD.Print("Starting attack...");
            
            // Se é um combo (não é o primeiro ataque), usar direção travada
            if (attack.CanCombo && attack.ComboCount > 0 && attack.LockedAttackDirection != Vector2I.Zero)
            {
                attackDirection = attack.LockedAttackDirection;
                GD.Print($"Using locked attack direction for combo: {attackDirection}");
            }
            else
            {
                // Primeiro ataque ou novo combo - determinar nova direção
                attackDirection = DetermineAttackDirection(input, movement, animation);
                attack.LockedAttackDirection = attackDirection; // Travar direção para combos futuros
                GD.Print($"New attack direction locked: {attackDirection}");
            }
            
            // Configurar ataque
            attack.IsAttacking = true;
            attack.AttackProgress = 0.0f;
            attack.AttackDirection = attackDirection;
            attack.TimeSinceLastAttack = 0.0f;
            
            // Sistema de combo melhorado
            if (attack.CanCombo && attack.ComboCount < GameConstants.MAX_COMBO_COUNT)
            {
                attack.ComboCount++;
                attack.ComboTimer = 0.0f;
            }
            else
            {
                attack.ComboCount = 1;
                attack.CanCombo = false;
            }
            
            GD.Print($"Attack started! Direction: {attackDirection}, Combo: {attack.ComboCount}, Locked Direction: {attack.LockedAttackDirection}");
        }

        private Vector2I DetermineAttackDirection(InputComponent input, MovementComponent movement, AnimationComponent animation)
        {
            // Usar o DirectionUtils para centralizar a lógica de determinação de direção
            return DirectionUtils.DetermineDirection(
                input.InputDirection, 
                movement.IsMoving ? movement.Direction : Vector2I.Zero, 
                animation.LastDirection
            );
        }

        private void UpdateAttack(ref AttackComponent attack, ref MovementComponent movement, float deltaTime)
        {
            attack.AttackProgress += deltaTime / attack.AttackDuration;
            
            if (attack.AttackProgress >= 1.0f)
            {
                // Ataque completo
                attack.IsAttacking = false;
                attack.AttackProgress = 0.0f;
                
                // Habilitar combo APENAS se foi um ataque bem-sucedido
                if (attack.ComboCount >= 1)
                {
                    attack.CanCombo = true;
                    attack.ComboTimer = 0.0f;
                }
                
                GD.Print($"Attack finished! Combo count: {attack.ComboCount}, Can combo: {attack.CanCombo}");
            }
        }

        /// <summary>
        /// Método para detectar entidades no alcance do ataque
        /// </summary>
        public void CheckAttackHits(ref AttackComponent attack, ref PositionComponent position)
        {
            if (!attack.IsAttacking) return;
            
            // Calcular posição do ataque baseada na direção
            Vector2I attackPosition = position.GridPosition + attack.AttackDirection;
            
            // Aqui você pode implementar lógica para detectar inimigos na posição
            // Por exemplo, fazer query por entidades inimigas na posição calculada
            
            // Para debug, vamos apenas mostrar onde o ataque está acontecendo
            GD.Print($"Attack hitting position: {attackPosition}");
        }
    }
}
