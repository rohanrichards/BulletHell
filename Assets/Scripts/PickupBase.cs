using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    public static GameObject Create(GameObject prefab, Transform origin)
    {
        // create our bullet instance
        GameObject pickupInstance = Instantiate<GameObject>(prefab);

        // set it's origin and rotation
        pickupInstance.transform.position = origin.transform.position;

        return pickupInstance;
    }

    public abstract void Pickup(StatsController statsController);
}
