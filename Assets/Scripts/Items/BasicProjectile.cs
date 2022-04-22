using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : BulletBase
{
    protected override void Start()
    {
        base.Start();
        // set it's base velocity
        rb.velocity = new Vector2(
            rb.transform.up.x * config.baseSpeed,
            rb.transform.up.y * config.baseSpeed
            );
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void SetDeath()
    {
        Invoke("KillSelf", config.Lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shootable")
        {
            Rigidbody2D targetBody = collision.gameObject.GetComponentInChildren<Rigidbody2D>();
            targetBody.AddForce(rb.velocity * parentWeapon.KnockBackForce * targetBody.mass * Time.fixedDeltaTime);

            IShootable controller = collision.gameObject.GetComponentInParent<IShootable>();
            controller.ApplyDamage(Damage);
            KillSelf();
        }else if(collision.gameObject.tag == "Terrain")
        {
            KillSelf();
        }
    }


    protected override void KillSelf()
    {
        Destroy(gameObject);
    }
}
