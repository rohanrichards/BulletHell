using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

public partial class PlayerMoverSystem : SystemBase
{
    public float3 direction;
    public float moveSpeed;

    private EntityQuery playerEntityQuery;
    protected override void OnStartRunning()
    {
        playerEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<PlayerTag>()
        );
    }

    protected override void OnUpdate()
    {
        MoveInDirectionJob moveJob = new MoveInDirectionJob { dt = Time.DeltaTime, direction = direction };
        moveJob.Schedule(playerEntityQuery);
    }
}