using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
/// Singleton class that is used to convert the ship prefabs to entiteis and spawn them in run time
///it also is responsible for the camera tracking, laser effects , ship upgrade spawning and shoot cooldown
///  </summary>
/// <param name="ShipManager"></param>

#endregion
public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }


    [Header("Ship Prefabs")]
    public GameObject DefaultShipPrefab;
    public GameObject[] ShipUpgradeBranchPrefabs1;
    public GameObject[] ShipUpgradeBranchPrefabs2;
    public GameObject[] ShipUpgradeBranchPrefabs3;
    public GameObject[] ShipUpgradeBranchPrefabs4;

    private Entity DefaultShipEntity;
    public Entity[] ShipUpgradeBranchEntity1;
    public Entity[] ShipUpgradeBranchEntity2;
    public Entity[] ShipUpgradeBranchEntity3;
    public Entity[] ShipUpgradeBranchEntity4;
    public Entity ShipUpgrade;
    public Entity CurrentShip;
    public EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private bool defaultShipSpawned;

    [Header("Control Options")]
    public float currentShipSpeed;
    public float currentShipStrafeSpeed;
    public float currentShipAcceleration, currentStrafeAcceleration, currentRollAcceleration;
    public bool CanHyperJump;
    public int Health;
    public bool SheidlActive;
    public bool ShieldProtection;
    public float MaxShieldTime;
    public float CurrentShieldTime;


    [Header("Misc Items")]
    public bool ShipDied;
    public bool CanAsteroidDamage;
    public GameObject ShipLazerPrefab;
    public GameObject SparksPrefab;
    public GameObject ExplosionPrefab;
    public GameObject ShieldPrefab;
    public ShipData shipdata;
    private Vector2 screenCenter;
    public List<ParticleSystem> ShipLazers;
    public List<ParticleSystem> ShipSparks;
    public ParticleSystem ShipExplosion;
    public ParticleSystem ShipShield;
    private Resolution currentRes;
    public Entity HitObject;
    public float ShootSpeed;
    public bool CanShoot;
    public bool isMoving;

    [Header("Camera Tracking")]
    public Transform ShipCamera;
    public float Offset;


    //converts prefabs to entitiys ready to be instansiated into the game in run time
    //also houses requied initliasiation (Unity dots requirments) 
    private void Awake()
    {
        Instance = this;
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
    private void Update()
    {
        if (GameManager.Instance.InGame)
        {
            if (blobAssetStore != null)
            {
                if (!defaultShipSpawned)
                {
                    //spawns the default ship and grabs its ship data componenet
                    CurrentShip = entityManager.Instantiate(DefaultShipEntity);
                    shipdata = entityManager.GetComponentData<ShipData>(CurrentShip);
                    defaultShipSpawned = true;
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (GameManager.Instance.InGame)
        {
            if (!GameManager.Instance.InMenu)
            {
                if (defaultShipSpawned)
                {
                    //checks if their is a ship needed to prevent bug in menu updates
                    if (CurrentShip == null)
                    {
                        return;
                    }

                    //moves the ship camera gamobject to the ship camera entity
                    ShipCamera.position = (entityManager.GetComponentData<Translation>(CurrentShip).Value) + Offset;
                    ShipCamera.rotation = (entityManager.GetComponentData<Rotation>(CurrentShip).Value);
                    //checks if the game screen size has changed to allow for correct mouse ship movment
                    if (currentRes.width != Screen.width || currentRes.height != Screen.height)
                    {
                        screenCenter.x = Screen.width * .5f;
                        screenCenter.y = Screen.height * .5f;
                        currentRes.width = Screen.width;
                        currentRes.height = Screen.height;
                        shipdata = entityManager.GetComponentData<ShipData>(CurrentShip);
                        shipdata.currentRes = currentRes;
                        shipdata.screenCenter = screenCenter;
                        entityManager.SetComponentData<ShipData>(CurrentShip, shipdata);
                        EdgeDetect.Instance.cam = null;
                        EdgeDetect.Instance.width = -1;
                    }
                    //sets health icons to correct health
                    UserInterfaceManager.Instance.UpdateHealth();
                }
            }
        }
    }
    //takes the old shipdata and C+P it to the new one (has to be this way due to bug where ship would teleport out of map)
    public void UpgradeShip()
    {
        Translation OldShipPos = entityManager.GetComponentData<Translation>(CurrentShip);
        Rotation OldShipRot = entityManager.GetComponentData<Rotation>(CurrentShip);
        ShipData OldshipData = entityManager.GetComponentData<ShipData>(CurrentShip);
        entityManager.DestroyEntity(CurrentShip);
        CurrentShip = entityManager.Instantiate(ShipUpgrade);
        shipdata = entityManager.GetComponentData<ShipData>(CurrentShip);
        entityManager.SetComponentData<ShipData>(CurrentShip, OldshipData);
        entityManager.SetComponentData<Translation>(CurrentShip, OldShipPos);
        entityManager.SetComponentData<Rotation>(CurrentShip, OldShipRot);
        ShipUpgrade = Entity.Null;
    }

    //called from shoot ship system to instantiate shootpoints and laser particle systems
    public void ShootParticleSystemInstantiated(float3 Pos)
    {
        var TempShootVar = GameObject.Instantiate(ShipLazerPrefab, Pos, new Quaternion(0, 0, 0, 0));
        TempShootVar.transform.parent = ShipCamera;
        TempShootVar.transform.rotation = new Quaternion(0, 0, 0, 0);
        TempShootVar.transform.localPosition = Pos;
        ShipLazers.Add(TempShootVar.GetComponent<ParticleSystem>());
    }

    //called from shoot ship system to instantiate shootpoints and laser particle systems
    public void SparkParticleSystemInstantiated(float3 Pos)
    {
        var TempSparkVar = GameObject.Instantiate(SparksPrefab, Pos, new Quaternion(0, 0, 0, 0));
        TempSparkVar.transform.parent = ShipCamera;
        TempSparkVar.transform.rotation = new Quaternion(0, 0, 0, 0);
        TempSparkVar.transform.localPosition = Pos;
        ShipSparks.Add(TempSparkVar.GetComponent<ParticleSystem>());
    }

    //called from shoot ship system to instantiate shootpoints and laser particle systems
    public void ExplosionandShieldParticleSystemInstantiated(float3 Pos)
    {
        var TempExplosionVar = GameObject.Instantiate(ExplosionPrefab, Pos, new Quaternion(0, 0, 0, 0));
        TempExplosionVar.transform.parent = ShipCamera;
        TempExplosionVar.transform.rotation = new Quaternion(0, 0, 0, 0);
        TempExplosionVar.transform.localPosition = Pos;
        ShipExplosion = TempExplosionVar.GetComponent<ParticleSystem>();

        var TempShieldVar = GameObject.Instantiate(ShieldPrefab, Pos, new Quaternion(0, 0, 0, 0));
        TempShieldVar.transform.parent = ShipCamera;
        TempShieldVar.transform.rotation = new Quaternion(0, 0, 0, 0);
        TempShieldVar.transform.localPosition = Pos;
        ShipShield = TempShieldVar.GetComponent<ParticleSystem>();
    }

    //Shoots Lazer Particles Only if there is lazer particles to be played
    public void ShootParticles()
    {
        if (ShipLazers.Count == 0) { return; }
        foreach (ParticleSystem Laser in ShipLazers)
        {
            Laser.Play();
        }
    }

    //called from shootship system (allows control over ship shoot speed)
    IEnumerator ShootCoolDown()
    {
        CanShoot = false;
        yield return new WaitForSeconds(ShootSpeed);
        CanShoot = true;
    }

    #region Destroys Player Ship and calls eneable death ui 
    public void DestroyShip()
    {
        ShipDied = true;
        StartCoroutine(DestoryShipTiming());
    }
    IEnumerator DestoryShipTiming()
    {
        ShipExplosion.Play();
        AudioManager.Instance.PlayPlayerExplsoion();
        yield return new WaitForSeconds(0.5f);
        ClearMemeory();
        GameManager.Instance.InMenu = true;
        UserInterfaceManager.Instance.EnableDeathUI();
        ShipSparks[0].Stop();
        ShipSparks[1].Stop();
        Cursor.visible = true;
    }
    #endregion

    #region Sheild methods called from shipsystem
    public void EnableShield()
    {
        if (CurrentShieldTime > 5)
        {
            ShipShield.Play();
            StartCoroutine(ShieldTimingSubtact());
            SheidlActive = true;
        }
    }

    IEnumerator ShieldTimingSubtact()
    {
        ShieldProtection = true;
        AudioManager.Instance.PlayShieldSfx();
        CurrentShieldTime = CurrentShieldTime - 0.1f;
        UserInterfaceManager.Instance.UpdateShieldTime();
        yield return new WaitForSeconds(0.1f);
        if (CurrentShieldTime > 0 && SheidlActive)
        {
            StartCoroutine(ShieldTimingSubtact());
        }
        else
        {
            ShieldProtection = false;
            AudioManager.Instance.StopShieldSfx();
            ShipShield.Stop();
            UserInterfaceManager.Instance.UpdateShieldTime();
        }
        if (CurrentShieldTime <= 0 && SheidlActive)
        {
            AudioManager.Instance.StopShieldSfx();
            CurrentShieldTime = 0;
        }
    }
    public void DisableShield()
    {
        if (ShipShield != null)
        {
            SheidlActive = false;
            ShipShield.Stop();
            AudioManager.Instance.StopShieldSfx();
            StartCoroutine(ShieldTimingAdd());
        }
    }
    IEnumerator ShieldTimingAdd()
    {
        ShieldProtection = false;
        CurrentShieldTime = CurrentShieldTime + 0.1f;
        UserInterfaceManager.Instance.UpdateShieldTime();
        yield return new WaitForSeconds(0.1f);
        if (CurrentShieldTime < MaxShieldTime && !SheidlActive)
        {
            StartCoroutine(ShieldTimingAdd());
        }
    }
    #endregion

    #region Cooldown for asteroid Damage
    public void AsteroidCoolDown()
    {
        StartCoroutine(AsteroidCoolDownTiming());
    }
    IEnumerator AsteroidCoolDownTiming() 
    {
        yield return new WaitForSeconds(5f);
        CanAsteroidDamage = true;
    }
    #endregion

    //used to clear memeory on close VERY IMPORTANT!!!!!!!!!
    public void ClearMemeory() 
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
        entityManager.CompleteAllJobs();
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }
    private void OnApplicationQuit()
    {
        ClearMemeory();
    }
}
