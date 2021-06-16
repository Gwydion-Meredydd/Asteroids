using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;


public class ShipManager : MonoBehaviour
{
    public static ShipManager shipManager;
    private ShipManager refShipManger; 


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

    public Entity CurrentShip;
    public EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private bool defaultShipSpawned;


    public ShipData shipdata;
    private Vector2 screenCenter;
    public List <ParticleSystem> ShipLazers;
    public GameObject ShipLazerPrefab;
    private Resolution currentRes;
    public Entity HitObject;

    [Header("Camera Tracking")]
    public Transform ShipCamera;
    public float Offset;

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
    }
    private void Start()
    {
        refShipManger = this;
        shipManager = refShipManger;
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
         
                    shipdata = entityManager.GetComponentData<ShipData>(CurrentShip);
                    defaultShipSpawned = true;
                }
                else
                {
                    /*
                    activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
                    activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
                    rollInput = Mathf.Lerp(rollInput, -Input.GetAxisRaw("Horizontal"), rollAcceleration * Time.deltaTime);

                    spawnedShipCc.Move(spawnedShipTransform.forward * activeForwardSpeed * Time.deltaTime);
                    spawnedShipCc.Move(spawnedShipTransform.right * activeStrafeSpeed * Time.deltaTime);
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
                    */


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
                ShipCamera.position = (entityManager.GetComponentData<Translation>(CurrentShip).Value) + Offset;

                ShipCamera.rotation = (entityManager.GetComponentData<Rotation>(CurrentShip).Value);
                if (currentRes.width != Screen.currentResolution.width || currentRes.height != Screen.currentResolution.height)
                {
                    screenCenter.x = Screen.width * .5f;
                    screenCenter.y = Screen.height * .5f;
                    currentRes = Screen.currentResolution;

                    shipdata.currentRes = currentRes;
                    shipdata.screenCenter = screenCenter;
                    entityManager.SetComponentData<ShipData>(CurrentShip, shipdata);
                }
            }
        }
    }

    private void SpawnShip()
    {
        
    }
    public void ShootParticleSystemInstantiated(float3 Pos) 
    {
        var TempShootVar = GameObject.Instantiate(ShipLazerPrefab, Pos, new Quaternion(0,0,0,0));
        TempShootVar.transform.parent = ShipCamera;
        TempShootVar.transform.rotation = new Quaternion(0, 0, 0, 0);
        TempShootVar.transform.localPosition = Pos;
        ShipLazers.Add(TempShootVar.GetComponent<ParticleSystem>());
    }
    public void ShootParticles() 
    {
        if (ShipLazers.Count == 0) { return; }
        foreach (ParticleSystem Laser in ShipLazers)
        {
            Debug.Log("Firing Lazers");
            Laser.Play();
        }
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
