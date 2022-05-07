using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public BulletSO config;
    public GameObject deathPrefab;
    [HideInInspector]
    public WeaponBase parentWeapon;
    protected Rigidbody2D rb;
    protected StatsController statsController;
    protected GameObject player;
    protected Rigidbody2D playerBody;
    protected Vector3 originalOffset;
    static protected EntityManager entityManager;


    public float Damage
    {
        get
        {
            return config.Damage + config.Damage * (statsController.globalStatsConfig.damagePercentBonus / 100);
        }
    }

    public float AOE
    {
        get
        {
            return config.AOE + config.AOE * (statsController.globalStatsConfig.areaPercentBonus / 100);
        }
    }

    public static GameObject Create(GameObject prefab, Transform origin, Vector3 offset, Quaternion rotation, Vector3 rotationOffset, BulletSO config, WeaponBase weapon)
    {
        // create our bullet instance
        GameObject bulletInstance = Instantiate<GameObject>(prefab, origin.position + offset, rotation);
        BulletBase controller = bulletInstance.GetComponent<BulletBase>();
        controller.originalOffset = rotationOffset;
        controller.config = config;
        controller.parentWeapon = weapon;
        controller.rb = bulletInstance.GetComponent<Rigidbody2D>();

        // tell it when to die
        controller.SetDeath();

        return bulletInstance;
    }

    public static Entity CreateEntity(Entity prefab, LocalToWorld origin, Vector3 offset, Quaternion rotation, Vector3 rotationOffset, BulletSO config, WeaponBase weapon)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // create our bullet instance
        Entity bullet = entityManager.Instantiate(prefab);

        entityManager.SetComponentData(bullet, new Translation { Value = origin.Position+(float3)offset });
        Rotation rot = new Rotation { Value = rotation };
        entityManager.SetComponentData(bullet, rot);

/*        BulletBase controller = bulletInstance.GetComponent<BulletBase>();
        controller.originalOffset = rotationOffset;
        controller.config = config;
        controller.parentWeapon = weapon;
        controller.rb = bulletInstance.GetComponent<Rigidbody2D>();*/

        // tell it when to die
        /*controller.SetDeath();*/

        return bullet;
    }

    private void Awake()
    {
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerBody = player.GetComponentInChildren<Rigidbody2D>();
        statsController = player.GetComponent<StatsController>();
    }

    protected virtual void Update()
    {

    }
    public abstract void SetDeath();
    protected abstract void KillSelf();

}
