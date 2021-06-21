using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

#region SummarySection
/// <summary>
///struct data tag to hold ship data
/// </summary>
/// <param name="ShipData"></param>

#endregion
[GenerateAuthoringComponent]
public struct ShipData : IComponentData
{
    public float3 Direction;
    public bool Teleport;
    public float hoverSpeed;
    public float activeForwardSpeed, activeStrafeSpeed;

    public float rollInput;
    public float rollSpeed, rollAcceleration;

    public float lookRateSpeed;
    public Vector2 lookInput, screenCenter, mouseDistance;
    public Resolution currentRes;
}
