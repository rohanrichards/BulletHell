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
        Vector3 randomVector = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion randomRotation = Quaternion.Euler(randomVector);
        GameObject newPrefab = Instantiate(prefabs[index], transform.position, randomRotation, transform);
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
