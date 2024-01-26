using Components.Authoring;
using Components.Authoring.Execute;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct CannonBallMovementSystem : ISystem
    {
        private ComponentTypeHandle<CannonBall> _cannonBallHandle;
        private ComponentTypeHandle<LocalTransform> _transformHandle;
        private ComponentTypeHandle<RemoveSelf> _removeSelfHandle;
        
        private EntityQuery _cannonBallQuery;
        private CannonBallMovementJobChunk _job;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<
            //     EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CannonBallMovement>();

            _cannonBallHandle = state.GetComponentTypeHandle<CannonBall>(isReadOnly: false);
            _transformHandle = state.GetComponentTypeHandle<LocalTransform>(isReadOnly: false);
            _removeSelfHandle = state.GetComponentTypeHandle<RemoveSelf>(isReadOnly: false);
            
            _cannonBallQuery = SystemAPI.QueryBuilder().WithAll<
                CannonBall,
                LocalTransform,
                RemoveSelf
            >().Build();
            _job = new CannonBallMovementJobChunk();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = SystemAPI.GetSingleton<
            //     EndSimulationEntityCommandBufferSystem.Singleton>();
            // var cannonBallJob = new CannonBallMovementJob
            // {
            //     ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged),
            //     DeltaTime = SystemAPI.Time.DeltaTime,
            // };
            //
            // cannonBallJob.Schedule();
            
            _cannonBallHandle.Update(ref state);
            _transformHandle.Update(ref state);
            _removeSelfHandle.Update(ref state);

            _job.DeltaTime = SystemAPI.Time.DeltaTime;
            _job.CannonBallHandle = _cannonBallHandle;
            _job.TransformHandle = _transformHandle;
            _job.RemoveSelfHandle = _removeSelfHandle;

            state.Dependency = _job.ScheduleParallel(_cannonBallQuery, state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct CannonBallMovementJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float DeltaTime;

        [BurstCompile]
        private void Execute(Entity entity, ref CannonBall cannonBall, 
            ref LocalTransform transform)
        {
            var gravity = new float3(0.0f, -9.82f, 0.0f);
            var invertY = new float3(1.0f, -1.0f, 1.0f);

            transform.Position += cannonBall.Velocity * DeltaTime;

            if (transform.Position.y < 0.0f)
            {
                transform.Position *= invertY;
                cannonBall.Velocity *= invertY * 0.8f;
            }

            cannonBall.Velocity += gravity * DeltaTime;

            var speed = math.lengthsq(cannonBall.Velocity);
            if (speed >= 0.1f) return;
            
            ECB.DestroyEntity(entity);
        }
    }

    [BurstCompile]
    internal struct CannonBallMovementJobChunk : IJobChunk
    {
        public float DeltaTime;
        
        public ComponentTypeHandle<CannonBall> CannonBallHandle;
        public ComponentTypeHandle<LocalTransform> TransformHandle;
        public ComponentTypeHandle<RemoveSelf> RemoveSelfHandle;
        
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, 
            bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var balls = chunk.GetNativeArray(ref CannonBallHandle);
            var transforms = chunk.GetNativeArray(ref TransformHandle);
            var removeSelfies = chunk.GetNativeArray(ref RemoveSelfHandle);

            var enumerator = new ChunkEntityEnumerator(false, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out var i))
            {
                var cannonBall = balls[i];
                var transform = transforms[i];
                
                var gravity = new float3(0.0f, -9.82f, 0.0f);
                var invertY = new float3(1.0f, -1.0f, 1.0f);

                transform.Position += cannonBall.Velocity * DeltaTime;

                if (transform.Position.y < 0.0f)
                {
                    transform.Position *= invertY;
                    cannonBall.Velocity *= invertY * 0.8f;
                }

                cannonBall.Velocity += gravity * DeltaTime;

                balls[i] = cannonBall;
                transforms[i] = transform;
                
                var speed = math.lengthsq(cannonBall.Velocity);
                if (speed >= 0.1f) continue;

                removeSelfies[i] = new RemoveSelf
                {
                    Trigger = true,
                };
            }
        }
    }
}