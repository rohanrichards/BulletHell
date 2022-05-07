using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GenericEnemy : EnemyBase, IConvertGameObjectToEntity
{
    public override void Start()
    {
        //base.Start();
    }

    public override void Update()
    {
        //base.Update();
    }

    public override void GetTarget()
    {
    }

    private void FixedUpdate()
    {
    }

    public override void StartAttacking()
    {
        StartCoroutine(AttemptAttack());
    }

    public override void StopAttacking()
    {
        StopAllCoroutines();
    }

    protected override IEnumerator AttemptAttack()
    {
        StatsController playerStatsController = GameObject.FindObjectOfType<StatsController>();
        playerStatsController.ApplyDamage(1, this);
        yield return new WaitForSeconds(attackRate);
        StartCoroutine(AttemptAttack());
    }

    public override void ApplyDamage(float damage)
    {
        base.ApplyDamage(damage);
    }

    public new void KillSelf()
    {
        //base creates xp orbs
        base.KillSelf();
    }

    public override float GetLikelihoodWeight(float time)
    {
        return likelihood.Evaluate(time);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(MoveTowardTargetTag));
        dstManager.AddComponent(entity, typeof(RotateToTargetTag));
        dstManager.AddComponent(entity, typeof(EnemyTag));

/*        dstManager.AddComponent(entity, typeof(EntityMovementSettings));
        dstManager.SetComponentData(entity, new EntityMovementSettings { moveSpeed = config.moveSpeed });


        dstManager.AddComponent<EntityHealthComponent>(entity);
        dstManager.SetComponentData(entity, new EntityHealthComponent { CurrentHealth = config.currentHealth, MaxHealth = config.baseHealth});*/
    }
}
