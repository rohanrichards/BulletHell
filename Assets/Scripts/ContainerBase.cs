using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContainerBase : MonoBehaviour, IShootable
{
    protected Rigidbody2D rb;
    public DestroyableSO config;

    public static GameObject Create(GameObject prefab, Vector2 origin, Transform container)
    {
        GameObject containerInstance = Instantiate<GameObject>(prefab, (Vector3)origin, new Quaternion(), container);
        ContainerBase controller = containerInstance.GetComponent<ContainerBase>();
        controller.config = Instantiate<DestroyableSO>(controller.config);
        controller.rb = containerInstance.GetComponentInChildren<Rigidbody2D>();
        return containerInstance;
    }

    protected virtual void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        rb.tag = "Shootable";
    }

    protected virtual void Update()
    {
        
    }

    public abstract void ApplyDamage(float damage);
    public abstract void KillSelf();

}
