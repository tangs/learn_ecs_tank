using Components.Authoring;
using Components.Authoring.Execute;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct CannonBallMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<
                EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CannonBallMovement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<
                EndSimulationEntityCommandBufferSystem.Singleton>();
            var cannonBallJob = new CannonBallMovementJob
            {
                ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged),
                DeltaTime = SystemAPI.Time.DeltaTime,
            };

            cannonBallJob.Schedule();
        }
    }

    [BurstCompile]
    public partial struct CannonBallMovementJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float DeltaTime;

        private void Execute(Entity entity, ref CannonBall cannonBall, ref LocalTransform transform)
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
}