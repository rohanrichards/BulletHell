using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDestroyable : ContainerBase
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ApplyDamage(float damage)
    {
        config.currentHealth -= damage;
        if (config.currentHealth <= 0)
        {
            KillSelf();
        }
    }

    public override void KillSelf()
    {
        if(Random.Range(0f,1f) > 0.66)
        {
            PickupGenerator pickupGenerator = GameObject.FindObjectOfType<PickupGenerator>();
            pickupGenerator.CreateRepairItem(rb.transform);
        }
        Destroy(gameObject);
    }
}
