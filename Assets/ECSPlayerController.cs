using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ECSPlayerController : MonoBehaviour
{
    public GameObject playerPrefab;
    protected EntityManager entityManager;
    protected Entity playerEntityPrefab;
    protected BlobAssetStore blobAssetStore;
    private static EntityQuery playerQuery;

    public static LocalToWorld getPlayerLocation()
    {
        var queryResult = playerQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        if (queryResult.Length > 0)
        {
            return queryResult[0];
        }
        else
        {
            return new LocalToWorld();
        }
    }

    public void Awake()
    {
        playerQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<LocalToWorld>(),
            ComponentType.ReadWrite<PlayerTag>()
        );
    }

    void Start()
    {
        blobAssetStore = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));

        Entity player = entityManager.Instantiate(playerEntityPrefab);
        entityManager.SetComponentData(player, new Translation());
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);

        PlayerMoverSystem mover = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerMoverSystem>();
        mover.moveSpeed = 200;
        mover.direction = movement;

        if(movement != Vector3.zero)
        {
            PlayerRotatorSystem rotator = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerRotatorSystem>();
            rotator.direction = movement;
            rotator.turnSpeed = 0.1f;
        }
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}
