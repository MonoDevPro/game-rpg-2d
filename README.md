# Game RPG 2D - Sistema de Movimento em Grid com Arch ECS

Este projeto implementa um sistema base para um jogo RPG 2D estilo top-down usando o framework ECS Arch no Godot 4.5 com C#.

## Características Implementadas

### ✅ Sistema de Movimento em Grid
- Movimento baseado em grid de 32x32 pixels
- Suporte para 8 direções (incluindo diagonais)
- Movimento suave com interpolação usando ease-out
- Sistema ECS usando Arch framework

### ✅ Controles
- **W** - Mover para cima
- **S** - Mover para baixo
- **A** - Mover para esquerda
- **D** - Mover para direita
- **Combinações** - Movimento diagonal (ex: W+D = nordeste)

### ✅ Câmera
- Câmera segue o jogador automaticamente
- Movimento suave configurável
- Integração com o sistema ECS

## Estrutura do Projeto

```
Scripts/
├── GameManager.cs              # Gerenciador principal e singleton
├── CameraController.cs         # Controle da câmera
├── Components/                 # Componentes ECS
│   ├── PositionComponent.cs    # Posição no grid e mundo
│   ├── MovementComponent.cs    # Estado do movimento
│   ├── InputComponent.cs       # Input do jogador
│   └── NodeComponent.cs        # Referência ao Node2D
├── Systems/                    # Sistemas ECS
│   ├── InputSystem.cs          # Processamento de input
│   ├── MovementSystem.cs       # Lógica de movimento
│   └── RenderSystem.cs         # Sincronização visual
└── Entities/
    └── Player.cs               # Script do jogador
```

## Como Usar

1. **Configurar a Cena Principal:**
   - Abra `Main.tscn` no editor do Godot
   - A cena já está configurada com GameManager, Player e Camera

2. **Personalizar o Jogador:**
   - No nó Player, você pode ajustar:
     - `MoveSpeed`: Velocidade do movimento (padrão: 4.0)
     - `StartGridPosition`: Posição inicial no grid

3. **Personalizar a Câmera:**
   - No nó Camera2D, você pode ajustar:
     - `FollowSpeed`: Velocidade de seguimento (padrão: 5.0)
     - `SmoothFollow`: Ativar/desativar movimento suave

## Arquitetura ECS

### Componentes
- **PositionComponent**: Armazena posição no grid e posição no mundo
- **MovementComponent**: Controla estado de movimento e interpolação
- **InputComponent**: Armazena direção do input atual
- **NodeComponent**: Referência ao Node2D do Godot

### Sistemas
- **InputSystem**: Captura input WASD e converte para direções
- **MovementSystem**: Processa movimento no grid com interpolação
- **RenderSystem**: Sincroniza posições ECS com nós do Godot

## Próximos Passos Sugeridos

1. **Adicionar Sprites**: Substitua o ColorRect por sprites animados
2. **Sistema de Colisão**: Implementar verificação de colisões no grid
3. **Múltiplas Entidades**: Adicionar NPCs e inimigos
4. **Sistema de Animação**: Integrar animações de caminhada
5. **Mapa/Tilemap**: Implementar sistema de mapas

## Dependências

- Godot 4.5
- .NET 8.0
- Arch ECS 2.1.0

O sistema está pronto para ser executado e expandido conforme suas necessidades!
