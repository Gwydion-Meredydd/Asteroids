using Unity.Entities;

#region SummarySection
/// <summary>
///struct data tag to tell if the asteroid system that the asetoid has collided
/// </summary>
/// <param name="UpgradeData"></param>

#endregion
[GenerateAuthoringComponent]
public struct UpgradeData : IComponentData
{
    public bool HasCollided;
}
