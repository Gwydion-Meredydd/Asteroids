using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class ShipMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation pos, ref Rotation rot, in ShipData shipData,in LocalToWorld ltw) =>
        {
            pos.Value += ltw.Forward * shipData.activeForwardSpeed * deltaTime;
            pos.Value += ltw.Right * shipData.activeStrafeSpeed * deltaTime;
            if (shipData.mouseDistance.y == 0 || shipData.mouseDistance.x == 0 || shipData.lookRateSpeed == 0 
            || shipData.rollInput == 0 || shipData.rollSpeed == 0) { return; }

             rot.Value = math.mul(rot.Value, quaternion.RotateX(math.radians(-shipData.mouseDistance.y * shipData.lookRateSpeed * deltaTime)));
             rot.Value = math.mul(rot.Value, quaternion.RotateY(math.radians(shipData.mouseDistance.x * shipData.lookRateSpeed * deltaTime)));
             rot.Value = math.mul(rot.Value, quaternion.RotateZ(math.radians(shipData.rollInput * shipData.rollSpeed * deltaTime)));
            
        }).Run();
    }
}
