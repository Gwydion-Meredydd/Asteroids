using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

#region SummarySection
/// <summary>
/// System that shoots a raycast from each laser on the ship when the left mouse button is down and sets the shootdata information. It also will find if the ship lasers have been initlised
/// </summary>
/// <param name="ShipShootSystem"></param>

#endregion
public class ShipShootSystem : SystemBase
{
    public bool CanShoot;

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        if (ShipManager.Instance.CanShoot)
        {
            if (Input.GetMouseButton(0))
            {
                AudioManager.Instance.PlayPlayerShoot();
                ShipManager.Instance.ShootParticles();
                Entities.ForEach((ref ShootData shootData, in Translation pos, in LocalToWorld ltw, in LocalToParent ltp) =>
                {
                    //initlises the lasers particle systems via shipmanager method
                    if (!shootData.ShootParticleSystemInstantiated)
                    {
                        shootData.ShootParticleSystemInstantiated = true;
                        ShipManager.Instance.ShootParticleSystemInstantiated(pos.Value);
                    }

                    //creates a new ray from each laser position
                    UnityEngine.Ray ray = new UnityEngine.Ray(ltw.Position, ltw.Forward);
                    var hitobject = Raycast(ray.origin, ray.direction * 100000f);

                    if (hitobject != null)
                    {
                        //try catch block is used to see if the raycast hit an asteroid
                        //gets the asetoid data and reduces the health then sets the asteroid data back
                        try
                        {
                            shootData.asteroidData = new AsteroidData();
                            shootData.asteroidData = EntityManager.GetComponentData<AsteroidData>(hitobject);


                            if (shootData.asteroidData.Hit == false)
                            {
                                shootData.asteroidData.Health = shootData.asteroidData.Health - 1;
                                shootData.asteroidData.Hit = true;
                                EntityManager.SetComponentData(hitobject, shootData.asteroidData);
                                if (shootData.asteroidData.Health <= 0)
                                {
                                    shootData.asteroidData.Dead = true;
                                    EntityManager.SetComponentData(hitobject, shootData.asteroidData);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("failed  " + e.ToString());
                        }
                    }
                }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                        //.run should work by it self)
                
                //shoot cool down ensures controllable timing for the lasers
                ShipManager.Instance.StartCoroutine("ShootCoolDown");
            }
        }
    }
    //raycast method that returns hit entity 
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
