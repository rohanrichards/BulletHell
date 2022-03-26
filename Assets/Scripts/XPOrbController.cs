using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPOrbController : PickupBase
{
    public int value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Pickup(StatsController statsController)
    {
        statsController.ApplyXP(value);
        KillSelf();
    }
    protected void KillSelf()
    {
        Destroy(gameObject);
    }
}
