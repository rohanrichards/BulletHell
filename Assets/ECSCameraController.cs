using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ECSCameraController : MonoBehaviour
{
    public Entity playerEntity;
    private EntityQuery playerEntityQuery;
    private EntityManager manager;

    private void Awake()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerEntityQuery = manager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<PlayerTag>()
        );
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
        if(playerEntity == null) { return; }

        var location = playerEntityQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        if (location.Length > 0)
        {
            transform.position = new Vector3(location[0].Value.x, location[0].Value.y, transform.position.z);
        }

    }
}
