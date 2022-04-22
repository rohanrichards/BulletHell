using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupColliderController : MonoBehaviour
{
    GameObject player;
    StatsController controller;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<StatsController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pickups")
        {
            PickupBase pickupController = other.gameObject.GetComponent<PickupBase>();
            pickupController.Pickup();
        }
    }
}
