using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
///struct data tag to know the location of the spark point location
/// </summary>
/// <param name="ShootData"></param>

#endregion
[GenerateAuthoringComponent]
public struct SparksPointData : IComponentData
{
    public bool SparksParticleSystemInstantiated;
}