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
/// system class thats respobbile for enemy ship movement and rotation
/// </summary>
/// <param name="EnemyShipSystem"></param>

#endregion
public class EnemyShipSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithAll<EnemyShipData>().ForEach((ref EnemyShipData enemyShipData, ref Translation pos,ref Rotation rot,in Entity ent, in LocalToWorld ltw) =>
        {
            if (GameManager.Instance.InGame)
            {
                if (!GameManager.Instance.InMenu)
                {
                    if (!GameManager.Instance.Paused)
                    {
                        if (!EnemyManager.Instance.EntityToDestory)
                        {
                            //initilisation
                            if (!enemyShipData.SpeedInstantiated)
                            {
                                float MoveSpeed = UnityEngine.Random.Range(EnemyManager.Instance.MoveSpeed - 5, EnemyManager.Instance.MoveSpeed);
                                float RotateSpeed = UnityEngine.Random.Range(EnemyManager.Instance.RotationSpeed - 1, EnemyManager.Instance.RotationSpeed);
                                enemyShipData.MoveSpeed = MoveSpeed;
                                enemyShipData.RotateSpeed = RotateSpeed;
                                enemyShipData.SpeedInstantiated = true;
                                EnemyManager.Instance.SpawnRefrenceGO(pos.Value, rot.Value, ent);
                            }

                            //rotation 
                            float3 ShipPos = ShipManager.Instance.ShipCamera.position;
                            float3 dirtoTarget = ShipPos - pos.Value;
                            quaternion targetrotation = quaternion.LookRotationSafe(dirtoTarget, math.down());
                            rot.Value = math.slerp(rot.Value, targetrotation, deltaTime * enemyShipData.RotateSpeed);

                            //movement
                            if (Vector3.Distance(ShipPos, pos.Value) > EnemyManager.Instance.StoppingDistance)
                            {
                                pos.Value += ltw.Forward * enemyShipData.MoveSpeed * deltaTime;
                            }
                            EnemyManager.Instance.SetNewPosandRot(pos.Value, rot.Value, ent);
                        }
                    }
                }
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)
    }
}