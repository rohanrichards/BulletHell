using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPOrbController : PickupBase
{
    public int speed = 100;
    public int value;
    private bool collecting = false;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (collecting)
        {
            if (Vector3.Distance(playerBody.transform.position, transform.position) < 1)
            {
                statsController.ApplyXP(value);
                KillSelf();
            }
            else
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, playerBody.transform.position, step);
            }
        }
    }

    private void FixedUpdate()
    {


    }

    public override void Pickup()
    {
        if (!collecting)
        {
            collecting = true;
        }
    }
    protected void KillSelf()
    {
        Destroy(gameObject);
    }
}
