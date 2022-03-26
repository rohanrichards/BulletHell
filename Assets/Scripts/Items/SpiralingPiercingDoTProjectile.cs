using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralingPiercingDoTProjectile : BulletBase
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(DamageLoop());

        // set scale based on AOE bonus
        transform.localScale = transform.localScale * AOE;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        // always turning left
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, -120) * Time.fixedDeltaTime);
        rb.MoveRotation(rb.transform.rotation * targetRotation);

        // always moving "forward"
        rb.AddForce(rb.transform.right * config.baseSpeed * Time.deltaTime);
    }

    public override void SetDeath()
    {
        Invoke("KillSelf", config.Lifespan);
    }


    protected override void KillSelf()
    {
        Destroy(gameObject);
    }

    IEnumerator DamageLoop()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemies"));
        List<Collider2D> hits = new List<Collider2D>();
        // get anything it's touching
        GetComponent<Collider2D>().OverlapCollider(filter, hits);
        foreach (Collider2D collision in hits)
        {
            if (collision.gameObject.tag == "Shootable")
            {
                GenericEnemy controller = collision.gameObject.GetComponentInParent<GenericEnemy>();

                if (controller && controller.currentHealth > 0)
                {
                    controller.ApplyDamage(Damage);
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(DamageLoop());
    }
}
