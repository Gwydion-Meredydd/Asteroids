using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

#region SummarySection
/// <summary>
/// System that is responsible for handling ships collisions with asteroids, sets ship collision data. has collided to true on collision
/// </summary>
/// <param name="ShipShootSystem"></param>

#endregion

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ShipAsteroidCollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    [BurstCompile]
    struct CollisionDamageSystem : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<ShipData> shipDataGroup;
        [ReadOnly] public ComponentDataFromEntity<AsteroidData> asteroidDataGroup;

        public ComponentDataFromEntity<ShipAsteroidCollisionData> collisionDataGroup;
        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool shipisEntityA = shipDataGroup.HasComponent(entityA);
            bool asteroidisEntityA = asteroidDataGroup.HasComponent(entityA);
            bool shipisEntityB = shipDataGroup.HasComponent(entityB);
            bool asteroidisEntityB = asteroidDataGroup.HasComponent(entityB);

            if (shipisEntityA && asteroidisEntityB) 
            {
                ShipAsteroidCollisionData shipCollisionData = new ShipAsteroidCollisionData();
                shipCollisionData.HasCollided = true;

                collisionDataGroup[entityA] = shipCollisionData;
            }

            if (shipisEntityB && asteroidisEntityA)
            {
                ShipAsteroidCollisionData shipCollisionData = new ShipAsteroidCollisionData();
                shipCollisionData.HasCollided = true;

                collisionDataGroup[entityB] = shipCollisionData;
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new CollisionDamageSystem();

        job.shipDataGroup = GetComponentDataFromEntity<ShipData>(true);
        job.asteroidDataGroup = GetComponentDataFromEntity<AsteroidData>(true);
        job.collisionDataGroup = GetComponentDataFromEntity<ShipAsteroidCollisionData>(false);


        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);

        jobHandle.Complete();

        return jobHandle;
    }
}
