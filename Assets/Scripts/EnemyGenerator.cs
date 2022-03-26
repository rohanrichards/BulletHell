using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public float baseWaveSpeed = 5.0f;
    public float minWaveSpeed = 1.0f;
    public int baseClusterSize = 3;
    public int maxClusterSize = 10;
    public int baseClusterCount = 2;
    public int maxClusterCount = 10;
    public int spawnRadius = 30;
    public int clusterRadius = 3;
    public int despawnRadius = 45;
    public int maxEnemies = 3000;
    public GameObject[] availableEnemies;
    public GameObject enemyContainer;
    private GameObject player;
    private Rigidbody2D playerBody;
    public int totalEnemies = 0;
    public AnimationCurve vizualizedSpawnDitribution;
    private DifficultyManager difficultyManager;
    // Start is called before the first frame update

    public float WaveSpeed
    {
        get { return baseWaveSpeed - ((baseWaveSpeed - minWaveSpeed) * difficultyManager.GetDifficultyMod()); }
    }
    public int ClusterSize
    {
        get { return Mathf.FloorToInt(baseClusterSize + (maxClusterSize * difficultyManager.GetDifficultyMod())); }
    }
    public int ClusterCount
    {
        get { return Mathf.FloorToInt(baseClusterCount + (maxClusterCount * difficultyManager.GetDifficultyMod())); }

    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerBody = player.GetComponentInChildren<Rigidbody2D>();
        difficultyManager = gameObject.GetComponent<DifficultyManager>();

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1);

        StartCoroutine(SpawnEnemies());
        StartCoroutine(CheckForOutOfRange());

        //GenerateTestEnemies();
        //SpawnCluster(new Vector2(10, 10), false);
    }

    void GenerateTestEnemies()
    {
        int width = 50;
        int height = 50;
        int padding = 2;

        for(int x = 0; x<width; x+= padding)
        {
            for (int y = 0; y < height; y+= padding)
            {
                Vector2 location = new Vector2(x, y);
                SpawnEnemy(location, availableEnemies[0]);
            }
        }
    }

    private void LateUpdate()
    {
        totalEnemies = GameObject.FindObjectsOfType<EnemyBase>().Length;
    }

    IEnumerator SpawnEnemies(int clusterOverride = 0)
    {
        if(totalEnemies < maxEnemies)
        {
            int clustersToSpawn = clusterOverride == 0 ? ClusterCount : clusterOverride;
            for (int i = 0; i < clustersToSpawn; i++)
            {
                int retry = 0;
                bool super = UnityEngine.Random.value > 0.98;

                while (retry < 3)
                {
                    int clusterCircle = spawnRadius + (retry * 10);
                    Vector2 clusterLocation = (Vector2)player.GetComponentInChildren<Rigidbody2D>().transform.position + RandomPointOnCircleEdge(clusterCircle);
                    Collider2D match = Physics2D.OverlapCircle(clusterLocation, clusterRadius);

                    if (match)
                    {
                        //Debug.Log("Failed to spawn cluster, expanding zone");
                        retry++;
                    }
                    else
                    {
                        SpawnCluster(clusterLocation, super);
                        break;
                    }
                }
            }
        }
        if(clusterOverride == 0)
        {
            yield return new WaitForSeconds(WaveSpeed);
            StartCoroutine(SpawnEnemies());
        }
}

    void SpawnCluster(Vector2 location, bool super)
    {
        int retry = 0;
        GameObject enemyPrefab = PickEnemyForCluster();
        float weight = enemyPrefab.GetComponent<GenericEnemy>().GetLikelihoodWeight(difficultyManager.GetDifficultyMod());
        int enemyCount;
        int spawnCircle;

        if (super)
        {
            enemyCount = 1;
        }else
        {
            enemyCount = ClusterSize;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            spawnCircle = 0;
            while(retry < 10)
            {
                spawnCircle += retry/3;
                Vector2 spawnLocation = location + RandomPointInsideCircle(spawnCircle);
                float checkRadius = enemyPrefab.GetComponentInChildren<CircleCollider2D>().radius; 
                Collider2D[] matches = Physics2D.OverlapCircleAll(spawnLocation, checkRadius);

                if (matches.Length > 0)
                {
                    //Debug.DrawLine(new Vector2(spawnLocation.x - checkRadius, spawnLocation.y - checkRadius), new Vector2(spawnLocation.x + checkRadius, spawnLocation.y + checkRadius), Color.red, 10000);
                    //Debug.Log("Something at this location already (retry " + retry + ")");
                    retry++;
                    continue;
                }
                else
                {
                    if (super)
                    {
                        SpawnSuper(spawnLocation, enemyPrefab);
                    }else
                    {
                        SpawnEnemy(spawnLocation, enemyPrefab);
                    }
                    retry = 0;
                    break;
                }
            }
        }
    }

    GameObject PickEnemyForCluster()
    {
        int totalEnemies = availableEnemies.Length;
        List<GameObject> selectedEnemies = new List<GameObject>();
        AnimationCurve curve = new AnimationCurve();
        for(int i = 0; i < totalEnemies; i++)
        {
            float weight = availableEnemies[i].GetComponent<GenericEnemy>().GetLikelihoodWeight(difficultyManager.GetDifficultyMod());
            if(weight > 0)
            {
                selectedEnemies.Add(availableEnemies[i]);
            }
        }

        totalEnemies = selectedEnemies.Count;
        for (int i = 0; i < totalEnemies; i++)
        {
            float weight = selectedEnemies[i].GetComponent<GenericEnemy>().GetLikelihoodWeight(difficultyManager.GetDifficultyMod());
            Keyframe key = new Keyframe((i + 1) / totalEnemies, (i + 1) / totalEnemies);
            key.weightedMode = WeightedMode.Both;
            key.inWeight = weight;
            key.outWeight = weight;
            curve.AddKey(key);
        }

        if (selectedEnemies.Count == 1) return selectedEnemies[0];

        vizualizedSpawnDitribution = curve;
        float point = curve.Evaluate(UnityEngine.Random.value);
        int index = Mathf.RoundToInt(point * (selectedEnemies.Count-1));
        string log = FormattableString.Invariant($"point: {point}, selectedEnemies: {selectedEnemies.Count}, index: {index}");
        Debug.Log(log);
        return selectedEnemies[index];
    }

    void SpawnSuper(Vector2 location, GameObject prefab)
    {
        GameObject newEnemy = EnemyBase.Create(prefab, location, enemyContainer.transform);
        newEnemy.transform.position = location;
        SpriteRenderer[] sprites = newEnemy.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer r in sprites)
        {
            r.color = new Color(255, 0, 0);
        }
        newEnemy.transform.localScale = newEnemy.transform.localScale * 2;
        EnemyBase controller = newEnemy.GetComponent<EnemyBase>();
        controller.isSuper = true;
        controller.config.XPValue *= 10;
        controller.config.baseHealth += controller.config.baseHealth * 10;
        controller.config.moveSpeed *= 2;
    }

    void SpawnEnemy(Vector2 location, GameObject prefab)
    {
        GameObject newEnemy = EnemyBase.Create(prefab, location, enemyContainer.transform);
        Rigidbody2D body = newEnemy.GetComponentInChildren<Rigidbody2D>();
        totalEnemies++;
    }

    IEnumerator CheckForOutOfRange()
    {
        int despawned = 0;
        EnemyBase[] enemies = GameObject.FindObjectsOfType<EnemyBase>();
        foreach(EnemyBase enemy in enemies)
        {
            Rigidbody2D rb = enemy.rb;
            Vector3 distance = rb.transform.position - playerBody.transform.position;
            if(distance.magnitude > despawnRadius)
            {
                Destroy(enemy.gameObject);
                despawned++;
            }
        }
        int clustersToSpawn = Mathf.CeilToInt(despawned / ClusterSize);
        if(clustersToSpawn > 0)
        {
            Debug.Log("enemies to spawn: " + clustersToSpawn);
            StartCoroutine(SpawnEnemies(clustersToSpawn));
        }
        yield return new WaitForSeconds(1); 
        StartCoroutine(CheckForOutOfRange());
    }

    Vector2 RandomPointOnCircleEdge(float radius)
    {
        Vector2 point =  UnityEngine.Random.insideUnitCircle.normalized * radius;
        return new Vector2(point.x, point.y);
    }

    Vector2 RandomPointInsideCircle(float radius)
    {
        Vector2 point = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector2(point.x, point.y);
    }

}