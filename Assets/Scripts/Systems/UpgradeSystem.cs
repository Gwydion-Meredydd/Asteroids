using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

#region SummarySection
/// <summary>
///  system class that is used to trigger collision event between ship and upgrade entity
/// </summary>
/// <param name="UpgradeSystem"></param>

#endregion
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class UpgradeSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    //sets unity dots veriables needeed to handle collsion trigger events
    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    //unity dots job handle for collison triggers
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new UpgradeSystemJob();

        job.allUpgradePickups = GetComponentDataFromEntity<UpgradeData>(true);
        job.allShips = GetComponentDataFromEntity<ShipData>(true);
        job.entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

    [BurstCompile]
    struct UpgradeSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<UpgradeData> allUpgradePickups;
        [ReadOnly] public ComponentDataFromEntity<ShipData> allShips;

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            //checks if the the ship collided with the upgrade entity or vise versa
            if (allUpgradePickups.HasComponent(entityA) && allUpgradePickups.HasComponent(entityB))
            {
                return;
            }
            if (allUpgradePickups.HasComponent(entityA) && allShips.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityA);
            }
            else if (allShips.HasComponent(entityA) && allUpgradePickups.HasComponent(entityB))
            {
                //has collided is used in upgradecollisionssytem to know when the box can be removed
                UpgradeData upgradeData = new UpgradeData();
                upgradeData.HasCollided = true;
                entityCommandBuffer.SetComponent<UpgradeData>(entityB, upgradeData);
            }

        }
    }
}
