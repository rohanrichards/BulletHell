using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GenericDestroyable : ContainerBase, IConvertGameObjectToEntity
{    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(ShootableTag));

        dstManager.AddComponentData(entity, GetPickupType());
        dstManager.AddComponentData(entity, new EntityHealthComponent { CurrentHealth = config.currentHealth, MaxHealth = config.baseHealth });
    }

    private EntityDataComponent GetPickupType()
    {
        float chance = Random.Range(0f, 1f);
        if(chance <= 0.075)
        {
            return new EntityDataComponent { Type = EntityDeathTypes.DoesNothingOnDeath, Chest = true };
        }else if(chance <= 0.25) {
            return new EntityDataComponent { Type = EntityDeathTypes.DoesNothingOnDeath, Health = 10 };
        }
        else
        {
            return new EntityDataComponent { Type = EntityDeathTypes.DoesNothingOnDeath};
        }
    }
}
