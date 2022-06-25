using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [HideInInspector]
    public GameObject testingPrefab;
    protected EntityManager entityManager;
    protected Entity testingEntityPrefab;
    protected BlobAssetStore blobAssetStore;

    [Header("Testing Settings")]
    public bool testing = false;
    public int testEnemyIndex = 3;
    public int testClusterCount = 5;
    public int testGridSize = 3;

    [Header("Spawning Settings")]
    public float baseWaveSpeed = 5.0f;
    public float minWaveSpeed = 1.0f;
    public int baseClusterSize = 3;
    public int maxClusterSize = 10;
    public int baseClusterCount = 2;
    public int maxClusterCount = 10;
    public int spawnRadius = 30;
    public int despawnRadius = 45;
    public GameObject[] availableEnemies;
    public List<Entity> availableEnemyEntities = new List<Entity>();
    public GameObject enemyContainer;

    [Header("Debug Info")]
    public int totalEnemies = 0;
    public AnimationCurve vizualizedSpawnDitribution;

    private DifficultyManager difficultyManager;

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
        difficultyManager = gameObject.GetComponent<DifficultyManager>();

        blobAssetStore = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int i = 0; i < availableEnemies.Length; i++)
        {
            GameObject prefab = availableEnemies[i];
            availableEnemyEntities.Add(GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore)));
        }
        testingEntityPrefab = availableEnemyEntities[testEnemyIndex];

        StartCoroutine(DelayedStart());
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1);

        if (testing)
        {
            GenerateTestEntities(testClusterCount);
        }
        else
        {
            StartCoroutine(GenerateEntityEnemies());
        }
    }

    void GenerateTestEntities(int size)
    {
        EnemyGeneratorSystem generatorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EnemyGeneratorSystem>();
        NativeArray<Translation> locations = generatorSystem.GetClusterLocations(size, spawnRadius);

        for (int i = 0; i < locations.Length; i++)
        {
            SpawnTestEntityCluster(testGridSize, locations[i]);
        }

        locations.Dispose();
    }


    void SpawnTestEntityCluster(int count, Translation clusterLocation)
    {
        NativeArray<Entity> enemies = new NativeArray<Entity>(count*count, Allocator.TempJob);
        entityManager.Instantiate(testingEntityPrefab, enemies);
        int padding = 1;
        int width = count, height = count;

        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 location = new Vector3(clusterLocation.Value.x + x + (x * padding), clusterLocation.Value.y + y + (y * padding), 0);
                entityManager.SetComponentData(enemies[index], new Translation { Value = location });
                index++;
            }
        }

        enemies.Dispose();
    }

    IEnumerator GenerateEntityEnemies(int clusterOverride = 0)
    {
        int clustersToSpawn = clusterOverride == 0 ? ClusterCount : clusterOverride;

        EnemyGeneratorSystem generatorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EnemyGeneratorSystem>();
        NativeArray<Translation> locations = generatorSystem.GetClusterLocations(clustersToSpawn, spawnRadius);

        for (int i = 0; i < locations.Length; i++)
        {
            SpawnEntityCluster(locations[i]);
        }

        locations.Dispose();
        yield return new WaitForSeconds(WaveSpeed);
        StartCoroutine(GenerateEntityEnemies());
    }

    void SpawnEntityCluster(Translation clusterLocation)
    {
        bool super = UnityEngine.Random.value > 0.98;
        int enemyCount = super ? 1 : ClusterSize;
        int enemyPrefabIndex = PickEnemyForCluster();

        // create the entities
        NativeArray<Entity> enemies = new NativeArray<Entity>(enemyCount, Allocator.TempJob);
        entityManager.Instantiate(availableEnemyEntities[enemyPrefabIndex], enemies);

        // set their positions
        float shellDistance = 1.0f;
        float shellSpacing = 1.0f;
        int shell = 0;
        int shellIndex = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            float radius = shell * shellDistance;
            float circumference = radius * (2 * Mathf.PI);
            int numberInShell = Mathf.Min(Mathf.FloorToInt(circumference / shellSpacing), enemies.Length - i + shellIndex + 1);
            if(numberInShell == 0)
            {
                numberInShell = 1;
            }
            
            float frac = (float)shellIndex / (float)numberInShell;
            float angle = frac * (2 * Mathf.PI);
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;

            float3 location = new Vector3(clusterLocation.Value.x + x, clusterLocation.Value.y + y, 0);
            entityManager.SetComponentData(enemies[i], new Translation { Value = location });

            if(entityManager.HasComponent(enemies[i], typeof(EntityTargetSettings)))
            {
                float3 targetMoveDirection = ECSPlayerController.getPlayerLocation().Position - location;
                entityManager.SetComponentData(enemies[i], new EntityTargetSettings { targetMovementDirection = targetMoveDirection});
                Quaternion originRotation = Quaternion.AngleAxis(Mathf.Atan2(targetMoveDirection.y, targetMoveDirection.x), new Vector3(0, 0, 1));
                entityManager.SetComponentData(enemies[i], new Rotation { Value = originRotation});
                entityManager.AddComponent(enemies[i], typeof(LightTag));
            }

            shellIndex++;
            if(shellIndex >= numberInShell)
            {
                shellIndex = 0;
                shell++;
            }
        }

        enemies.Dispose();
    }

    int PickEnemyForCluster()
    {
        int totalEnemies = availableEnemies.Length;
        List<GameObject> selectedEnemies = new List<GameObject>();
        AnimationCurve curve = new AnimationCurve();
        for(int i = 0; i < totalEnemies; i++)
        {
            float weight = availableEnemies[i].GetComponent<EnemyBase>().GetLikelihoodWeight(difficultyManager.GetDifficultyMod());
            if(weight > 0)
            {
                selectedEnemies.Add(availableEnemies[i]);
            }
        }

        totalEnemies = selectedEnemies.Count;
        for (int i = 0; i < totalEnemies; i++)
        {
            float weight = selectedEnemies[i].GetComponent<EnemyBase>().GetLikelihoodWeight(difficultyManager.GetDifficultyMod());
            Keyframe key = new Keyframe((i + 1) / totalEnemies, (i + 1) / totalEnemies);
            key.weightedMode = WeightedMode.Both;
            key.inWeight = weight;
            key.outWeight = weight;
            curve.AddKey(key);
        }

        if (selectedEnemies.Count == 1)
        {
            return Array.FindIndex(availableEnemies, e => e == selectedEnemies[0]);
        };

        vizualizedSpawnDitribution = curve;
        float point = curve.Evaluate(UnityEngine.Random.value);
        int index = Mathf.RoundToInt(point * (selectedEnemies.Count-1));
        //string log = FormattableString.Invariant($"point: {point}, selectedEnemies: {selectedEnemies.Count}, index: {index}");
        //Debug.Log(log);
        return Array.FindIndex(availableEnemies, e => e == selectedEnemies[index]);
    }

    void SpawnSuper(Vector2 location, GameObject prefab)
    {
/*        GameObject newEnemy = EnemyBase.Create(prefab, location, enemyContainer.transform);
        newEnemy.transform.position = location;
        SpriteRenderer[] sprites = newEnemy.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer r in sprites)
        {
            r.color = new Color(255, 0, 0);
        }
        newEnemy.transform.localScale = newEnemy.transform.localScale * 2;
        EnemyBase controller = newEnemy.GetComponent<EnemyBase>();
        Rigidbody2D body = newEnemy.GetComponentInChildren<Rigidbody2D>();
        controller.isSuper = true;
        controller.config.XPValue *= 10;
        controller.config.baseHealth += controller.config.baseHealth * 10;
        controller.config.currentHealth = controller.config.baseHealth;
        controller.config.moveSpeed *= 2f;
        body.mass *= 2;*/
    }

    IEnumerator CheckForOutOfRange()
    {
/*        int despawned = 0;
        EnemyBase[] enemies = GameObject.FindObjectsOfType<EnemyBase>();
        foreach(EnemyBase enemy in enemies)
        {
            Rigidbody2D rb = enemy.rb;
            Vector3 distance = rb.transform.position - playerBody.transform.position;
            if(distance.magnitude > despawnRadius)
            {
                if (enemy.isSuper)
                {
                    Destroy(enemy.gameObject);
                    despawned += 10;
                }
                else
                {
                    EnemyBase.SendToPool(enemy.gameObject);
                    despawned++;
                }
            }
        }
        int clustersToSpawn = Mathf.CeilToInt(despawned / ClusterSize);
        if(clustersToSpawn > 0)
        {
            StartCoroutine(SpawnEnemies(clustersToSpawn));
        }*/
        yield return new WaitForSeconds(1); 
        StartCoroutine(CheckForOutOfRange());
    }

}
