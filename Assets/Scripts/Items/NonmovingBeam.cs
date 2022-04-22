using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonmovingBeam : BulletBase
{

    private ContactFilter2D targetFilter;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D collider;
    List<Collider2D> collisions = new List<Collider2D>();
    public float attackRateInSeconds = 0.2f;
    private ParticleSystem particlesTop;
    private ParticleSystem particlesTrail;
    protected override void Start()
    {
        base.Start();
        targetFilter = new ContactFilter2D();
        targetFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        targetFilter.useTriggers = true;
        spriteRenderer = transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();       
        StartCoroutine(AttemptAttack());
        particlesTop = transform.Find("ParticlesTop").gameObject.GetComponent<ParticleSystem>();
        particlesTrail = transform.Find("ParticlesTrail").gameObject.GetComponent<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();
        rb.transform.position = playerBody.transform.position;
        rb.transform.rotation = Quaternion.Euler(originalOffset) * playerBody.transform.rotation;

        float distance = (10 + AOE);

        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, rb.transform.up, distance, targetFilter.layerMask);
        if (hit)
        {
            distance = hit.distance / transform.localScale.y;
        }
        else
        {
            distance = distance / transform.localScale.y;
        }

        spriteRenderer.size = new Vector2(spriteRenderer.size.x, distance);

        collider.size = new Vector2(collider.size.x, distance);
        collider.offset = new Vector2(0, distance / 2);

        ParticleSystem.ShapeModule topShape = particlesTop.shape;
        ParticleSystem.ShapeModule trailShape = particlesTrail.shape;

        topShape.position = new Vector3(topShape.position.x, distance - topShape.radius * 2);
        trailShape.position = new Vector3(topShape.position.x, distance - topShape.radius * 2);
    }

    public void FixedUpdate()
    {
    }
    public override void SetDeath()
    {
        Invoke("KillSelf", config.Lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisions.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collisions.Remove(collision);
    }

    private  IEnumerator AttemptAttack()
    {
        foreach(Collider2D coll in collisions)
        {
            if (coll.gameObject.tag == "Shootable")
            {
                IShootable controller = coll.gameObject.GetComponentInParent<IShootable>();
                Rigidbody2D targetBody = coll.gameObject.GetComponentInChildren<Rigidbody2D>();
                Vector3 direction = coll.transform.position - collider.transform.position;
                targetBody.AddForce(direction.normalized * 50 * parentWeapon.KnockBackForce * targetBody.mass * Time.fixedDeltaTime);
                controller.ApplyDamage(Damage);
            }
        }
        yield return new WaitForSeconds(attackRateInSeconds);
        StartCoroutine(AttemptAttack());
    }

    protected override void KillSelf()
    {
        particlesTrail.transform.parent = null;
        particlesTrail.Stop();
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
