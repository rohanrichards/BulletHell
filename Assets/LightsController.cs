using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static LightManagerSystem;

public class LightsController : MonoBehaviour
{
    private List<int> trackedLights;
    // this will need to be updated in the shader too
    private int maxLights = 25;
    private LightsCollection lightsArray;

    private class LightsCollection
    {
        public List<LightData> lights = new List<LightData>();
        public float[] toFloatArray()
        {
            List<float> res = new List<float>();
            for (int i = 0; i < lights.Count; i++)
            {
                LightData l = lights[i];
                Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(l.position.x, l.position.y, 1));

                res.Add(screenPoint.x);
                res.Add(screenPoint.y);
                res.Add(l.radius);
                res.Add(l.rgb.x * l.intensity);
                res.Add(l.rgb.y * l.intensity);
                res.Add(l.rgb.z * l.intensity);
            }
            return res.ToArray();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        trackedLights = new List<int>();
        lightsArray = new LightsCollection();
        for(int i = 0; i < maxLights; i++)
        {
            LightData light = new LightData { position = new Vector3(0.0f, 0.0f, 0.0f), intensity = 1, radius = 0.0f, rgb = { x = 0.0f, y = 0.0f, z = 0.0f } };
            lightsArray.lights.Add(light);
        }
        Shader.SetGlobalFloatArray("_Lights", lightsArray.toFloatArray());
        Shader.SetGlobalInt("_LightCount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(lightsArray.lights.Count > 0)
        {
            Shader.SetGlobalFloatArray("_Lights", lightsArray.toFloatArray());
        }
        Shader.SetGlobalInt("_LightCount", lightsArray.lights.Count);
    }

    public void setLights(NativeList<LightManagerSystem.LightData> lightInfo)
    {
        lightsArray = new LightsCollection();
        List<int> newTrackedLights = new List<int>();

        for (int i = 0; i < lightInfo.Length; i++)
        {
            if (isOffScreen(lightInfo[i].position))
            {
                continue;
            }
            int index = lightInfo[i].index;
            bool exists = trackedLights.Contains(index);
            if (exists && lightsArray.lights.Count < maxLights)
            {
                lightsArray.lights.Add(lightInfo[i]);
                newTrackedLights.Add(index);
            }
        }

        for (int i = 0; i < lightInfo.Length; i++)
        {
            if (isOffScreen(lightInfo[i].position))
            {
                continue;
            }
            int index = lightInfo[i].index;
            bool exists = trackedLights.Contains(index);
            if (!exists && lightsArray.lights.Count < maxLights)
            {
                lightsArray.lights.Add(lightInfo[i]);
                newTrackedLights.Add(index);
            }
        }
        trackedLights = newTrackedLights;
    }

    private bool isOffScreen(float3 pos)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(pos.x, pos.y, 1));
        if (screenPoint.x < -0.5 || screenPoint.x > 1.5 || screenPoint.y < -0.5 || screenPoint.y > 1.5)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
