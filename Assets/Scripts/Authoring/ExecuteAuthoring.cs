using Components;
using Components.Authoring;
using Components.Authoring.Execute;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        public bool turretMovement;
        public bool tankMovement;
        public bool turretShooting;
        public bool cannonBallMovement;
        public bool tankSpawn;
        private class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                if (authoring.turretMovement) AddComponent<TurretMovement>(entity);
                if (authoring.tankMovement) AddComponent<TankMovement>(entity);
                if (authoring.turretShooting) AddComponent<TurretShooting>(entity);
                if (authoring.cannonBallMovement) AddComponent<CannonBallMovement>(entity);
                if (authoring.tankSpawn) AddComponent<TankSpawn>(entity);
            }
        }
    }
}