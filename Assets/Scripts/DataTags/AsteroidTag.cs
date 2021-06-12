using Unity;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AsteroidTag : IComponentData
{
    public bool Initalised;
    public bool Hit;
    public int Health;
    public float Speed;
    public float4x4 currentAsteroidScale;
    public float4x4 asteroidLargeScale;
    public float4x4 asteroidMediumScale;
    public float4x4 asteroidSmallScale;
}
