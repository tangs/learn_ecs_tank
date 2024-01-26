using Components.Authoring;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class CannonBallAuthoring : MonoBehaviour
    {
        private class Baker : Baker<CannonBallAuthoring>
        {
            public override void Bake(CannonBallAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<CannonBall>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
                AddComponent<RemoveSelf>(entity);
            }
        }
    }
}