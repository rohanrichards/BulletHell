using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public EnemySO config;
    public Rigidbody2D rb;
    public float currentHealth;
    protected GameObject target;
    protected Transform targetTransform;
    public AnimationCurve likelihood;
    public bool isTouchingPlayer;
    public GameObject deathPrefab;
    public bool isSuper;
    public static GameObject Create(GameObject prefab, Vector2 origin, Transform container)
    {
        // create our bullet instance
        GameObject enemyInstance = Instantiate<GameObject>(prefab, (Vector3)origin, new Quaternion(), container);
        EnemyBase controller = enemyInstance.GetComponent<EnemyBase>();
        controller.config = Instantiate<EnemySO>(controller.config);
        controller.rb = enemyInstance.GetComponentInChildren<Rigidbody2D>();

        // set it's origin and rotation
        controller.rb.transform.position = origin;

        return enemyInstance;
    }

    public abstract void GetTarget();
    public abstract void ApplyDamage(float damage);
    public void KillSelf()
    {
        PickupGenerator pickupGenerator = GameObject.FindObjectOfType<PickupGenerator>();
        pickupGenerator.CreateXPOrb(rb.transform, config.XPValue);
        Instantiate(deathPrefab, rb.transform.position, rb.transform.rotation);
        if (isSuper)
        {
            pickupGenerator.CreateChest(rb.transform);
        }
    }
    public abstract float GetLikelihoodWeight(float time);
    public abstract void StartAttacking();
    public abstract void StopAttacking();
    protected abstract IEnumerator AttemptAttack();
}
