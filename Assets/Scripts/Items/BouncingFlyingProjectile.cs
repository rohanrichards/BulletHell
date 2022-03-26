using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingFlyingProjectile : BulletBase
{
    public int targetRange = 10;
    private Vector3 target;

    protected override void Start()
    {
        base.Start();
        // pick a target
        target = rb.transform.position + RandomPointOnCircleEdge(targetRange);
        Vector3 directionToTarget = rb.transform.position - target;
        // set the base velocity
        rb.velocity = directionToTarget.normalized * config.baseSpeed;
    }

    private void FixedUpdate()
    {
        
    }

    public override void SetDeath()
    {
        Invoke("KillSelf", config.Lifespan);
    }


    protected override void KillSelf()
    {
        Destroy(gameObject);
    }
    Vector3 RandomPointOnCircleEdge(float radius)
    {
        Vector2 point = UnityEngine.Random.insideUnitCircle.normalized * radius;
        return new Vector3(point.x, point.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shootable" || collision.gameObject.tag == "Terrain")
        {
            Debug.Log("bouncing");
            // shoot a ray out in the direction of the collider
            Ray ray = new Ray(rb.transform.position, rb.transform.position - collision.gameObject.transform.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);
            rb.velocity = reflection.normalized * config.baseSpeed;

            if(collision.gameObject.tag == "Shootable")
            {
                GenericEnemy controller = collision.gameObject.GetComponentInParent<GenericEnemy>();
                Rigidbody2D targetBody = collision.gameObject.GetComponentInChildren<Rigidbody2D>();
                targetBody.AddForce(-ray.direction.normalized * config.baseSpeed * parentWeapon.KnockBackForce * targetBody.mass * Time.fixedDeltaTime);

                if (controller && controller.currentHealth > 0)
                {
                    controller.ApplyDamage(Damage);
                }
            }
        }
    }
}
