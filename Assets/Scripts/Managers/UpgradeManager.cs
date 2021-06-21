using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

#region SummarySection
/// <summary>
/// Singleton class that is used to track ship upgrades, and calculate which ship type will the player get with each upgrade
/// </summary>
/// <param name="UpgradeManager"></param>

#endregion
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    private UpgradeManager refUpgradeManager;
    public GameObject[] UpgradeItems;
    public Entity[] UpgradeEntitis;
    public List<Entity> SpawnedEntetis;
    public int RandomDropChance;
    [HideInInspector]
    public float4 UpgradePath;
    [HideInInspector]
    public bool MaxUpgradeReached;
    [HideInInspector]
    public bool SpawnNewDrop;
    public float3 SpawnNewDropPos;
    [HideInInspector]
    public bool removeEntity;
    public Entity entityToRemove;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;
    private int MaxRandomSpawnValue;

    private void Awake()
    {
        Instance = this;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        MaxRandomSpawnValue = UpgradeItems.Length;
        UpgradeEntitis = new Entity[UpgradeItems.Length];
        for (int ItemValue = 0; ItemValue < UpgradeItems.Length; ItemValue++)
        {
            UpgradeEntitis[ItemValue] = GameObjectConversionUtility.ConvertGameObjectHierarchy(UpgradeItems[ItemValue], settings);
        }
        SpawnedEntetis = new List<Entity>();
    }
    private void Update()
    {
        //is enabled in upgrade collision system when the player flies into an upgrade box
        if (removeEntity) 
        {
            RemoveDrop();
        }
        if (SpawnNewDrop) 
        {
            SpawnDropUpgrade();
        }
        if (MaxUpgradeReached) 
        {
            RemoveAllDrops();
        }
    }
    
    public void RemoveAllDrops() 
    {
        if (SpawnedEntetis.Count > 0) 
        {
            foreach (Entity Dropitem in SpawnedEntetis)
            {
                if (Dropitem != null)
                {
                    entityManager.DestroyEntity(Dropitem);
                }
            }
        }
        SpawnedEntetis = new List<Entity>();
    }


    //called from asteroid Manager to spawn item drop
    public void SpawnDropUpgrade() 
    {
        SpawnNewDrop = false;
        Debug.Log("SPANING UPGRADE DROP");
        Entity SpawnUpgradeItem = entityManager.Instantiate(UpgradeEntitis[UnityEngine.Random.Range(0, MaxRandomSpawnValue)]);
        entityManager.SetComponentData(SpawnUpgradeItem, new Translation { Value = SpawnNewDropPos });
        SpawnedEntetis.Add(SpawnUpgradeItem);
        SpawnNewDropPos = new float3(0,0,0);
    }

    public void RemoveDrop() 
    {
        Debug.Log("Removing entity");
        SpawnedEntetis.Remove(entityToRemove);
        removeEntity = false;
        entityManager.DestroyEntity(entityToRemove);
        UserInterfaceManager.Instance.EnableUpgradeUI();
        entityToRemove = Entity.Null;
    }

    /// <summary>
    /// Uses a float 4 to track which upgrade the players on, then calls the upgrade ship method in shipmanager and calls the disable ui method in user interface manager
    /// </summary>
    /// <param name="UpgradeSelected"></param>
    public void UpgradeSelected(float UpgradeValue) 
    {
        #region Upgrade 1
        if (UpgradePath.x == 0)
        {
            switch (UpgradeValue)
            {
                case 1:
                    ShipManager.Instance.currentShipSpeed = 40;
                    ShipManager.Instance.currentShipStrafeSpeed = 40;
                    ShipManager.Instance.currentShipAcceleration = 15;
                    ShipManager.Instance.currentStrafeAcceleration = 15;
                    ShipManager.Instance.currentRollAcceleration = 15;
                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity1[0];
                    break;
                case 2:
                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity1[1];
                    break;
            }
            UpgradePath.x = UpgradeValue;
        }
        #endregion
        #region Upgrade 2
        else if (UpgradePath.y == 0)
        {
            switch (UpgradePath.x)
            {
                case 1:
                    switch (UpgradeValue)
                    {
                        case 1:
                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity2[0];
                            break;
                        case 2:
                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity2[1];
                            break;
                    }
                    break;
                case 2:
                    switch (UpgradeValue)
                    {
                        case 1:
                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity2[2];
                            break;
                        case 2:
                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity2[3];
                            break;
                    }
                    break;
            }
            UpgradePath.y = UpgradeValue;
        }
        #endregion
        #region Upgrade 3
        else if (UpgradePath.z == 0)
        {
            switch (UpgradePath.x)
            {
                case 1:
                    switch (UpgradePath.y)
                    {
                        case 1:
                            switch (UpgradeValue)
                            {
                                case 1:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[0];
                                    break;
                                case 2:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[1];
                                    break;
                            }
                            break;
                        case 2:
                            switch (UpgradeValue)
                            {
                                case 1:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[2];
                                    break;
                                case 2:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[3];
                                    break;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (UpgradePath.y)
                    {
                        case 1:
                            switch (UpgradeValue)
                            {
                                case 1:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[4];
                                    break;
                                case 2:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[5];
                                    break;
                            }
                            break;
                        case 2:
                            switch (UpgradeValue)
                            {
                                case 1:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[6];
                                    break;
                                case 2:
                                    ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity3[7];
                                    break;
                            }
                            break;
                    }
                    break;
            }
            UpgradePath.z = UpgradeValue;
        }
        #endregion
        #region Upgrade 4
        else if (UpgradePath.w == 0)
        {
            MaxUpgradeReached = true;
            switch (UpgradePath.x)
            {
                case 1:
                    switch (UpgradePath.y)
                    {
                        case 1:
                            switch (UpgradePath.z)
                            {
                                case 1:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[0];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[1];
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[2];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[3];
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case 2:
                            switch (UpgradePath.z)
                            {
                                case 1:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[4];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[5];
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[6];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[7];
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (UpgradePath.y)
                    {
                        case 1:
                            switch (UpgradePath.z)
                            {
                                case 1:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[8];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[9];
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[10];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[11];
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case 2:
                            switch (UpgradePath.z)
                            {
                                case 1:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[12];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[13];
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (UpgradeValue)
                                    {
                                        case 1:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[14];
                                            break;
                                        case 2:
                                            ShipManager.Instance.ShipUpgrade = ShipManager.Instance.ShipUpgradeBranchEntity4[15];
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
            #endregion
            UpgradePath.w = UpgradeValue;
        }
        ShipManager.Instance.UpgradeShip();
        UserInterfaceManager.Instance.DisableUpgradeUI();
    }
    private int ReturnNewRandom()
    {
        int generatedRandom = UnityEngine.Random.Range(0, MaxRandomSpawnValue);
        return (generatedRandom);
    }

    //used to clear memeory on close VERY IMPORTANT!!!!!!!!!
    private void OnApplicationQuit()
    {
        foreach (Entity upgrades in UpgradeEntitis)
        {
            entityManager.DestroyEntity(upgrades);
        }
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }
    #region SummarySection
    /// <summary>
    /// Singleton class that is used to track ship upgrades, and calculate which ship type will the player get with each upgrade
    /// </summary>
    /// <param name="UpgradeManager"></param>

    #endregion
}
