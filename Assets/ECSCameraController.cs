using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ECSCameraController : MonoBehaviour
{

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        var location = ECSPlayerController.getPlayerLocation();
        transform.position = new Vector3(location.Position.x, location.Position.y, transform.position.z);
    }
}
