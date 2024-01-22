using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TankAuthoring : MonoBehaviour
    {
        private class Baker : Baker<TankAuthoring>
        {
            public override void Bake(TankAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Tank>(entity);
            }
        }
    }
}