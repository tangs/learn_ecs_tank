using Unity.Entities;

namespace Components.Authoring
{
    public struct RemoveSelf : IComponentData, IEnableableComponent
    {
        public bool Trigger;
    }
}