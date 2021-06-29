using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

#region SummarySection
/// <summary>
///  system class that is responssible for moving the ship using the ship data veriables also is responsible for hyperjump mechanic
/// </summary>
/// <param name="ShipMoveSystem"></param>

#endregion
public class ShipMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!GameManager.Instance.Paused)
        {

            float deltaTime = Time.DeltaTime;
            Entities.WithAll<ShipData>().ForEach((ref Translation pos, ref Rotation rot, ref ShipData shipData,ref ShipWallCollisionData shipWallCollisionData, in LocalToWorld ltw) =>
            {
                if (!shipData.Teleport)
                {
                    //moves the ship according to the ship data veriables set in ship system
                    pos.Value += ltw.Forward * shipData.activeForwardSpeed * deltaTime;
                    pos.Value += ltw.Right * shipData.activeStrafeSpeed * deltaTime;

                    rot.Value = math.mul(rot.Value, quaternion.RotateX(math.radians(-shipData.mouseDistance.y * shipData.lookRateSpeed * deltaTime)));
                    rot.Value = math.mul(rot.Value, quaternion.RotateY(math.radians(shipData.mouseDistance.x * shipData.lookRateSpeed * deltaTime)));
                    rot.Value = math.mul(rot.Value, quaternion.RotateZ(math.radians(shipData.rollInput * shipData.rollSpeed * deltaTime)));
                    shipData.Pos = pos.Value;
                }
                else if (shipData.Teleport)
                {
                    //teleports the ship to a random location within the play space
                    AudioManager.Instance.PlayPlayerTeleport();
                    shipData.Teleport = false;
                    pos.Value = new Vector3(
                    ReturnValue(AsteroidManager.Instance.SpawnMax.x - 100, AsteroidManager.Instance.SpawnMin.x + 100),
                    ReturnValue(AsteroidManager.Instance.SpawnMax.y - 100, AsteroidManager.Instance.SpawnMin.y + 100),
                    ReturnValue(AsteroidManager.Instance.SpawnMax.z - 100, AsteroidManager.Instance.SpawnMin.z + 100));
                }
                if (shipWallCollisionData.HasCollided)
                {
                    //teleports the ship to a random location within the play space
                    AudioManager.Instance.PlayPlayerTeleport();
                    shipWallCollisionData.HasCollided = false;
                    shipData.Teleport = false;
                    pos.Value = new Vector3(
                    ReturnValue(AsteroidManager.Instance.SpawnMax.x - 100, AsteroidManager.Instance.SpawnMin.x + 100),
                    ReturnValue(AsteroidManager.Instance.SpawnMax.y - 100, AsteroidManager.Instance.SpawnMin.y + 100),
                    ReturnValue(AsteroidManager.Instance.SpawnMax.z - 100, AsteroidManager.Instance.SpawnMin.z + 100));
                }

            }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
        }                       //.run should work by it self)
    }
    public float ReturnValue(float max, float Min) 
    {
        float returnvalue = UnityEngine.Random.Range(Min, max);
        return (returnvalue);
    }
}
