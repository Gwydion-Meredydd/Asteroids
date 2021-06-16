using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ShootData : IComponentData
{
    public Entity HitObject;
    public AsteroidData asteroidData;
    public Vector3 ShootPoint;
    public bool ShootParticleSystemInstantiated;
}