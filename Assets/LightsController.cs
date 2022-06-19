using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    private float _Tick = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float ang = _Tick * 0.01f;
        float rad = 0.4f;

        int num_lights = (((int)_Tick / 100) % 2) == 0 ? 1 : 2;

        float[] array = new float[] {
            0.5f, 0.5f, 3.0f,
            1.0f, 1.0f, 1.0f,

            0.5f + Mathf.Sin(ang) * rad, 0.5f + Mathf.Cos(ang) * rad, 1.5f,
            Mathf.Abs(Mathf.Sin(ang * 5.0f)), 0.0f, 1.0f
        };
        Shader.SetGlobalFloatArray("_Lights", array);
        Shader.SetGlobalInt("_LightCount", num_lights);

        _Tick++;
    }
}
