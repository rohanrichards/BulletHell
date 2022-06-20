using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    private float _Tick = 0.0f;
    private struct lightData
    {
        public float x, y, rad, r, g, b;
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
                res.Add(l.x);
                res.Add(l.y);
                res.Add(l.rad);
                res.Add(l.r);
                res.Add(l.g);
                res.Add(l.b);
            }
            return res.ToArray();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float ang = _Tick * 0.01f;
        float rad = 0.4f;

        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();

        int num_lights = 2;
        //int num_lights = (((int)_Tick / 100) % 2) == 0 ? 1 : 2;

        lightsCollection array = new lightsCollection {
            lights = new lightData[] { 
                new lightData { x = 1 - playerLocation.Position.x/35, y = 1 - playerLocation.Position.y/15, rad = 1, r = 1, g = 1, b = 1 },
                new lightData { x = 0, y = 1, rad = 1, r = 1.0f, g = 0.0f, b = 0.0f }
            }
        };
           // 0.5f + Mathf.Sin(ang) * rad, 0.5f + Mathf.Cos(ang) * rad, 1.5f,
           // Mathf.Abs(Mathf.Sin(ang * 5.0f)), 0.0f, 1.0f
        Shader.SetGlobalFloatArray("_Lights", array.toFloatArray());
        Shader.SetGlobalInt("_LightCount", num_lights);

        _Tick++;
    }
}
