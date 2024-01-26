using Components.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(CannonBallMovementSystem))]
    public partial struct RemoveCannonBallSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            // var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (removeSelf, entity) in SystemAPI.Query<RemoveSelf>().WithEntityAccess())
            {
                if (removeSelf.Trigger is false) continue;
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}