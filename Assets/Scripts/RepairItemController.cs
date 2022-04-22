using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairItemController : PickupBase
{
    public int value;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Pickup()
    {
        statsController.ApplyHealth(value);
        KillSelf();
    }
    protected void KillSelf()
    {
        Destroy(gameObject);
    }
}
