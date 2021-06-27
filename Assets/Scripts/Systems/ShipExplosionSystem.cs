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
public class ShipExplosionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<ExplosionPointData>().ForEach((ref ExplosionPointData explosionPointData, in Translation pos) =>
        {
            //initlises the explosion particle systems via shipmanager method
            if (!explosionPointData.ExplosionParticleSystemInstantiated)
            {
                explosionPointData.ExplosionParticleSystemInstantiated = true;
                ShipManager.Instance.ExplosionParticleSystemInstantiated(pos.Value);
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)
    }
}
