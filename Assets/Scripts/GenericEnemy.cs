using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : EnemyBase
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        Vector3 targetDirection = targetTransform.transform.position - rb.transform.position;
        spriteContainer.rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
    }

    public override void GetTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        targetTransform = target.GetComponentInChildren<Rigidbody2D>().transform;
    }

    private void FixedUpdate()
    {
        Vector2 direction = targetTransform.transform.position - rb.transform.position;
        rb.AddForce(direction.normalized * config.moveSpeed * rb.mass * Time.deltaTime);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
/*        Vector2 direction = rb.transform.position - collision.transform.position;
        //Rigidbody2D targetBody = collision.gameObject.GetComponent<Rigidbody2D>();
        float len = direction.magnitude;
        Vector2 normalDir = direction * (1 / len);
        Vector2 force = normalDir * (1 / len * len);
        rb.AddForce(force * Time.fixedDeltaTime * .1f);*/
    }
}
