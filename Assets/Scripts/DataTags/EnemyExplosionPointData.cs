using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
///struct data tag to know the location of the enemy explosion point location
/// </summary>
/// <param name="ShootData"></param>

#endregion
[GenerateAuthoringComponent]
public struct EnemyExplosionPointData : IComponentData
{
    public bool ExplosionParticleSystemInstantiated;
}