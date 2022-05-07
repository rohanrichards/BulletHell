using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Jobs;

public partial class PlayerRotatorSystem : SystemBase
{
    public float3 direction;
    public float turnSpeed;
    private EntityQuery playerQuery;

    protected override void OnStartRunning()
    {
        playerQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(), 
            ComponentType.ReadWrite<PlayerTag>()
        );
    }

    protected override void OnUpdate()
    {
        RotateToDirectionJob job = new RotateToDirectionJob { dt = Time.DeltaTime, direction = direction, turnSpeed = turnSpeed };
        job.Schedule(playerQuery);
    }
}
