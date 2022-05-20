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
    protected StatsController stats;

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

    public static PhysicsVelocity getPlayerPhysicsVelocity()
    {
        var queryResult = playerQuery.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp);
        if (queryResult.Length > 0)
        {
            return queryResult[0];
        }
        else
        {
            return new PhysicsVelocity();
        }
    }

    public static EntityHealthComponent getPlayerHealth()
    {
        var queryResult = playerQuery.ToComponentDataArray<EntityHealthComponent>(Allocator.Temp);
        if (queryResult.Length > 0)
        {
            return queryResult[0];
        }
        else
        {
            return new EntityHealthComponent();
        }
    }

    public static EntityXPComponent getPlayerXP()
    {
        var queryResult = playerQuery.ToComponentDataArray<EntityXPComponent>(Allocator.Temp);
        if (queryResult.Length > 0)
        {
            return queryResult[0];
        }
        else
        {
            return new EntityXPComponent();
        }
    }

    public void Awake()
    {
        playerQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<EntityHealthComponent>(),
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

        stats = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
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
        mover.moveSpeed = stats.statsConfig.RotateSpeed;
        mover.direction = movement;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = new Vector3(mousePosition.x - getPlayerLocation().Position.x, mousePosition.y - getPlayerLocation().Position.y, 0);

        PlayerRotatorSystem rotator = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerRotatorSystem>();
        rotator.direction = direction;
        rotator.turnSpeed = stats.statsConfig.RotateSpeed;
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}
