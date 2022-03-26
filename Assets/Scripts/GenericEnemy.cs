using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : EnemyBase
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        currentHealth = config.baseHealth;
        //find a target, usually the player at this stage
        GetTarget();
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

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        targetRotation = Quaternion.RotateTowards(rb.transform.rotation, targetRotation, 360 * Time.fixedDeltaTime);
        rb.MoveRotation(targetRotation);
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
        Debug.Log("Attacking Player");
        StatsController playerStatsController = GameObject.FindObjectOfType<StatsController>();
        playerStatsController.ApplyDamage(1);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(AttemptAttack());
    }

    public override void ApplyDamage(float damage)
    {
        Debug.Log("applying damage: " + damage);
        Debug.Log("health before: " + currentHealth);
        currentHealth -= damage;
        Debug.Log("health after: " + currentHealth);
        if(currentHealth <= 0)
        {
            KillSelf();
        }
    }

    public new void KillSelf()
    {
        //base creates xp orbs
        base.KillSelf();
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public override float GetLikelihoodWeight(float time)
    {
        return likelihood.Evaluate(time);
    }
}
