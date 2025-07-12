# Sistema ECS - Godot 4.5 + Arch ECS + C#

## Contexto do Projeto

**Jogo 2D RPG Multiplayer** desenvolvido em Godot 4.5 com C#, utilizando Arch ECS para arquitetura orientada a componentes. O foco é **mínimo boilerplate**, **máxima reutilização** e **facilidade de manutenção**.

### Dependências Configuradas
```xml
<PackageReference Include="Arch" Version="2.1.0" />
<PackageReference Include="Arch.LowLevel" Version="1.1.5" />
<PackageReference Include="Arch.System" Version="1.1.0" />
<PackageReference Include="Arch.System.SourceGenerator" Version="2.1.0" />
<PackageReference Include="Arch.EventBus" Version="1.0.2" />
```

---

## Arquitetura Obrigatória

### 1. Source Generators
- **Sistemas**: Herdam de `BaseSystem<World, float>` com queries via `[Query(All<T, Y>, Any<T, Y>, None<T, Y>)]`
- **Componentes**: Implementam `IComponent` (struct readonly quando possível)
- **Eventos**: Handlers com `[Event(order)]` para controle de execução

### 2. Estrutura Base Existente
```csharp
// Já implementados
public interface IComponent { }
public readonly struct NodeComponent(Node2D node) : IComponent { /* ... */ }
public abstract partial class BaseBody : CharacterBody2D { /* ... */ }
public sealed class EcsRunner { /* ... */ }
public partial class GameManager : Node { /* ... */ }
```

### 3. Enums e Constantes (Já Definidos)
- `Direction` - 8 direções possíveis
- `AnimationState` - Estados de animação
- `NpcBehaviourType` - Tipos de comportamento
- `Vocation`, `Gender` - Características da entidade
- `GameConstants.GRID_SIZE` - Tamanho do grid

---

## Regras de Gameplay

### Movimentação
1. **Grid-based**: Todas as entidades se movem em grid (`GameConstants.GRID_SIZE`)
2. **Input**: Mapeamento `move_` + direção cardinal (north, south, east, west)
3. **Diagonal**: Combinação de duas direções cardinais, animação da direção mais próxima
4. **Fluidez**: Movimento contínuo enquanto tecla pressionada
5. **Velocidade**: Controlada por `MovementSpeed` da entidade

### Sistema de Ataque
1. **Input**: Comando genérico `attack`
2. **Timing**: `AttackSpeed` define duração, `AttackCooldown` define intervalo
3. **Animação**: Baseada no estado atual + direção da entidade
4. **Fluidez**: Ataques contínuos respeitando cooldown

### Animações
1. **Padrão**: `AnimationState.ToString().ToLower() + '_' + Direction.ToString().ToLower()`
2. **Exemplos**: `idle_down`, `walk_south`, `attack_north`
3. **Fallback**: Direção mais próxima para diagonais

---

## Objetivos de Implementação

### 1. Sistema de Componentes
**Criar componentes reutilizáveis:**
```csharp
// Exemplo de estrutura esperada
public readonly struct MovementComponent : IComponent
{
    public readonly float Speed;
    public readonly Direction CurrentDirection;
    public readonly Vector2I GridPosition;
    public readonly bool IsMoving;
}

public readonly struct AttackComponent : IComponent
{
    public readonly float AttackSpeed;
    public readonly float AttackCooldown;
    public readonly float LastAttackTime;
    public readonly bool IsAttacking;
}

public readonly struct AnimationComponent : IComponent
{
    public readonly AnimationState State;
    public readonly Direction Direction;
    public readonly AnimatedSprite2D Sprite;
}
```

### 2. Sistema de Tags
**Componentes de identificação para multiplayer:**
```csharp
public readonly struct LocalPlayerTag : IComponent { }
public readonly struct RemotePlayerTag : IComponent { }
public readonly struct NpcTag : IComponent { }
```

### 3. Sistemas com Source Generators
**Implementar sistemas especializados:**
```csharp
[Query(All<MovementComponent, NodeComponent>, None<LocalPlayerTag>)]
public partial class MovementSystem : BaseSystem<World, float>
{
    private void Execute(ref MovementComponent movement, ref NodeComponent node)
    {
        // Lógica de movimento
    }
}

[Query(All<AttackComponent, AnimationComponent>)]
public partial class AttackSystem : BaseSystem<World, float>
{
    private void Execute([Data] in float deltaTime, ref AttackComponent attack, ref AnimationComponent animation)
    {
        // Lógica de ataque
    }
}
```

### 4. EventBus Estruturado
**Eventos tipados para comunicação:**
```csharp
public readonly record struct MovementStartedEvent(Entity Entity, Vector2I FromGrid, Vector2I ToGrid, Direction Direction);
public readonly record struct AttackExecutedEvent(Entity Entity, Direction Direction, float Damage);
public readonly record struct AnimationChangedEvent(Entity Entity, AnimationState OldState, AnimationState NewState);

public partial class GameEventHandler
{
    [Event(0)] // Ordem de execução
    public void OnMovementStarted(ref MovementStartedEvent evt) { /* ... */ }
    
    [Event(1)]
    public void OnAttackExecuted(ref AttackExecutedEvent evt) { /* ... */ }
}
```

---

## Diretrizes de Implementação

### Reutilização de Componentes
1. **Componentes atômicos**: Cada componente tem uma responsabilidade única
2. **Composição**: Entidades diferentes combinam componentes conforme necessário
3. **Tags**: Distinguem comportamentos específicos sem duplicar lógica
4. **Polimorfismo via sistemas**: Um sistema pode processar diferentes tipos de entidade

### Padrões de Sistema
1. **Input Systems**: Processam apenas entidades com `LocalPlayerTag`
2. **Movement Systems**: Processam todas as entidades com `MovementComponent`
3. **Animation Systems**: Sincronizam estado visual com lógica
4. **Network Systems**: Sincronizam entidades `RemotePlayerTag`

### EventBus Guidelines
1. **Ordem de execução**: Use `[Event(order)]` para controlar sequência
2. **Eventos imutáveis**: Readonly records para thread-safety
3. **Escopo específico**: Eventos focados em uma ação/mudança específica
4. **Performance**: Evite eventos muito frequentes (ex: por frame)

---

## Estrutura de Arquivos Sugerida

```
Scripts/
├── Core/
│   ├── Constants/
│   ├── Enums/
│   └── Extensions/
├── ECS/
│   ├── Components/
│   │   ├── Core/         # Componentes básicos
│   │   ├── Movement/     # Componentes de movimento
│   │   ├── Combat/       # Componentes de combate
│   │   ├── Animation/    # Componentes de animação
│   │   └── Tags/         # Componentes de identificação
│   ├── Systems/
│   │   ├── Input/        # Sistemas de input
│   │   ├── Movement/     # Sistemas de movimento
│   │   ├── Combat/       # Sistemas de combate
│   │   ├── Animation/    # Sistemas de animação
│   │   └── Network/      # Sistemas de rede
│   ├── Events/
│   │   ├── Movement/     # Eventos de movimento
│   │   ├── Combat/       # Eventos de combate
│   │   └── Animation/    # Eventos de animação
│   └── Entities/
│       ├── BaseBody.cs   # Classe base existente
│       ├── Player.cs     # Implementação do jogador
│       └── Npc.cs        # Implementação do NPC
├── Infrastructure/
│   ├── EcsRunner.cs      # Runner existente
│   ├── GameManager.cs    # Manager existente
│   └── AssetService.cs   # Serviço de assets
└── Network/
    ├── Commands/         # Comandos de rede
    └── Synchronization/  # Sincronização de estado
```

---

## Critérios de Sucesso

### Performance
- [ ] Sistemas processam apenas entidades relevantes (queries otimizadas)
- [ ] Componentes são structs readonly quando possível
- [ ] EventBus não impacta performance em runtime

### Manutenibilidade
- [ ] Componentes reutilizáveis entre diferentes tipos de entidade
- [ ] Sistemas especializados e focados
- [ ] Código gerado automaticamente pelos source generators

### Escalabilidade
- [ ] Fácil adição de novos tipos de entidade
- [ ] Fácil adição de novos comportamentos
- [ ] Arquitetura preparada para networking

---

## Próximos Passos

1. **Implementar componentes básicos** (Movement, Attack, Animation)
2. **Criar sistemas especializados** com source generators
3. **Configurar EventBus** para comunicação entre sistemas
4. **Testar integração** com entidades existentes
5. **Otimizar performance** baseado em profiling

### Questões Específicas para Implementação:
1. Como estruturar componentes para máxima reutilização?
2. Qual a melhor estratégia para sistemas que processam múltiplos tipos de entidade?
3. Como organizar eventos para evitar dependências circulares?
4. Quais padrões seguir para facilitar debugging e manutenção?