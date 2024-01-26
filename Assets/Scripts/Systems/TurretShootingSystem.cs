using Aspects;
using Components.Authoring;
using Components.Authoring.Execute;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TurretShootingSystem : ISystem
    {
        private int _frame;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TurretShooting>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_frame++ % 4 != 0) return;
            foreach (var (turret, localToWorld) in
                     SystemAPI.Query<TurretAspect, RefRO<LocalToWorld>>())
            {
                var pos = SystemAPI.GetComponent<LocalToWorld>(turret.CannonBallSpawn).Position;
                var instance = state.EntityManager.Instantiate(turret.CannonBallPrefab);
                state.EntityManager.SetComponentData(instance, new LocalTransform
                {
                    Position = pos,
                    Rotation = quaternion.identity,
                    Scale = SystemAPI.GetComponent<LocalTransform>(turret.CannonBallPrefab).Scale,
                });
                state.EntityManager.SetComponentData(instance, new CannonBall
                {
                    Velocity = localToWorld.ValueRO.Up * 20.0f,
                });
                // Debug.Log($"pos: {pos}, velocity: {localToWorld.ValueRO.Up * 20.0f}");
                state.EntityManager.SetComponentData(instance, new URPMaterialPropertyBaseColor
                {
                    Value = turret.Color,
                });
            }

            // state.Enabled = false;
        }
    }
}