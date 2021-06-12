using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;


public class ShipManager : MonoBehaviour
{
    [Header("SpawnPrefabs")]
    public GameObject DefaultShipPrefab;
    public GameObject[] ShipUpgradeBranchPrefabs1;
    public GameObject[] ShipUpgradeBranchPrefabs2;
    public GameObject[] ShipUpgradeBranchPrefabs3;
    public GameObject[] ShipUpgradeBranchPrefabs4;

    private Entity DefaultShipEntity;
    private Entity[] ShipUpgradeBranchEntity1;
    private Entity[] ShipUpgradeBranchEntity2;
    private Entity[] ShipUpgradeBranchEntity3;
    private Entity[] ShipUpgradeBranchEntity4;

    private Entity CurrentShip;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private bool defaultShipSpawned;

    [Header("Ship Controlls")]
    public float forwardSpeed,strafeSpeed,hoverSpeed;
    public float forwardAcceleration, strafeAcceleration, hoverAcceleration;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;

    private float rollInput;
    public float rollSpeed, rollAcceleration;

    public float lookRateSpeed;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private Resolution currentRes;
    public Transform spawnedShipTransform;
    public CharacterController spawnedShipCc;
    private Translation spawnedShipTranslation;
    private Rotation spawnedShipRotation;

    private void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        ShipUpgradeBranchEntity1 = new Entity[ShipUpgradeBranchPrefabs1.Length];
        ShipUpgradeBranchEntity2 = new Entity[ShipUpgradeBranchPrefabs2.Length];
        ShipUpgradeBranchEntity3 = new Entity[ShipUpgradeBranchPrefabs3.Length];
        ShipUpgradeBranchEntity4 = new Entity[ShipUpgradeBranchPrefabs4.Length];
        for (int ShipValue = 0; ShipValue < ShipUpgradeBranchPrefabs1.Length; ShipValue++)
        {
            ShipUpgradeBranchEntity1[ShipValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(ShipUpgradeBranchPrefabs1[ShipValue], settings);
        }
        for (int ShipValue = 0; ShipValue < ShipUpgradeBranchPrefabs2.Length; ShipValue++)
        {
            ShipUpgradeBranchEntity2[ShipValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(ShipUpgradeBranchPrefabs2[ShipValue], settings);
        }
        for (int ShipValue = 0; ShipValue < ShipUpgradeBranchPrefabs3.Length; ShipValue++)
        {
            ShipUpgradeBranchEntity3[ShipValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(ShipUpgradeBranchPrefabs3[ShipValue], settings);
        }
        for (int ShipValue = 0; ShipValue < ShipUpgradeBranchPrefabs4.Length; ShipValue++)
        {
            ShipUpgradeBranchEntity4[ShipValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(ShipUpgradeBranchPrefabs4[ShipValue], settings);
        }
        DefaultShipEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(DefaultShipPrefab, settings);

        screenCenter.x = Screen.width * .5f;
        screenCenter.y = Screen.height * .5f;
        currentRes = Screen.currentResolution;

    }

    private void Update()
    {
        if (GameManager._gameManager.InGame) 
        {
            if (blobAssetStore != null)
            {
                if (!defaultShipSpawned)
                {
                    CurrentShip = entityManager.Instantiate(DefaultShipEntity);
                    spawnedShipTranslation = entityManager.GetComponentData<Translation>(CurrentShip);
                    spawnedShipRotation = entityManager.GetComponentData<Rotation>(CurrentShip);

                    spawnedShipTransform.position = spawnedShipTranslation.Value;
                    spawnedShipTransform.rotation = spawnedShipRotation.Value;

                    defaultShipSpawned = true;
                }
                else
                {
                    activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
                    activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
                    activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);
                    rollInput = Mathf.Lerp(rollInput, -Input.GetAxisRaw("Horizontal"), rollAcceleration * Time.deltaTime);

                    spawnedShipCc.Move(spawnedShipTransform.forward * activeForwardSpeed * Time.deltaTime);
                    spawnedShipCc.Move((spawnedShipTransform.right * activeStrafeSpeed * Time.deltaTime)
                        + (transform.up * activeHoverSpeed * Time.deltaTime));
                    lookInput.x = Input.mousePosition.x;
                    lookInput.y = Input.mousePosition.y;

                    mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
                    mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

                    mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
                    spawnedShipTransform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

                    spawnedShipTranslation.Value = spawnedShipTransform.position;
                    spawnedShipRotation.Value = spawnedShipTransform.rotation;
                    entityManager.SetComponentData(CurrentShip, spawnedShipTranslation);
                    entityManager.SetComponentData(CurrentShip, spawnedShipRotation);

                }
            }
        }
    }
    private void LateUpdate()
    {
        if (GameManager._gameManager.InGame)
        {
            if (defaultShipSpawned)
            {
                if (CurrentShip == null)
                {
                    return;
                }
                if(currentRes.width != Screen.currentResolution.width || currentRes.height != Screen.currentResolution.height)
                {
                    screenCenter.x = Screen.width * .5f;
                    screenCenter.y = Screen.height * .5f;
                    currentRes = Screen.currentResolution;
                }
            }
        }
    }

    private void SpawnShip() 
    {
        
    }



    private void OnApplicationQuit()
    {
        foreach (Entity ShipU1 in ShipUpgradeBranchEntity1)
        {
            entityManager.DestroyEntity(ShipU1);
        }
        foreach (Entity ShipU2 in ShipUpgradeBranchEntity2)
        {
            entityManager.DestroyEntity(ShipU2);
        }
        foreach (Entity ShipU3 in ShipUpgradeBranchEntity3)
        {
            entityManager.DestroyEntity(ShipU3);
        }
        foreach (Entity ShipU4 in ShipUpgradeBranchEntity4)
        {
            entityManager.DestroyEntity(ShipU4);
        }
        ShipUpgradeBranchEntity1 = new Entity[0];
        ShipUpgradeBranchEntity2 = new Entity[0];
        ShipUpgradeBranchEntity3 = new Entity[0];
        ShipUpgradeBranchEntity4 = new Entity[0];
        entityManager.DestroyEntity(DefaultShipEntity);
        entityManager.DestroyEntity(CurrentShip);
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }
}
