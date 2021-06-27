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
public class ShipSparkSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<SparksPointData>().ForEach((ref SparksPointData sparksPointData, in Translation pos) =>
        {
            //initlises the sparks particle systems via shipmanager method
            if (!sparksPointData.SparksParticleSystemInstantiated)
            {
                sparksPointData.SparksParticleSystemInstantiated = true;
                ShipManager.Instance.SparkParticleSystemInstantiated(pos.Value);
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)
    }
}
