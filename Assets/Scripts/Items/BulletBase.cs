using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public BulletSO config;
    [HideInInspector]
    public WeaponBase parentWeapon;
    protected Rigidbody2D rb;
    protected StatsController statsController;
    protected GameObject player;

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

    public static GameObject Create(GameObject prefab, Transform origin, Vector3 offset, Quaternion rotation, BulletSO config, WeaponBase weapon)
    {
        // create our bullet instance
        GameObject bulletInstance = Instantiate<GameObject>(prefab, origin.position + offset, rotation);
        BulletBase controller = bulletInstance.GetComponent<BulletBase>();
        controller.config = config;
        controller.parentWeapon = weapon;
        controller.rb = bulletInstance.GetComponent<Rigidbody2D>();

        // tell it when to die
        controller.SetDeath();

        return bulletInstance;
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        statsController = player.GetComponent<StatsController>();
    }

    protected virtual void Update()
    {

    }
    public abstract void SetDeath();
    protected abstract void KillSelf();

}
