using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

public class ShipShootSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        if (Input.GetMouseButton(0))
        {
            ShipManager.shipManager.ShootParticles();
            Entities.ForEach((ref ShootData shootData,in Translation pos ,in LocalToWorld ltw, in LocalToParent ltp) =>
            {
                if (!shootData.ShootParticleSystemInstantiated) 
                {
                    shootData.ShootParticleSystemInstantiated = true;
                    ShipManager.shipManager.ShootParticleSystemInstantiated(pos.Value);
                }
                UnityEngine.Ray ray = new UnityEngine.Ray(ltw.Position, ltw.Forward);
                var hitobject = Raycast(ray.origin, ray.direction  * 100000f);
                if (hitobject != null)
                {
                    try
                    {
                        shootData.asteroidData = new AsteroidData();
                        shootData.asteroidData = EntityManager.GetComponentData<AsteroidData>(hitobject);


                        if (shootData.asteroidData.Hit == false)
                        {
                            shootData.asteroidData.Health = shootData.asteroidData.Health - 1;
                            shootData.asteroidData.Hit = true;
                            EntityManager.SetComponentData(hitobject, shootData.asteroidData);
                            Debug.Log("hit    ");
                            if (shootData.asteroidData.Health <= 0)
                            {
                                shootData.asteroidData.Dead = true;
                                EntityManager.SetComponentData(hitobject, shootData.asteroidData);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("failed    " + hitobject + "  " + e.ToString());
                    }
                }
                Debug.Log("Clearing");
            }).WithoutBurst().Run();
        }
    }
    public Entity Raycast(float3 fromPos, float3 toPos)
    {
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        RaycastInput raycastInput = new RaycastInput
        {
            Start = fromPos,
            End = toPos,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0,

            }
        };
        Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

        if (collisionWorld.CastRay(raycastInput, out raycastHit))
        {
            Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return hitEntity;
        }
        else
        {
            return Entity.Null;
        }
    }
}
