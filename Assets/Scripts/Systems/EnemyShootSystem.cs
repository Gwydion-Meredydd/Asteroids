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
/// system that is responsible for the enemy firing at the player
/// </summary>
/// <param name="ShipShootSystem"></param>

#endregion
public class EnemyShootSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<EnemyShootData>().ForEach((ref EnemyShootData enemyShootData, in Entity ent, in Translation pos, in LocalToWorld ltw, in LocalToParent ltp) =>
        {
            //initlises the lasers particle systems via shipmanager method
            if (!enemyShootData.ShootParticleSystemInstantiated)
            {
                enemyShootData.ShootParticleSystemInstantiated = true;
                enemyShootData.CanShoot = false;
                enemyShootData.ShootCooldown = EnemyManager.Instance.ShootCoolDown;
                EnemyManager.Instance.ShootParticleSystemInstantiated(pos.Value);
            }
            if (GameManager.Instance.InGame)
            {
                if (!GameManager.Instance.InMenu)
                {
                    Vector3 ShipPos = ShipManager.Instance.ShipCamera.position;
                    float ShipDistance = Vector3.Distance(ShipPos, ltw.Position);
                    if (enemyShootData.CanShoot)
                    {
                        if (ShipDistance < 200)
                        {
                            Debug.Log("cAN SHOOT 1");
                            AudioManager.Instance.PlayEnemyShoot();
                            EnemyManager.Instance.ShootParticleLaser(ent);

                            //random chance of hitting the ship if in distane or if the player ship isnt moving
                            if (UnityEngine.Random.Range(0, EnemyManager.Instance.HitChange) == EnemyManager.Instance.HitChange / 2 
                            || ShipManager.Instance.isMoving == false)
                            {
                                Debug.Log("cAN SHOOT 2");
                                //creates a new ray from each laser position
                                UnityEngine.Ray ray = new UnityEngine.Ray(ltw.Position, ltw.Forward);
                                var hitobject = Raycast(ray.origin, ray.direction * 200f);

                                if (hitobject != null)
                                {
                                    Debug.Log("cAN SHOOT 3");
                                    
                                    ShipManager.Instance.Health -= 1;
                                    UserInterfaceManager.Instance.UpdateHealth();
                                    
                                    //creates the ship to spark when damage 
                                    if (ShipManager.Instance.Health > 2)
                                    {
                                        if (ShipManager.Instance.ShipSparks[0].isPlaying)
                                        {
                                            ShipManager.Instance.ShipSparks[0].Stop();
                                        }
                                        if (ShipManager.Instance.ShipSparks[1].isPlaying)
                                        {
                                            ShipManager.Instance.ShipSparks[1].Stop();
                                        }
                                    }
                                    else if (ShipManager.Instance.Health == 2)
                                    {
                                        ShipManager.Instance.ShipSparks[0].Play();
                                    }
                                    else if (ShipManager.Instance.Health == 1)
                                    {
                                        ShipManager.Instance.ShipSparks[1].Play();
                                    }
                                    else if (ShipManager.Instance.Health <= 0)
                                    {
                                        if (!ShipManager.Instance.ShipDied)
                                        {
                                            ShipManager.Instance.DestroyShip();
                                        }
                                    }
                                }
                            }
                            enemyShootData.CurrentCooldown = enemyShootData.ShootCooldown;
                            enemyShootData.CanShoot = false;
                        }
                    }
                    else
                    {
                        enemyShootData.CurrentCooldown -= 0.1f;
                        if (enemyShootData.CurrentCooldown <= 0)
                        {
                            enemyShootData.CanShoot = true;
                        }
                    }
                }
                else
                {
                    enemyShootData.CanShoot = true;
                }
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)

        //shoot cool down ensures controllable timing for the lasers
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
