using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

#region SummarySection
/// <summary>
/// Singleton class that is used to convert the enemy ship prefabs to entiteis and spawn them in run time using spawning mechanics
///  </summary>
/// <param name="EnemyManager"></param>

#endregion
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject EnemyShipPrefab;
    public Entity EnemyShipEntity;

    [Tooltip("Enemy will spawn when the score is a multiple of this number")]
    public int ScoreSpawnMultiple;
    [Space]
    public GameObject ShipLazerPrefab;
    public GameObject ShipExplosionPrefab;
    public int MaxSpawnAmmout;
    public int CurrentAmmountSpawned;
    public int CurrentSpawnRate;
    public int MaxSpawnRate;
    private int OldAmmountSpawned;
    public List<EnemyShip> EnemyShipClass;
    public List<Entity> SpawnedEnemies;
    public EntityManager entityManager;
    private BlobAssetStore blobAssetStore;
    [Space]
    public bool EntityToDestory;
    public Entity ShipToDestroy;
    public bool UpdateShipVeriables;
    public int HitChange;
    public float StoppingDistance;
    public float MoveSpeed;
    public float RotationSpeed;
    public float ShootCoolDown;
    public void Awake()
    {
        Instance = this;
        SpawnedEnemies = new List<Entity>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        EnemyShipEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(EnemyShipPrefab, settings);
    }

    public void Update()
    {
        if (GameManager.Instance.InGame) 
        {
            if (!GameManager.Instance.Paused) 
            {
                if (ScoreManger.Instance.currentScore > 0)
                {
                    if (ScoreManger.Instance.currentScore % ScoreSpawnMultiple == 0)
                    {
                        if (CurrentSpawnRate < MaxSpawnRate)
                        {
                            //sets new score spawn multiple based on the score
                            CurrentSpawnRate = ScoreManger.Instance.currentScore / (ScoreSpawnMultiple * 2);
                            if (CurrentSpawnRate > MaxSpawnRate) 
                            {
                                CurrentSpawnRate = MaxSpawnRate;
                            }
                        }
                        if (CurrentAmmountSpawned < MaxSpawnAmmout)
                        {
                            //spawns an enemy when the players score is a multiple of ScoreSpawnMultiple
                            //and if the current spawn ammount is less than the max spawn ammount
                            //sets the enemy pos to a random pos in game world
                            ScoreManger.Instance.currentScore = ScoreManger.Instance.currentScore + 1;
                            UserInterfaceManager.Instance.UpdateScore();
                            for (int i = 0; i < CurrentSpawnRate; i++)
                            {
                                if (CurrentAmmountSpawned < MaxSpawnAmmout)
                                {
                                    CurrentAmmountSpawned += 1;
                                    Entity NewSpawnedEntity = entityManager.Instantiate(EnemyShipEntity);
                                    var postion = new Vector3(UnityEngine.Random.Range(AsteroidManager.Instance.SpawnMin.x, AsteroidManager.Instance.SpawnMax.x), UnityEngine.Random.Range(AsteroidManager.Instance.SpawnMin.y, AsteroidManager.Instance.SpawnMax.y), UnityEngine.Random.Range(AsteroidManager.Instance.SpawnMin.z, AsteroidManager.Instance.SpawnMax.z));
                                    entityManager.SetComponentData(NewSpawnedEntity, new Translation { Value = postion });
                                    SpawnedEnemies.Add(NewSpawnedEntity);
                                }
                            }
                        }
                    }
                }
                //called if the player upgrades ship making the enemie ships harder
                if (UpdateShipVeriables) 
                {
                    foreach (Entity Ships in SpawnedEnemies)
                    {
                        EnemyShipData enemyShipData = new EnemyShipData();
                        float MoveSpeed = UnityEngine.Random.Range(EnemyManager.Instance.MoveSpeed - 5, EnemyManager.Instance.MoveSpeed);
                        float RotateSpeed = UnityEngine.Random.Range(EnemyManager.Instance.RotationSpeed - 5, EnemyManager.Instance.RotationSpeed);
                        enemyShipData.MoveSpeed = MoveSpeed;
                        enemyShipData.RotateSpeed = RotateSpeed;
                        enemyShipData.SpeedInstantiated = true;
                        entityManager.SetComponentData(Ships, enemyShipData);
                    }
                    UpdateShipVeriables = false;
                }

                //set enemy ship refrence (for particles) to the correct pos
                if (EnemyShipClass.Count > 0) 
                {
                    foreach (EnemyShip enemyShip in EnemyShipClass)
                    {
                        enemyShip.ShipGO.transform.position = enemyShip.Pos;
                        enemyShip.ShipGO.transform.rotation = enemyShip.Rot;
                    }
                }

                //destroys the ship thats set to shiptodestroy
                //adds score
                if (EntityToDestory) 
                {
                    EntityToDestory = false;
                    CurrentAmmountSpawned = CurrentAmmountSpawned - 1;
    
                    SpawnedEnemies.Remove(ShipToDestroy);
                    entityManager.DestroyEntity(ShipToDestroy);
                    Debug.Log("DESTORY ENEMY done");
                    ScoreManger.Instance.currentScore = ScoreManger.Instance.currentScore + ScoreManger.Instance.enemyShipScore;
                }
            }
        }
    }

    #region destroys ship and removes refrence from veriables and enables entity to destory bool
    public void DestroyShip() 
    {
        EntityToDestory = true;
        for (int i = 0; i < EnemyShipClass.Count; i++)
        {
            if (SpawnedEnemies[i] == ShipToDestroy)
            {
                AudioManager.Instance.PlayPlayerExplsoion();
                EnemyShipClass[i].Explosion.Play();
                StartCoroutine(DestroyShipTiming(i));
                break;
            }
        }
    }
    IEnumerator DestroyShipTiming(int i) 
    {
        AudioManager.Instance.PlayPlayerExplsoion();
        yield return new WaitForSeconds(0.15f);
        Destroy(EnemyShipClass[i].gameObject);
        EnemyShipClass.RemoveAt(i);
        ShipToDestroy = Entity.Null;
    }
    #endregion 

    //creates laser prefab under refrence ship object to allow ships to shoot lasers 
    public void ShootParticleSystemInstantiated(float3 Pos)
    {
        var TempShootVar = GameObject.Instantiate(ShipLazerPrefab, Pos, new Quaternion(0, 0, 0, 0));        
        foreach (EnemyShip enemyShip in EnemyShipClass)
        {
            if (enemyShip.Laser == null) 
            {
                enemyShip.Laser = (TempShootVar.GetComponent<ParticleSystem>());
                TempShootVar.transform.parent = enemyShip.transform;
                TempShootVar.transform.rotation = new Quaternion(0, 0, 0, 0);
                TempShootVar.transform.localPosition = Pos;
                break;
            }
        }
    }

    //creates explosion prefab under refrence ship object to allow ships to explode 
    public void ExplosionParticleSystemInstantiated(float3 Pos) 
    {
        var TempExplosionVar = GameObject.Instantiate(ShipExplosionPrefab, Pos, new Quaternion(0, 0, 0, 0));
        foreach (EnemyShip enemyShip in EnemyShipClass)
        {
            if (enemyShip.Explosion == null)
            {
                enemyShip.Explosion = (TempExplosionVar.GetComponent<ParticleSystem>());
                TempExplosionVar.transform.parent = enemyShip.transform;
                TempExplosionVar.transform.rotation = new Quaternion(0, 0, 0, 0);
                TempExplosionVar.transform.localPosition = Pos;
                break;
            }
        }
    }
    
    //creates refrence go for particles to be spawned under
    public void SpawnRefrenceGO(float3 Pos, Quaternion rot, Entity ent) 
    {
        GameObject NewGO = new GameObject();
        EnemyShip TempEnemyShip = NewGO.AddComponent<EnemyShip>();
        TempEnemyShip.ShipGO = NewGO;
        EnemyShipClass.Add(TempEnemyShip.GetComponent<EnemyShip>());
        TempEnemyShip.ShipEnt = ent;
        TempEnemyShip.Rot = rot;
        TempEnemyShip.Pos = Pos;
    }

    //sets enemy ship class go to the correct locaiton
    public void SetNewPosandRot(float3 pos, quaternion rot, Entity ent) 
    {
        foreach (EnemyShip enemyShip in EnemyShipClass)
        {
            if (enemyShip.ShipEnt == ent) 
            {
                enemyShip.Pos = pos;
                enemyShip.Rot = rot;
            }
        }
    }

    //shoots ship lasers 
    public void ShootParticleLaser(Entity ent) 
    {
        foreach (EnemyShip enemyShip in EnemyShipClass) 
        {
            enemyShip.Laser.Play();
        }
    }

    //used to clear memeory on close VERY IMPORTANT!!!!!!!!!
    public void ClearMemeory()
    {
        foreach (Entity EnemyShip in SpawnedEnemies)
        {
            entityManager.DestroyEntity(EnemyShip);
        }
        SpawnedEnemies = new List<Entity>();
        entityManager.DestroyEntity(EnemyShipEntity);
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

//custom class used to use particle systems with entity using a gameobject to refrence the entitiys position and rotation
[System.Serializable]
public class EnemyShip:MonoBehaviour
{
    public GameObject ShipGO;
    public Vector3 Pos;
    public quaternion Rot;
    public Entity ShipEnt;
    public ParticleSystem Laser;
    public ParticleSystem Explosion;
}
