using System.Collections.Generic;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    /// <summary>
    /// Componente para armazenar buffer de inputs recebidos da rede para jogadores remotos
    /// </summary>
    public readonly struct BufferedInputComponent(int maxBufferSize = 30)
    {
        private readonly Queue<InputCommand> _inputBuffer = new();

        public void EnqueueInput(Vector2I direction, bool attack, double timestamp)
        {
            // Evitar overflow do buffer
            while (_inputBuffer.Count >= maxBufferSize)
            {
                _inputBuffer.Dequeue();
            }
            
            _inputBuffer.Enqueue(new InputCommand
            {
                Direction = direction,
                Attack = attack,
                Timestamp = timestamp
            });
        }
        
        public bool TryDequeueInput(out InputCommand command)
        {
            if (_inputBuffer.Count > 0)
            {
                command = _inputBuffer.Dequeue();
                return true;
            }
            
            command = default;
            return false;
        }
    }
    
    public struct InputCommand
    {
        public Vector2I Direction;
        public bool Attack;
        public double Timestamp;
    }
}
