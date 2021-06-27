using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
///struct data tag to know the location of the explosion point on enemy ships
/// </summary>
/// <param name="ShootData"></param>

#endregion
[GenerateAuthoringComponent]
public struct ExplosionPointData : IComponentData
{
    public bool ExplosionParticleSystemInstantiated;
}