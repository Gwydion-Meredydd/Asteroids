using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

#region SummarySection
/// <summary>
/// Singleton class that is used to house the entetis of the asteroids that where converted from prefabs also controlls the spawning of the asteroids (on start, and on destroy)
///  </summary>
/// <param name="AsteroidManager"></param>

#endregion
public class AsteroidManager : MonoBehaviour
{ 
    public static AsteroidManager Instance { get; private set; }

    [Header("Asteroid Prefabs")]
    public GameObject[] asteroidsPrefabLarge;
    public GameObject[] asteroidsPrefabMedium;
    public GameObject[] asteroidsPrefabSmall;

    private Entity[] asteroidEntityLarge;
    private Entity[] asteroidEntityMedium;
    private Entity[] asteroidEntitySmall;

    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    public List<Entity> spawnedAsteroids;
    [Header("Spawn Controllers")]
    public bool spawnLargeAsteroids;
    public bool spawnMediumAsteroid;
    public bool RemoveEnityQueue;
    public bool spawnSmallAsteroid;
    [Space]
    public float CurrentSpawnAmmount;
    public float spawnOverHead;
    public float CurrentOverHead;
    public float MaxSpawnAmmount;
    [HideInInspector]
    public float3 DestoryedEntityPos;
    [HideInInspector]
    public Vector3 mediumSpawnPos;
    [HideInInspector]
    public Vector3 smallSpawnPos;
    public Entity EntityToRemove;

    private int MaxRandomSpawnValue;
    [Space]
    [Header("Spawn Size")]
    public Vector3 SpawnMax;
    public Vector3 SpawnMin;


    //converts prefabs to entitiys ready to be instansiated into the game in run time
    //also houses requied initliasiation (Unity dots requirments) 
    private void Awake()
    {
        Instance = this;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        MaxRandomSpawnValue = asteroidsPrefabLarge.Length;
        asteroidEntityLarge = new Entity[asteroidsPrefabLarge.Length];
        asteroidEntityMedium = new Entity[asteroidsPrefabLarge.Length];
        asteroidEntitySmall = new Entity[asteroidsPrefabLarge.Length];
        for (int AsteroidValue = 0; AsteroidValue < asteroidsPrefabLarge.Length; AsteroidValue++)
        {
            asteroidEntityLarge[AsteroidValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidsPrefabLarge[AsteroidValue], settings);
            asteroidEntityMedium[AsteroidValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidsPrefabMedium[AsteroidValue], settings);
            asteroidEntitySmall[AsteroidValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidsPrefabSmall[AsteroidValue], settings);
        }
        spawnedAsteroids = new List<Entity>();
    }
    private void Update()
    {
        if (spawnLargeAsteroids == true)
        {
            if (CurrentSpawnAmmount < MaxSpawnAmmount)
            {
                float OldSpawnAmmount = CurrentSpawnAmmount;
                for (CurrentSpawnAmmount = OldSpawnAmmount; CurrentSpawnAmmount < MaxSpawnAmmount; CurrentSpawnAmmount++)
                {
                    Entity NewSpawnedEntity = entityManager.Instantiate(asteroidEntityLarge[ReturnNewRandom()]);
                    spawnedAsteroids.Add(NewSpawnedEntity);
                    var postion = new Vector3(UnityEngine.Random.Range(SpawnMin.x, SpawnMax.x), UnityEngine.Random.Range(SpawnMin.y, SpawnMax.y), UnityEngine.Random.Range(SpawnMin.z, SpawnMax.z));
                    entityManager.SetComponentData(NewSpawnedEntity, new Translation { Value = postion });
                }
            }
            else 
            {
                spawnLargeAsteroids = false;
            }
        }
        if (CurrentOverHead < spawnOverHead)
        {
            if (spawnMediumAsteroid)
            {

                CurrentOverHead = CurrentOverHead + 1;
                Entity NewSpawnedEntity = entityManager.Instantiate(asteroidEntityMedium[ReturnNewRandom()]);
                spawnedAsteroids.Add(NewSpawnedEntity);
                var postion = mediumSpawnPos;
                entityManager.SetComponentData(NewSpawnedEntity, new Translation { Value = postion });
                spawnMediumAsteroid = false;
            }
            if (spawnSmallAsteroid)
            {
                CurrentOverHead = CurrentOverHead + 1;
                Entity NewSpawnedEntity = entityManager.Instantiate(asteroidEntitySmall[ReturnNewRandom()]);
                spawnedAsteroids.Add(NewSpawnedEntity);
                var postion = smallSpawnPos;
                entityManager.SetComponentData(NewSpawnedEntity, new Translation { Value = postion });
                spawnSmallAsteroid = false;
            }
        }
        //removes an asteroid entity
        if (RemoveEnityQueue)
        {
            if (EntityToRemove != null)
            {
                spawnedAsteroids.Remove(EntityToRemove);
                entityManager.DestroyEntity(EntityToRemove);

                //Overhead system allows for extra small and medium asteroids to spawn when large asteroid spawn cap is met
                if (CurrentOverHead == 0)
                {
                    CurrentSpawnAmmount = CurrentSpawnAmmount - 1;
                }
                else
                {
                    CurrentOverHead = CurrentOverHead - 1;
                }
                spawnLargeAsteroids = true;
                EntityToRemove = Entity.Null;
                RemoveEnityQueue = false;

                //uses the upgrade manager class singleton to randomly choose if the destroyed asteroid should drop a upgrade box
                float SelectRandomValue = UnityEngine.Random.Range(0, UpgradeManager.Instance.RandomDropChance);
                if (!UpgradeManager.Instance.MaxUpgradeReached)
                {
                    if (SelectRandomValue == UpgradeManager.Instance.RandomDropChance / 2)
                    {
                        UpgradeManager.Instance.SpawnNewDrop = true;
                        UpgradeManager.Instance.SpawnNewDropPos = DestoryedEntityPos;
                        DestoryedEntityPos = float3.zero;
                    }
                }
            }
        }
    }
    private int ReturnNewRandom() 
    {
        int generatedRandom = UnityEngine.Random.Range(0, MaxRandomSpawnValue);
        return (generatedRandom);
    }

    //used to clear memeory on close VERY IMPORTANT!!!!!!!!!
    public void ClearMemory() 
    {
        foreach (Entity entity in spawnedAsteroids)
        {
            entityManager.DestroyEntity(entity);
        }
        foreach (Entity entLarge in asteroidEntityLarge)
        {
            entityManager.DestroyEntity(entLarge);
        }
        foreach (Entity entMedium in asteroidEntityMedium)
        {
            entityManager.DestroyEntity(entMedium);
        }
        foreach (Entity entSmall in asteroidEntitySmall)
        {
            entityManager.DestroyEntity(entSmall);
        }
        asteroidEntityLarge = new Entity[0];
        asteroidEntityMedium = new Entity[0];
        asteroidEntitySmall = new Entity[0];
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }

    private void OnApplicationQuit()
    {
        ClearMemory();
    }
}
