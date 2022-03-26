using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingFlyingProjectile : BulletBase
{
    public float explosionRadius = 5.0f;
    public int targetRange = 10;
    private Vector3 target;
    public GameObject explosionPrefab;

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
        Vector2 direction = rb.transform.position - target;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        targetRotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 1000 * Time.deltaTime);
        rb.MoveRotation(targetRotation);
    }

    public override void SetDeath()
    {
        Invoke("KillSelf", config.Lifespan);
    }


    protected override void KillSelf()
    {
        float explosionRadius = AOE;
        float explosionForce = parentWeapon.KnockBackForce;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemies"));
        List<Collider2D> hits = new List<Collider2D>();
        Physics2D.OverlapCircle(rb.transform.position, explosionRadius, filter, hits);

        foreach (Collider2D collision in hits)
        {
            if(collision.gameObject.tag == "Shootable")
            {
                // push them away from the explosion center
                Rigidbody2D collisionBody = collision.gameObject.GetComponentInChildren<Rigidbody2D>();
                GenericEnemy controller = collision.gameObject.GetComponentInParent<GenericEnemy>();
                Vector2 dir = (collisionBody.transform.position - rb.transform.position);
                float wearoff = 1 - (dir.magnitude / explosionRadius);
                collisionBody.AddForce(dir.normalized * explosionForce * collisionBody.mass * wearoff);

                if (controller && controller.currentHealth > 0)
                {
                    controller.ApplyDamage(Damage);
                }
            }
        }
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    Vector3 RandomPointOnCircleEdge(float radius)
    {
        Vector2 point = UnityEngine.Random.insideUnitCircle.normalized * radius;
        return new Vector3(point.x, point.y, 0);
    }
}
