using System.Collections;
using Unity.Entities;
using UnityEngine;

public abstract class WeaponBase : ItemBase
{
    protected Rigidbody2D playerBody;
    protected StatsController statsController;
    public WeaponSO weaponConfig;
    public BulletSO bulletConfig;
    public GameObject bulletPrefab;
    public bool isFiring = false;
    public KeyCode toggleButton;

    // ECS Stuff
    protected Entity bulletEntityPrefab;
    protected BlobAssetStore blobAssetStore;
    protected EntityManager manager;

    public float RateOfFire
    {
        get
        {
            return weaponConfig.ROF + weaponConfig.ROF * (statsController.globalStatsConfig.rofPercentBonus / 100);
        }
    }

    public float KnockBackForce
    {
        get
        {
            return weaponConfig.KnockBackForce + (weaponConfig.KnockBackForce * (statsController.globalStatsConfig.knockbackPercentBonus/ 100));
        }
    }

    public float ProjectileCount
    {
        get
        {
            return weaponConfig.ProjectileCount + statsController.globalStatsConfig.projectileCountBonus;
        }
    }

    protected override void Start()
    {
        base.Start();
        weaponConfig = Instantiate<WeaponSO>(weaponConfig);
        bulletConfig = Instantiate<BulletSO>(bulletConfig);
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(toggleButton))
        {
            if (isFiring)
            {
                StopFiring();
                isFiring = false;
            }
            else
            {
                StartFiring();
                isFiring = true;
            }
        }
    }

    public override void Unlock()
    {
        base.Unlock();
        //enable the weapon and add its upgrades into available upgrades
        foreach (WeaponUpgradeSO upgrade in upgradePool)
        {
            WeaponUpgradeSO instance = Instantiate<WeaponUpgradeSO>(upgrade);
            gameObject.GetComponent<StatsController>().availableUpgrades.Add(instance);
        }
        StartFiring();
    }

    public override void IncreaseLevel()
    {
        base.IncreaseLevel();
        WeaponUpgradeSO levelUpgrade = (WeaponUpgradeSO)levelUpgrades[level - 1];
        levelUpgrade.ApplyUpgrade(this);
    }

    public void StartFiring()
    {
        isFiring = true;
        StartCoroutine(Fire());
    }

    public void StopFiring()
    {
        isFiring = false;
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    public abstract IEnumerator Fire();
}
