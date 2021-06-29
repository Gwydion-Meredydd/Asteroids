using UnityEngine;
using Unity.Entities;

#region SummarySection
/// <summary>
///struct data tag for player inputs
/// </summary>
/// <param name="PlayerInputData"></param>

#endregion
[GenerateAuthoringComponent]
public struct PlayerInputData : IComponentData
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode hyperJumpKey;
    public KeyCode SheidlKey;
    public KeyCode PauseMenuKey;
}