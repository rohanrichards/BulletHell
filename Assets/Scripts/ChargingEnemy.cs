using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemy : EnemyBase
{
    Vector3 travelDirection;
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        spriteContainer.rotation = Quaternion.LookRotation(Vector3.forward, travelDirection);
    }

    public override void GetTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D body = target.GetComponentInChildren<Rigidbody2D>();
        // draw a ray from location to player and travel along that ray
        Ray ray = new Ray(rb.transform.position, body.transform.position - rb.transform.position);
        travelDirection = ray.direction;
    }

    private void FixedUpdate()
    {
        rb.AddForce(travelDirection.normalized * config.moveSpeed * rb.mass * Time.deltaTime);

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
/*        StatsController playerStatsController = GameObject.FindObjectOfType<StatsController>();
        playerStatsController.ApplyDamage(1, this);*/
        yield return new WaitForSeconds(attackRate);
        //StartCoroutine(AttemptAttack());
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
}
