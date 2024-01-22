using Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct TurretRotationSystem : ISystem
    {
        private ComponentTypeHandle<MovementComponent> _movementsHandle;
        private ComponentTypeHandle<LocalTransform> _transformsHandle;
        
        private EntityQuery _turretQuery;
        private JobChunk _job;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TurretComponent>();
            
            _movementsHandle = state.GetComponentTypeHandle<MovementComponent>(isReadOnly: true);
            _transformsHandle = state.GetComponentTypeHandle<LocalTransform>(isReadOnly: false);
            
            _turretQuery = SystemAPI.QueryBuilder()
                .WithAll<TurretComponent, LocalTransform>().Build();
            _job = new JobChunk();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _movementsHandle.Update(ref state);
            _transformsHandle.Update(ref state);

            _job.DeltaTime = SystemAPI.Time.DeltaTime;
            _job.MovementsHandle = _movementsHandle;
            _job.TransHandle = _transformsHandle;

            state.Dependency = _job.ScheduleParallel(_turretQuery, state.Dependency);
        }
    }

    [BurstCompile]
    internal struct JobChunk : IJobChunk
    {
        [ReadOnly]
        public ComponentTypeHandle<MovementComponent> MovementsHandle;
        public ComponentTypeHandle<LocalTransform> TransHandle;
        
        public float DeltaTime;
        
        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var turrets = chunk.GetNativeArray(ref MovementsHandle);
            var transforms = chunk.GetNativeArray(ref TransHandle);
            
            var enumerator = new ChunkEntityEnumerator(false, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out var i))
            {
                transforms[i] = transforms[i].RotateY(turrets[i].RadiusPerSecond * DeltaTime);
            }
        }
    }
}