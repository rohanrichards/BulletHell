using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine;

public class ChargingEnemyAuthoringComponent : EnemyBase, IConvertGameObjectToEntity
{
    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);
        dstManager.AddComponent(entity, typeof(MoveForwardTag));
        dstManager.AddComponent(entity, typeof(RotateToFixedTargetTag));
        dstManager.AddComponent(entity, typeof(EntityTargetSettings));
        //dstManager.AddComponent(entity, typeof(BoidTag));
    }
    /*    public override void GetTarget()
        {
            target = GameObject.FindGameObjectWithTag("Player");
            Rigidbody2D body = target.GetComponentInChildren<Rigidbody2D>();
            // draw a ray from location to player and travel along that ray
            Ray ray = new Ray(rb.transform.position, body.transform.position - rb.transform.position);
            travelDirection = ray.direction;
        }*/
}
