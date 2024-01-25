using Unity.Entities;

namespace Components.Authoring
{
    public struct Turret : IComponentData
    {
        public Entity CannonBallPrefab;
        public Entity CannonBallSpawn;
    }
}