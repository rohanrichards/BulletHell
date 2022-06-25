using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    private List<int> trackedLights;
    // this will need to be updated in the shader too
    private int maxLights = 25;
    private LightsCollection lightsArray;
    private class LightData
    {
        public Vector3 world, local;
        public float rad, r, g, b;
    }

    private class LightsCollection
    {
        public List<LightData> lights = new List<LightData>();
        public float[] toFloatArray()
        {
            List<float> res = new List<float>();
            for (int i = 0; i < lights.Count; i++)
            {
                LightData l = lights[i];
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

    // Start is called before the first frame update
    void Awake()
    {
        trackedLights = new List<int>();
        lightsArray = new LightsCollection();
        for(int i = 0; i < maxLights; i++)
        {
            LightData light = new LightData { world = new Vector3(0.0f, 0.0f, 0.0f), rad = 0.0f, r = 0.0f, g = 0.0f, b = 0.0f };
            lightsArray.lights.Add(light);
        }
        Shader.SetGlobalFloatArray("_Lights", lightsArray.toFloatArray());
        Shader.SetGlobalInt("_LightCount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloatArray("_Lights", lightsArray.toFloatArray());
        Shader.SetGlobalInt("_LightCount", lightsArray.lights.Count);
    }

    public void setLights(NativeList<LightManagerSystem.lightInfo> lightInfo)
    {
        lightsArray = new LightsCollection();
        List<int> newTrackedLights = new List<int>();
        for (int i = 0; i < lightInfo.Length; i++)
        {
            int index = lightInfo[i].index;
            bool exists = trackedLights.Contains(index);
            if (exists && lightsArray.lights.Count < maxLights)
            {
                LightData light = new LightData { world = (Vector3)lightInfo[i].position, rad = lightInfo[i].radius, r = lightInfo[i].rgb.x, g = lightInfo[i].rgb.y, b = lightInfo[i].rgb.z };
                lightsArray.lights.Add(light);
                newTrackedLights.Add(index);
            }
        }

        for (int i = 0; i < lightInfo.Length; i++)
        {
            int index = lightInfo[i].index;
            bool exists = trackedLights.Contains(index);
            if (!exists && lightsArray.lights.Count < maxLights)
            {
                LightData light = new LightData { world = (Vector3)lightInfo[i].position, rad = lightInfo[i].radius, r = lightInfo[i].rgb.x, g = lightInfo[i].rgb.y, b = lightInfo[i].rgb.z };
                lightsArray.lights.Add(light);
                newTrackedLights.Add(index);
            }
        }
        trackedLights = newTrackedLights;
    }
}
