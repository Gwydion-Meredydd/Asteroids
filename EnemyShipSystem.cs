using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

#region SummarySection
/// <summary>
/// system class thats respobbile for taking input data and converting into movement data for the ship move system to use
/// </summary>
/// <param name="EnemyShipSystem"></param>

#endregion
public class EnemyShipSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref EnemyShipData enemyShipData, ref Translation pos,ref Rotation rot, in LocalToWorld ltw) =>
        {
            if (GameManager.Instance.InGame)
            {
                if (!GameManager.Instance.InMenu)
                {
                    if (!GameManager.Instance.Paused)
                    {
                        float3 ShipPos = ShipManager.Instance.ShipCamera.position;
                        Quaternion ShipRot = ShipManager.Instance.ShipCamera.rotation;
                        var newRotation = RotateTowards(rot.Value, ShipRot , deltaTime * 10);

                    }
                }
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)
    }
    public  quaternion RotateTowards(
            quaternion from,
            quaternion to,
            float maxDegreesDelta)
    {
        float num = Angle(from, to);
        return num < float.Epsilon ? to : math.slerp(from, to, math.min(1f, maxDegreesDelta / num));
    }

    
    public  float Angle(this quaternion q1, quaternion q2)
    {
        var dot = math.dot(q1, q2);
        return !(dot > 0.999998986721039) ? (float)(math.acos(math.min(math.abs(dot), 1f)) * 2.0) : 0.0f;
    }
}