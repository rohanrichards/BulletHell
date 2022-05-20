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

        RaycastSystem raycaster = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<RaycastSystem>();
        raycaster.target = ECSPlayerController.getPlayerLocation().Position;
    }
}
