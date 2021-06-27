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
public class EnemyShipExplosionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<EnemyExplosionPointData>().ForEach((ref EnemyExplosionPointData enemyExplosionPointData, in Translation pos) =>
        {
            //initlises the enemy explosion particle systems via shipmanager method
            if (!enemyExplosionPointData.ExplosionParticleSystemInstantiated)
            {
                enemyExplosionPointData.ExplosionParticleSystemInstantiated = true;
                EnemyManager.Instance.ExplosionParticleSystemInstantiated(pos.Value);
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)
    }
}
