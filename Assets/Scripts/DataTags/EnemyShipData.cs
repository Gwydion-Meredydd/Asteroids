using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

#region SummarySection
/// <summary>
///struct data tag to hold enemy ship data
/// </summary>
/// <param name="EnemyShipData"></param>

#endregion
[GenerateAuthoringComponent]
public struct EnemyShipData : IComponentData
{
    public bool SpeedInstantiated;

    public float MoveSpeed;
    public float RotateSpeed;
}
