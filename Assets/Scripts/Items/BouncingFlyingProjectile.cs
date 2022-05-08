using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingFlyingProjectile : BulletBase
{
    protected override void Start()
    {
        base.Start();
    }

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shootable" || collision.gameObject.tag == "Terrain")
        {
            // shoot a ray out in the direction of the collider
            Ray ray = new Ray(rb.transform.position, rb.transform.position - collision.gameObject.transform.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);
            rb.velocity = reflection.normalized * config.baseSpeed;

            if(collision.gameObject.tag == "Shootable")
            {
                IShootable controller = collision.gameObject.GetComponentInParent<IShootable>();
                Rigidbody2D targetBody = collision.gameObject.GetComponentInChildren<Rigidbody2D>();
                targetBody.AddForce(-ray.direction.normalized * config.baseSpeed * parentWeapon.KnockBackForce * targetBody.mass * Time.fixedDeltaTime);
                controller.ApplyDamage(Damage);
            }
        }
    }*/
}
