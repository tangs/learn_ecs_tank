using Components;
using Components.Authoring;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class TurretAuthoring : MonoBehaviour
    {
        public float degreesPerSecond;

        public GameObject cannonBallPrefab;
        public Transform cannonBallSpawn;
        
        private class Baker : Baker<TurretAuthoring>
        {
            public override void Bake(TurretAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new Turret
                {
                    CannonBallPrefab = GetEntity(authoring.cannonBallPrefab, TransformUsageFlags.Dynamic),
                    CannonBallSpawn = GetEntity(authoring.cannonBallSpawn, TransformUsageFlags.Dynamic),
                });
                AddComponent(entity, new MovementComponent
                {
                    RadiusPerSecond = math.radians(authoring.degreesPerSecond),
                });
            }
        }
    }
}
