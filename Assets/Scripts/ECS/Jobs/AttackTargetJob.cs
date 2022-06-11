using Unity.Entities;
using Unity.Burst;
using static EntityMessagingController;
using Unity.Transforms;

[BurstCompile]
partial struct AttackTargetJob : IJobEntity
{
    public float dt;
    public EntityCommandBuffer ecb;
    public EntityHealthComponent targetHealth;
    public Entity target;
    public void Execute(Entity entity, ref EntityDamageComponent damage, in Translation position, in Rotation rotation)
    {
        if(damage.attacking == true)
        {
            damage.attackTime -= dt;
            //UnityEngine.Debug.Log("attack countdown: " + damage.attackTime + " dt is: " + dt);

            if (damage.attackTime <= 0)
            {
                //UnityEngine.Debug.Log("attacking: " + targetHealth.CurrentHealth);
                damage.attackTime = damage.attackCooldown;
                targetHealth.CurrentHealth -= damage.Damage;
                ecb.SetComponent(target, targetHealth);
                ecb.SetComponent(entity, damage);
                // create attack message
                Entity message = ecb.CreateEntity();
                ecb.AddComponent(message, new MessageDataComponent { position=position.Value, rotation = rotation.Value,  type = MessageTypes.Attack });
                ecb.AddComponent<EntityDataComponent>(message, new EntityDataComponent { Type = EntityDeathTypes.DoesNothingOnDeath});
            }
        }
    }
}