using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class LaserBolt : BulletBase
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        EntityDataComponent type = new EntityDataComponent { Type = EntityDeathTypes.DoesNothingOnDeath, Damage = config.Damage, Size = config.AOE };
        dstManager.AddComponentData(entity, type);
    }

    /*    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Shootable")
            {
                Rigidbody2D targetBody = collision.gameObject.GetComponentInChildren<Rigidbody2D>();
                targetBody.AddForce(rb.velocity * parentWeapon.KnockBackForce * targetBody.mass * Time.fixedDeltaTime);

                IShootable controller = collision.gameObject.GetComponentInParent<IShootable>();
                controller.ApplyDamage(Damage);
                KillSelf();
            }
            else if (collision.gameObject.tag == "Terrain")
            {
                KillSelf();
            }
        }*/
}
