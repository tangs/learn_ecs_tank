using Components;
using Components.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct SafeZoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SafeZone>();
            state.RequireForUpdate<TankConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var radius = SystemAPI.GetSingleton<TankConfig>().SafeZoneRadius;
            
            const float debugRenderStepInDegrees = 20;
            for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees)
            {
                var a = float3.zero;
                var b = float3.zero;
                math.sincos(math.radians(angle), out a.x, out a.z);
                math.sincos(math.radians(angle + debugRenderStepInDegrees), out b.x, out b.z);
                Debug.DrawLine(a * radius, b * radius);
            }

            var job = new SafeZoneJob
            {
                SquaredRadius = radius * radius,
            };
            job.ScheduleParallel();
        }
    }
    
    [WithAll(typeof(Turret))]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct SafeZoneJob : IJobEntity
    {
        public float SquaredRadius;

        private void Execute(in LocalToWorld transformMatrix, 
            EnabledRefRW<Shooting> shootingState)
        {
            shootingState.ValueRW = math.lengthsq(transformMatrix.Position) > SquaredRadius;
        }
    }
}