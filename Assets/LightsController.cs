using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    private struct lightData
    {
        public Vector3 world, local;
        public float rad, r, g, b;
    }

    private struct lightsCollection
    {
        public lightData[] lights;
        public float[] toFloatArray()
        {
            List<float> res = new List<float>();
            for (int i = 0; i < lights.Length; i++)
            {
                lightData l = lights[i];
                Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(l.world.x, l.world.y, 1));
                l.local = screenPoint;

                res.Add(l.local.x);
                res.Add(l.local.y);
                res.Add(l.rad);
                res.Add(l.r);
                res.Add(l.g);
                res.Add(l.b);
            }
            return res.ToArray();
        }
    }

    private lightsCollection lightsArray;
    private int lightCount;

    // Start is called before the first frame update
    void Awake()
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        lightCount = 5;

        lightsArray = new lightsCollection
        {
            lights = new lightData[] {
                new lightData { world = new Vector3(playerLocation.Position.x, playerLocation.Position.y, 0), rad = 1.0f, r = 1.0f, g = 1.0f, b = 1.0f },
                new lightData { world = new Vector3(0, 5, 0), rad = 0.5f, r = 1.0f, g = 0.0f, b = 0.0f },
                new lightData { world = new Vector3(4, 5, 0), rad = 0.5f, r = 0.0f, g = 1.0f, b = 0.0f },
                new lightData { world = new Vector3(8, 5, 0), rad = 0.5f, r = 0.0f, g = 0.0f, b = 1.0f },
                new lightData { world = new Vector3(12, 5, 0), rad = 0.5f, r = 1.0f, g = 0.0f, b = 1.0f },
                new lightData { world = new Vector3(-10, -10, 0), rad = 0.5f, r = 0.0f, g = 1.0f, b = 0.0f },
                new lightData { world = new Vector3(-10, -10, 0), rad = 0.5f, r = 0.0f, g = 1.0f, b = 0.0f },
                new lightData { world = new Vector3(-10, -10, 0), rad = 0.5f, r = 0.0f, g = 1.0f, b = 0.0f }
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerLight();
        Shader.SetGlobalFloatArray("_Lights", lightsArray.toFloatArray());
        Shader.SetGlobalInt("_LightCount", lightCount);
    }

    private void updatePlayerLight()
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        lightsArray.lights[0].world = playerLocation.Position;
    }
}
