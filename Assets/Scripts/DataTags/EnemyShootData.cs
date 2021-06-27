using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
///struct data tag to hold enemy shoot  data and to know from which point the ship shoots lasers from
/// </summary>
/// <param name="ShootData"></param>

#endregion
[GenerateAuthoringComponent]
public struct EnemyShootData : IComponentData
{
    public ShipData shipData;
    public Vector3 ShootPoint;
    public float ShootCooldown;
    public float CurrentCooldown;
    
    public bool CanShoot;
    public bool ShootParticleSystemInstantiated;
}