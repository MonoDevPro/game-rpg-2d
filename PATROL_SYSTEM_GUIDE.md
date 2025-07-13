# Sistema de Patrulha para NPCs - Guia de Configuração

## 📋 Como Configurar um NPC com Patrulha

### **1. No Editor Godot:**

1. **Crie um nó NPC** (que herda de BaseBody)
2. **Configure as propriedades Export:**
   - **Npc Id**: ID único do NPC (ex: 1001)
   - **Npc Name**: Nome do NPC (ex: "Guarda da Torre")
   - **Behaviour Type**: Selecione **Patrol**
   - **Patrol Waypoints String**: Define os waypoints (ex: "0,0;5,0;5,5;0,5")
   - **Patrol Speed**: Velocidade da patrulha (ex: 50.0)
   - **Wait Duration**: Tempo de espera em cada waypoint (ex: 2.0)
   - **Is Looping**: Se deve repetir a patrulha infinitamente
   - **Reverse On End**: Se deve ir e voltar (ping-pong)
   - **Way Point Tolerance**: Tolerância para considerar waypoint alcançado (ex: 0.5)

### **2. Formato dos Waypoints:**

```
"x1,y1;x2,y2;x3,y3;x4,y4"
```

**Exemplos:**
- Patrulha quadrada: `"0,0;5,0;5,5;0,5"`
- Patrulha linear: `"0,0;10,0;20,0"`
- Patrulha em L: `"0,0;5,0;5,5"`

### **3. Tipos de Patrulha:**

#### **Linear + Loop:**
- `Is Looping = true`
- `Reverse On End = false`
- Comportamento: A→B→C→D→A→B→C→D...

#### **Ping-Pong:**
- `Is Looping = true`
- `Reverse On End = true`
- Comportamento: A→B→C→D→C→B→A→B→C→D...

#### **Linear Único:**
- `Is Looping = false`
- `Reverse On End = false`
- Comportamento: A→B→C→D (para)

### **4. Eventos de Debug:**

O sistema emite eventos que podem ser monitorados:
- `PatrolWaypointReachedEvent`: Quando alcança waypoint
- `PatrolStateChangedEvent`: Quando muda estado
- `PatrolCompletedEvent`: Quando completa patrulha
- `PatrolInterruptedEvent`: Quando é interrompido

### **5. Estados do Sistema:**

- **Moving**: Movendo para próximo waypoint
- **Waiting**: Esperando no waypoint atual
- **Returning**: Retornando ao ponto inicial
- **Completed**: Patrulha finalizada
- **Paused**: Patrulha pausada

### **6. Integração com Outros Sistemas:**

- **CollisionSystem**: NPCs evitam obstáculos durante patrulha
- **AnimationSystem**: Animações de movimento são aplicadas automaticamente
- **MovementSystem**: Usa o sistema de movimento grid-based existente

---

## 🎮 Exemplo de Configuração no Godot:

```
[NPC Node]
├── Npc Id: 1001
├── Npc Name: "Guarda da Entrada"
├── Behaviour Type: Patrol
├── Patrol Waypoints String: "0,0;10,0;10,10;0,10"
├── Patrol Speed: 50.0
├── Wait Duration: 3.0
├── Is Looping: true
├── Reverse On End: false
└── Way Point Tolerance: 0.5
```

Este NPC fará uma patrulha quadrada infinita, esperando 3 segundos em cada canto.

---

## 🚀 Sistema Pronto para Uso!

O sistema de patrulha está totalmente integrado ao ECS e funcionando automaticamente para todos os NPCs configurados com `NpcBehaviourType.Patrol`.
