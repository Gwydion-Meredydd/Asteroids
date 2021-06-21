using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
///struct data tag to hold shoot point data, also used to know where the lasers come from
/// </summary>
/// <param name="ShootData"></param>

#endregion
[GenerateAuthoringComponent]
public struct ShootData : IComponentData
{
    public Entity HitObject;
    public AsteroidData asteroidData;
    public Vector3 ShootPoint;
    public bool ShootParticleSystemInstantiated;
}