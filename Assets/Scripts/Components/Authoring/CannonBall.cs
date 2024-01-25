using Unity.Entities;
using Unity.Mathematics;

namespace Components.Authoring
{
    public struct CannonBall : IComponentData
    {
        public float3 Velocity;
    }
}