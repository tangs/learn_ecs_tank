using Behaviour;
using Components.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct CameraFollowSystem : ISystem
    {
        private Entity _target;
        private Random _random;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraFollow>();
            _random = new Random(123);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_target == Entity.Null || Input.GetKeyDown(KeyCode.Space))
            {
                var tankQuery = SystemAPI.QueryBuilder().WithAll<Tank>().Build();
                var tanks = tankQuery.ToEntityArray(Allocator.Temp);
                if (tanks.Length == 0) return;

                _target = tanks[_random.NextInt(tanks.Length)];
            }

            var cameraTransform = CameraSingleton.Instance.transform;
            var tankTransform = state.EntityManager.GetComponentData<LocalToWorld>(_target);
            var position = tankTransform.Position;
            position -= 10.0f * tankTransform.Forward;
            position.y += 5f;
            cameraTransform.position = position;
            cameraTransform.LookAt(tankTransform.Position);
        }
    }
}