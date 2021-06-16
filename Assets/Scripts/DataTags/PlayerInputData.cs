using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerInputData : IComponentData
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode hyperJumpKey;
}