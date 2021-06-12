using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class AsteroidManager : MonoBehaviour
{
    public GameObject[] asteroidsPrefabLarge;
    public GameObject[] asteroidsPrefabMedium;
    public GameObject[] asteroidsPrefabSmall;

    private Entity[] asteroidEntityLarge;
    private Entity[] asteroidEntityMedium;
    private Entity[] asteroidEntitySmall;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private List<Entity> spawnedAsteroids;
    public bool spawnAsteroids;
    public float CurrentSpawnAmmount;
    public float MaxSpawnAmmount;

    private int MaxRandomSpawnValue;

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

    // Update is called once per frame
    private void Update()
    {
        if (spawnAsteroids == true)
        {
            if (CurrentSpawnAmmount < MaxSpawnAmmount)
            {
                for (CurrentSpawnAmmount = 0; CurrentSpawnAmmount < MaxSpawnAmmount; CurrentSpawnAmmount++)
                {
                    Entity NewSpawnedEntity = entityManager.Instantiate(asteroidEntityLarge[ReturnNewRandom()]);
                    spawnedAsteroids.Add(NewSpawnedEntity);
                    var postion = new Vector3(UnityEngine.Random.Range(-1000f, 1000f), UnityEngine.Random.Range(-1000f, 1000f), UnityEngine.Random.Range(-1000f, 1000f));
                    entityManager.SetComponentData(NewSpawnedEntity, new Translation { Value = postion });
                }
            }
            else 
            {
                spawnAsteroids = false;
            }
        }
    }
    private int ReturnNewRandom() 
    {
        int generatedRandom = UnityEngine.Random.Range(0, MaxRandomSpawnValue);
        return (generatedRandom);
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
        spawnedAsteroids.Clear();
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }
}
