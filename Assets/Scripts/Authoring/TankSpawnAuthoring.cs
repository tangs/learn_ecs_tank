using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TankSpawnAuthoring : MonoBehaviour
    {
        public GameObject tankPrefab;
        public int tankCount;
        public float safeZoneRadius;
        
        private class Baker : Baker<TankSpawnAuthoring>
        {
            public override void Bake(TankSpawnAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent(entity, new TankConfig
                {
                    TankPrefab = GetEntity(authoring.tankPrefab, TransformUsageFlags.Dynamic),
                    TankCount = authoring.tankCount,
                    SafeZoneRadius = authoring.safeZoneRadius,
                });
            }
        }
    }
}