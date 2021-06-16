using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class AsteroidManager : MonoBehaviour
{
    private AsteroidManager refAsteroidManager;
    public static AsteroidManager asteroidManager;

    public GameObject[] asteroidsPrefabLarge;
    public GameObject[] asteroidsPrefabMedium;
    public GameObject[] asteroidsPrefabSmall;

    private Entity[] asteroidEntityLarge;
    private Entity[] asteroidEntityMedium;
    private Entity[] asteroidEntitySmall;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    public List<Entity> spawnedAsteroids;
    public bool spawnAsteroids;
    public float CurrentSpawnAmmount;
    public float MaxSpawnAmmount;
    public bool RemoveEnityQueue;
    public Entity EntityToRemove;

    private int MaxRandomSpawnValue;
    public Vector3 SpawnMax;
    public Vector3 SpawnMin;

    // Start is called before the first frame update
    private void Awake()
    {
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
            asteroidEntityMedium[AsteroidValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidsPrefabLarge[AsteroidValue], settings);
            asteroidEntitySmall[AsteroidValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidsPrefabLarge[AsteroidValue], settings);
        }
        spawnedAsteroids = new List<Entity>();
    }
    private void Start() 
    {
        refAsteroidManager = this;
        asteroidManager = refAsteroidManager;
    }
    // Update is called once per frame
    private void Update()
    {
        if (spawnAsteroids == true)
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
                spawnAsteroids = false;
            }
        }
        if (RemoveEnityQueue)
        {
            if (EntityToRemove != null)
            {
                spawnedAsteroids.Remove(EntityToRemove);
                entityManager.DestroyEntity(EntityToRemove);
                CurrentSpawnAmmount = CurrentSpawnAmmount - 1;
                spawnAsteroids = true;
                EntityToRemove = Entity.Null;
                RemoveEnityQueue = false;
            }
        }
    }
    private int ReturnNewRandom() 
    {
        int generatedRandom = UnityEngine.Random.Range(0, MaxRandomSpawnValue);
        return (generatedRandom);
    }
    public void DestroyAsteroid(Entity asteroidToDestroy) 
    {
        Debug.Log("Method");

        foreach (Entity entity in spawnedAsteroids)
        {
            Debug.Log("Called");
            if (entity == asteroidToDestroy)
            {
                Debug.Log("RemovingEntity");
                entityManager.DestroyEntity(entity);
            }
        }
        CurrentSpawnAmmount = CurrentSpawnAmmount - 1;
        spawnedAsteroids.Remove(asteroidToDestroy);
        spawnAsteroids = true;
    }
    private void OnApplicationQuit()
    {
        Debug.Log("Quit Called");
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
}
