using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class TurretAuthoring : MonoBehaviour
    {
        public float degreesPerSecond;
        
        private class Baker : Baker<TurretAuthoring>
        {
            public override void Bake(TurretAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<TurretComponent>(entity);
                AddComponent(entity, new MovementComponent
                {
                    RadiusPerSecond = math.radians(authoring.degreesPerSecond),
                });
            }
        }
    }
}
