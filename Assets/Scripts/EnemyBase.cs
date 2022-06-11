using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IConvertGameObjectToEntity
{
    public EnemySO config;
    public AnimationCurve likelihood;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(EnemyTag));
        dstManager.AddComponent(entity, typeof(ShootableTag));

        dstManager.AddComponentData(entity, new EntityDataComponent { Type = EntityDeathTypes.SplattersOnDeath, Size = transform.localScale.x, XP = config.XPValue });
        dstManager.AddComponentData(entity, new EntityMovementSettings { moveSpeed = config.moveSpeed });
        dstManager.AddComponentData(entity, new EntityHealthComponent { CurrentHealth = config.currentHealth, MaxHealth = config.baseHealth });
        dstManager.AddComponentData(entity, new EntityDamageComponent { Damage = config.damage, attacking = false, attackTime = 0, attackCooldown = 1f, attackRange = 1.5f });
    }

    public float GetLikelihoodWeight(float time)
    {
        return likelihood.Evaluate(time);
    }
}
