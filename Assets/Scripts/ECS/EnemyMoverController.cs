using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnemyMoverController : MonoBehaviour
{
    private void Start()
    {
    }

    private void FixedUpdate()
    {
        EnemyRotatorSystem rotator = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EnemyRotatorSystem>();
        rotator.turnSpeed = .03f;

        DamagerRaycastSystem raycaster = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<DamagerRaycastSystem>();
        raycaster.target = ECSPlayerController.getPlayerLocation().Position;
    }
}
