using System;

namespace GameRpg2D.Scripts.ECS.Components
{
    /// <summary>
    /// Atributo para marcar structs como componentes do ECS
    /// (Substituto para o Arch.AOT.SourceGenerator que estava com bug)
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
    }
}
