using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PickupGenerator : MonoBehaviour
{
    public GameObject XPOrbPrefab;
    public GameObject chestPrefab;
    public GameObject repairPrefab;
    private Entity XPOrbEntityPrefab;
    private Entity chestEntityPrefab;
    private Entity repairEntityPrefab;

    protected EntityManager entityManager;
    protected BlobAssetStore blobAssetStore;

    void Start()
    {
        blobAssetStore = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        XPOrbEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(XPOrbPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
        chestEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(chestPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
        repairEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(repairPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
    }

    void Update()
    {
        
    }

    public void CreateXPOrb(float3 location, int value)
    {
        Entity newPickup = PickupBase.Create(XPOrbEntityPrefab, location);
        entityManager.AddComponentData(newPickup, new EntityDataComponent { XP = value });
        entityManager.AddComponentData(newPickup, new EntityMovementSettings { moveSpeed = 20 });
    }

    public void CreateChest(float3 location)
    {
        Entity newPickup = PickupBase.Create(chestEntityPrefab, location);
        entityManager.AddComponentData(newPickup, new EntityDataComponent { Chest = true });
    }

    public void CreateRepairItem(float3 location, int value)
    {
        Entity newPickup = PickupBase.Create(repairEntityPrefab, location);
        entityManager.AddComponentData(newPickup, new EntityDataComponent { Health = value });
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}
