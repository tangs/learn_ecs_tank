using Components;
using Components.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct TankSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankSpawn>();
            state.RequireForUpdate<TankConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var random = new Random(123);
            
            var tankConfig = SystemAPI.GetSingleton<TankConfig>();

            var query = SystemAPI.QueryBuilder().
                WithAll<URPMaterialPropertyBaseColor>().Build();
            var queryMask = query.GetEntityQueryMask();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tanks = new NativeArray<Entity>(tankConfig.TankCount,
                Allocator.Temp);
            ecb.Instantiate(tankConfig.TankPrefab, tanks);
            
            foreach (var tank in tanks)
            {
                ecb.SetComponentForLinkedEntityGroup(tank, queryMask, 
                    new URPMaterialPropertyBaseColor
                {
                    Value = RandomColor(ref random)
                });
            }
            
            ecb.Playback(state.EntityManager);
        }
        
        private static float4 RandomColor(ref Random random)
        {
            // 0.618034005f is inverse of the golden ratio
            var hue = (random.NextFloat() + 0.618034005f) % 1;
            return (Vector4)Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }
}