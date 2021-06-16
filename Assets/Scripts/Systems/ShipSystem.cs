using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class ShipSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref ShipData shipData,ref Translation pos, in PlayerInputData playerInputData) =>
        {

            bool isRightKeyPressed = Input.GetKey(playerInputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(playerInputData.leftKey);
            bool isUpKeyPressed = Input.GetKey(playerInputData.upKey);
            bool isDownKeyPressed = Input.GetKey(playerInputData.downKey);

            float  horizontalValue = Convert.ToInt32(isRightKeyPressed);
            horizontalValue -= Convert.ToInt32(isLeftKeyPressed);
            float verticalValue = Convert.ToInt32(isUpKeyPressed);
            verticalValue -= Convert.ToInt32(isDownKeyPressed);


            shipData.activeForwardSpeed = Mathf.Lerp(shipData.activeForwardSpeed, verticalValue * shipData.forwardSpeed, shipData.forwardAcceleration * deltaTime);
            shipData.activeStrafeSpeed = Mathf.Lerp(shipData.activeStrafeSpeed, horizontalValue * shipData.strafeSpeed, shipData.strafeAcceleration * deltaTime);
            shipData.rollInput = Mathf.Lerp(shipData.rollInput, horizontalValue, shipData.rollAcceleration * deltaTime);

           // if (shipData.currentRes.width != Screen.currentResolution.width || shipData.currentRes.height != Screen.currentResolution.height)
            //{
                //shipData.screenCenter.x = Screen.width * .5f;
                //shipData.screenCenter.y = Screen.height * .5f;
                //shipData.currentRes = Screen.currentResolution;
           // }

            shipData.lookInput.x = Input.mousePosition.x;
            shipData.lookInput.y = Input.mousePosition.y;
            shipData.mouseDistance.x = (shipData.lookInput.x - shipData.screenCenter.x) / shipData.screenCenter.y;
            shipData.mouseDistance.y = (shipData.lookInput.y - shipData.screenCenter.y) / shipData.screenCenter.y;

            //Debug.Log(shipData.lookInput);
            //shipData.Move(new Vector3(0,0,1) * shipData.activeForwardSpeed * deltaTime);
            //shipData.ShipcharacterController.Move(new Vector3(1, 0, 0) * shipData.activeStrafeSpeed * deltaTime);

            // shipData.ShipTransform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

        }).Run();
    }
}
