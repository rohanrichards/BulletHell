using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabRandomizer : MonoBehaviour
{
    public GameObject[] prefabs;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }


        int index = Random.Range(0, prefabs.Length);
        GameObject newPrefab = Instantiate(prefabs[index], transform);
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
