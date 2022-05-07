using System.Collections;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;

public class Launcher : WeaponBase
{
    public GameObject bulletDeathPrefab;
    private EntityQuery playerQuery;
    protected Entity bulletEntityPrefab;
    protected BlobAssetStore blobAssetStore;
    private EntityManager manager;
    protected override void Start()
    {
        base.Start();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerQuery = manager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<PlayerTag>()
        );
        blobAssetStore = new BlobAssetStore();
        bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Unlock()
    {
        base.Unlock();
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    public override IEnumerator Fire()
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        Debug.Log(playerLocation.Right);
        float arcSize = 90;
        float arcSegment = arcSize / ProjectileCount;
        float offsetWidth = 0.75f;
        float offsetSegment = offsetWidth / ProjectileCount;
        for (int i = 0; i < ProjectileCount; i++)
        {

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            Vector3 originOffset = playerLocation.Up + (playerLocation.Right * ((offsetWidth / 2) - offset));
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            BulletBase.CreateEntity(bulletEntityPrefab, playerLocation, originOffset, Quaternion.Euler(rotation) * (Quaternion)playerLocation.Rotation, offsetVector, bulletConfig, this);
        }
        yield return new WaitForSeconds(1 / RateOfFire);
        StartCoroutine(Fire());
    }
}
