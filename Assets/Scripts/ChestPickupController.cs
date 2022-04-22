using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ChestPickupController : PickupBase
{
    private GameObject player;
    private OpenChestUIController chestUIController;

    public override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
        chestUIController = GameObject.FindObjectOfType<OpenChestUIController>();
    }

    public override void Pickup()
    {
        chestUIController.Show();
        KillSelf();
    }

    protected void KillSelf()
    {
        Destroy(gameObject);
    }
}
