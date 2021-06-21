using Unity;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
///struct data tag to hold asteroid data for each asteroid
/// </summary>
/// <param name="AsteroidData"></param>

#endregion

[GenerateAuthoringComponent]
public struct AsteroidData : IComponentData
{
    public bool Initalised;
    public bool Hit;
    public bool Dead;
    public int Health;
    public float Speed;
    public float4x4 currentAsteroidScale;
    public float4x4 asteroidLargeScale;
    public float4x4 asteroidMediumScale;
    public float4x4 asteroidSmallScale;
}
