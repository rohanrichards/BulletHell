using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class ContainerBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    public DestroyableSO config;

    public static GameObject Create(GameObject prefab, Vector2 origin, Transform container)
    {
        GameObject containerInstance = Instantiate<GameObject>(prefab, (Vector3)origin, new Quaternion(), container);
        ContainerBase controller = containerInstance.GetComponent<ContainerBase>();
        controller.config = Instantiate<DestroyableSO>(controller.config);
        return containerInstance;
    }
}
