using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class Discharger : WeaponBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Unlock()
    {
        base.Unlock();
    }

    public override IEnumerator Fire()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        List<Entity> bullets = weaponConfig.FireFunc(weaponConfig, bulletEntityPrefab, true);

        foreach (Entity entity in bullets)
        {
            Scale scale = new Scale { Value = 0.75f * weaponConfig.AOE };
            entityManager.AddComponentData<Scale>(entity, scale);

            PhysicsVelocity vel = entityManager.GetComponentData<PhysicsVelocity>(entity);
            vel.Linear += UnityEngine.Random.Range(-weaponConfig.Speed * 0.3f, weaponConfig.Speed * 0.1f );
            entityManager.SetComponentData<PhysicsVelocity>(entity, vel);

            RebuildCollider(entity);
        }

        yield return new WaitForSeconds(1 / weaponConfig.ROF);
        StartCoroutine(Fire());
    }

    private void RebuildCollider(Entity entity)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        unsafe
        {
            PhysicsCollider collider = entityManager.GetComponentData<PhysicsCollider>(entity);
            Unity.Physics.SphereCollider* ptr = (Unity.Physics.SphereCollider*)collider.ColliderPtr;
            SphereGeometry sphere = new SphereGeometry
            {
                Radius = 0.5f * weaponConfig.AOE,
            };
            ptr->Geometry = sphere;
            manager.AddComponentData(entity, collider);
        }
    }
}
