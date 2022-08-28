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
    protected static EntityManager entityManager;
    protected Entity playerEntityPrefab;
    protected BlobAssetStore blobAssetStore;
    private static EntityQuery playerQuery;
    public static StatsController stats;
    private static Entity player;
    private static float3 playerPosRollingAvg;

    public static float minLen = 10.0f;
    public static float maxLen = 20.0f;
    public static float minArc = 90.0f;
    public static float maxArc = 360.0f;

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

    public static void setPlayerHealth(int value)
    {
        var queryResult = playerQuery.ToComponentDataArray<EntityHealthComponent>(Allocator.Temp);
        if (queryResult.Length > 0)
        {
            EntityHealthComponent health = queryResult[0];
            health.CurrentHealth = value;
            entityManager.SetComponentData(player, health);
        }
        else
        {
            entityManager.SetComponentData(player, new EntityHealthComponent{ CurrentHealth = value, MaxHealth = value});
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

    public static Entity getPlayerEntity()
    {
        return player;
    }

    public static float2 getSpawnBiasDegreesArc() {
        float3 biasDir = getPlayerLocation().Position - playerPosRollingAvg;
        float len = math.length(biasDir);
        float arcFrac = 1.0f - math.max(0.0f, math.min(1.0f, (len - minLen) / (maxLen - minLen)));
        float angle = 57.2958f * Mathf.Atan2(biasDir.y, biasDir.x);
        return new float2(angle, minArc + arcFrac * (maxArc - minArc));
    }

    public static float3 getPlayerMoveDirection()
    {
        return (getPlayerLocation().Position - playerPosRollingAvg);
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

        blobAssetStore = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Start()
    {
        if (player != Entity.Null)
        {
            playerPosRollingAvg = getPlayerLocation().Position;
        }
    }

    public void CreatePlayer(StatsConfigSO startingPlayerStatsConfig, GlobalStatsConfigSO startingPlayerGlobalStatsConfig)
    {
        stats = GetComponent<StatsController>();
        stats.statsConfig = Instantiate(startingPlayerStatsConfig);
        stats.globalStatsConfig = Instantiate(startingPlayerGlobalStatsConfig);

        playerEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
        player = entityManager.Instantiate(playerEntityPrefab);
        entityManager.SetComponentData(player, new Translation());
        entityManager.AddComponentData(player, new LightDataComponent { radius = 1.5f, color = { x = 1f, y = 1f, z = 1f }, intensity = 3 });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.FindObjectOfType<PauseGameUIController>().Toggle();
        }
    }

    private void FixedUpdate()
    {

        if (player != Entity.Null)
        {
            float3 pos = getPlayerLocation().Position;
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);

            PlayerMoverSystem mover = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerMoverSystem>();
            mover.moveSpeed = stats.statsConfig.RotateSpeed;
            mover.direction = movement;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 direction = new Vector3(mousePosition.x - pos.x, mousePosition.y - pos.y, 0);

            PlayerRotatorSystem rotator = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerRotatorSystem>();
            rotator.direction = direction;
            rotator.turnSpeed = stats.statsConfig.RotateSpeed;

            float frac = 0.995f;
            playerPosRollingAvg = playerPosRollingAvg * frac + pos * (1.0f - frac);

            {
                float2 angleArc = getSpawnBiasDegreesArc();
                float radiansA = (angleArc.x + -0.5f * angleArc.y) * 0.01745329251f; // convert degrees to radians
                float radiansB = (angleArc.x + 0.5f * angleArc.y) * 0.01745329251f; // convert degrees to radians
                float3 pointA = new float3(math.cos(radiansA), math.sin(radiansA), 0.0f) * 5.0f;
                float3 pointB = new float3(math.cos(radiansB), math.sin(radiansB), 0.0f) * 5.0f;
                Debug.DrawLine(pos, pos + pointA, Color.red, 0.02f);
                Debug.DrawLine(pos, pos + pointB, Color.green, 0.02f);
            }
        }
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}
