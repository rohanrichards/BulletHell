using Unity.Entities;
using Unity.Burst;

[BurstCompile]
partial struct LifespanReducerJob : IJobEntity
{
    public float dt;

    public void Execute(ref LifespanComponent lifespan)
    {
        lifespan.Value -= dt;
    }
}