using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

#region SummarySection
/// <summary>
/// system class thats respobbile for taking input data and converting into movement data for the ship move system to use
/// </summary>
/// <param name="ShipSystem"></param>

#endregion
public class ShipSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref ShipData shipData, ref Translation pos, in PlayerInputData playerInputData) =>
        {
            if (!GameManager.Instance.InMenu)
            {
                //takes the player keyboard input
                bool isRightKeyPressed = Input.GetKey(playerInputData.rightKey);
                bool isLeftKeyPressed = Input.GetKey(playerInputData.leftKey);
                bool isUpKeyPressed = Input.GetKey(playerInputData.upKey);
                bool isDownKeyPressed = Input.GetKey(playerInputData.downKey);
                bool isHPressed = Input.GetKeyDown(playerInputData.hyperJumpKey);
                bool isEscapeKeyPressed = Input.GetKeyDown(playerInputData.PauseMenuKey);

                //converts booleans to floats
                float horizontalValue = Convert.ToInt32(isRightKeyPressed);
                horizontalValue -= Convert.ToInt32(isLeftKeyPressed);
                float verticalValue = Convert.ToInt32(isUpKeyPressed);
                verticalValue -= Convert.ToInt32(isDownKeyPressed);
                if (ShipManager.Instance.CanHyperJump)
                {
                    shipData.Teleport = isHPressed;
                }
                if (isEscapeKeyPressed) 
                {
                    UserInterfaceManager.Instance.EscapeKeyPressed();
                }

                //sets ship data move veraibles
                shipData.activeForwardSpeed = Mathf.Lerp(shipData.activeForwardSpeed, verticalValue * ShipManager.Instance.currentShipSpeed, ShipManager.Instance.currentShipAcceleration * deltaTime);
                shipData.activeStrafeSpeed = Mathf.Lerp(shipData.activeStrafeSpeed, horizontalValue * ShipManager.Instance.currentShipStrafeSpeed, ShipManager.Instance.currentStrafeAcceleration * deltaTime);
                shipData.rollInput = Mathf.Lerp(shipData.rollInput, horizontalValue, ShipManager.Instance.currentRollAcceleration * deltaTime);

                //sets mouse rotation input in ship data
                shipData.lookInput.x = Input.mousePosition.x;
                shipData.lookInput.y = Input.mousePosition.y;
                shipData.mouseDistance.x = (shipData.lookInput.x - shipData.screenCenter.x) / shipData.screenCenter.y;
                shipData.mouseDistance.y = (shipData.lookInput.y - shipData.screenCenter.y) / shipData.screenCenter.y;

                //plays ship moving audio when ship moves
                if (!GameManager.Instance.Paused)
                {
                    if (isRightKeyPressed || isLeftKeyPressed || isUpKeyPressed || isDownKeyPressed ||
                     shipData.mouseDistance.x > 0.3f || shipData.mouseDistance.x < -0.3f
                     || shipData.mouseDistance.y > 0.2f || shipData.mouseDistance.y < -0.2f)
                    {

                        AudioManager.Instance.PlayPlayerMove();
                    }
                    else
                    {
                        AudioManager.Instance.StopPlayerMove();
                    }
                }
            }
        }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                //.run should work by it self)
    }
}
