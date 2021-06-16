using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

public class RayCastManager : MonoBehaviour
{
    private Entity Raycast(float3 fromPos, float3 toPos)
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

        if (collisionWorld.CastRay(raycastInput,out raycastHit))
        {
            Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return hitEntity; 
        }
        else 
        {
            return Entity.Null;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {

            UnityEngine.Ray ray = new UnityEngine.Ray(ShipManager.shipManager.ShipCamera.position ,ShipManager.shipManager.transform.forward);
            Debug.Log(Raycast(ray.origin, ray.direction * 100000f));

        }
    }
}
