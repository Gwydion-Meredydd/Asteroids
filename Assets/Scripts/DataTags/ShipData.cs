using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShipData : IComponentData
{
    public float3 Direction;
    public float forwardSpeed, strafeSpeed, hoverSpeed;
    public float forwardAcceleration, strafeAcceleration;
    public float activeForwardSpeed, activeStrafeSpeed;

    public float rollInput;
    public float rollSpeed, rollAcceleration;

    public float lookRateSpeed;
    public Vector2 lookInput, screenCenter, mouseDistance;
    public Resolution currentRes;
}
