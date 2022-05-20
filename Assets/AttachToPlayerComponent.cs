using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class AttachToPlayerComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LocalToWorld playerLoc = ECSPlayerController.getPlayerLocation();
        transform.position = playerLoc.Position;
    }
}
