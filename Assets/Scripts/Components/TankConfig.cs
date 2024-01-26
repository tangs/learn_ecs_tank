using Unity.Entities;

namespace Components
{
    public struct TankConfig : IComponentData
    {
        public Entity TankPrefab;
        public int TankCount;
        public float SafeZoneRadius;
    }
}