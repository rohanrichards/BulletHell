using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class BeamLaser : WeaponBase
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
        List<Entity> bullets = weaponConfig.FireFunc(weaponConfig, bulletEntityPrefab);

        foreach (Entity entity in bullets)
        {
            AttachToTargetComponent attached = new AttachToTargetComponent { target = ECSPlayerController.getPlayerEntity() };
            entityManager.AddComponentData(entity, attached);

            PhysicsVelocity vel = entityManager.GetComponentData<PhysicsVelocity>(entity);
            vel.Linear = 0;
            entityManager.SetComponentData<PhysicsVelocity>(entity, vel);

            EntityOffsetData offset = entityManager.GetComponentData<EntityOffsetData>(entity);
            offset.positionOffset = new Translation { Value = { x = 0, y = 0.5f + weaponConfig.AOE / 2, z = 0 } };
            entityManager.SetComponentData<EntityOffsetData>(entity, offset);

            //NonUniformScale scale = entityManager.GetComponentData<NonUniformScale>(entity);
            NonUniformScale scale = new NonUniformScale { Value = new float3 { x = 2, y = 1 * weaponConfig.AOE, z = 1 } };
            entityManager.AddComponentData<NonUniformScale>(entity, scale);

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
            Unity.Physics.CapsuleCollider* ptr = (Unity.Physics.CapsuleCollider*)collider.ColliderPtr;
            CapsuleGeometry sphere = new CapsuleGeometry
            {
                Radius = 0.5f,
                Vertex0 = new float3 { x = -1, y = -weaponConfig.AOE / 2, z = 0 },
                Vertex1 = new float3 { x = 1, y = weaponConfig.AOE / 2, z = 0 }
            };
            ptr->Geometry = sphere;
            manager.AddComponentData(entity, collider);
        }
    }
}
