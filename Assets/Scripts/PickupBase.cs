using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    protected Rigidbody2D playerBody;
    protected StatsController statsController;
    public static GameObject Create(GameObject prefab, Transform origin)
    {
        // create our bullet instance
        GameObject pickupInstance = Instantiate<GameObject>(prefab);

        // set it's origin and rotation
        pickupInstance.transform.position = origin.transform.position;

        return pickupInstance;
    }

    public virtual void Start()
    {
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody2D>();
        statsController = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsController>();
    }

    public virtual void Pickup() { }

}
