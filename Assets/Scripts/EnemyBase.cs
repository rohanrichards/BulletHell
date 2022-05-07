using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IShootable
{
    [HideInInspector]
    public Rigidbody2D rb;
    public EnemySO config;
    protected GameObject target;
    protected Transform targetTransform;
    public AnimationCurve likelihood;
    public bool isTouchingPlayer;
    public GameObject deathPrefab;
    public bool isSuper;
    public float attackRate = 0.85f;
    protected PickupGenerator pickupGenerator;
    protected Transform spriteContainer;
    static private Dictionary<string, Queue<GameObject>> enemyPool = new Dictionary<string, Queue<GameObject>>();

    public static GameObject Create(GameObject prefab, Vector2 origin, Transform container)
    {
        GameObject enemyInstance = GetFromPool(prefab.name+"(Clone)");
        if (enemyInstance)
        {
            enemyInstance.transform.position = origin;
            enemyInstance.SetActive(true);
        }else
        {
            enemyInstance = Instantiate<GameObject>(prefab, (Vector3)origin, new Quaternion(), container);
        }        
        EnemyBase controller = enemyInstance.GetComponent<EnemyBase>();
        controller.config = Instantiate<EnemySO>(controller.config);
        controller.rb = enemyInstance.GetComponent<Rigidbody2D>();
        controller.config.currentHealth = controller.config.baseHealth;
        controller.GetTarget();

        return enemyInstance;
    }


    public static GameObject GetFromPool(string name)
    {
        Queue<GameObject> enemies = new Queue<GameObject>();
        if(enemyPool.TryGetValue(name, out enemies))
        {
            try
            {
                return enemies.Dequeue();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        return null;
    }

    public static void SendToPool(GameObject enemy)
    {
        if (!enemyPool.ContainsKey(enemy.name))
        {
            enemyPool.Add(enemy.name, new Queue<GameObject>());
        }
        enemyPool[enemy.name].Enqueue(enemy);
        enemy.SetActive(false);
    }

    public virtual void Start()
    {
        pickupGenerator = GameObject.FindObjectOfType<PickupGenerator>();
        rb = GetComponentInChildren<Rigidbody2D>();
        spriteContainer = rb.transform.Find("Sprites");
    }

    public virtual void Update()
    {
        if (config.currentHealth <= 0)
        {
            KillSelf();
        }
    }

    public abstract void GetTarget();
    public virtual void ApplyDamage(float damage)
    {
        if(config.currentHealth > 0)
        {
            config.currentHealth -= damage;
        }
    }
    public virtual void KillSelf()
    {
        pickupGenerator.CreateXPOrb(rb.transform, config.XPValue);
        GameObject corpse = Instantiate(deathPrefab, rb.transform.position, spriteContainer.transform.rotation);
        corpse.transform.localScale = gameObject.transform.localScale;
        StopAllCoroutines();

        if (isSuper)
        {
            pickupGenerator.CreateChest(rb.transform);
            Destroy(gameObject);
        }

        SendToPool(gameObject);
    }
    public abstract float GetLikelihoodWeight(float time);
    public abstract void StartAttacking();
    public abstract void StopAttacking();
    protected abstract IEnumerator AttemptAttack();
}
