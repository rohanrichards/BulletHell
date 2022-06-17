using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

//[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    [Header("Worldgen Toggles")]
    public bool generateGround = true;
    public bool generateObstacles = true;
    public bool generateStructures = true;
    public bool generateDestroyables = true;

    [Header("Worldgen Settings")]
    public int chunkSize = 10;
    public int renderBounds = 50;
    public int minObstablesPerChunk = 10;
    public int maxObstablesPerChunk = 30;
    public int minDestroyablesPerChunk = 1;
    public int maxDestroyablesPerChunk = 3;
    public int structureChanceAsPercent = 20;

    [Header("Config")]
    public GameObject obstaclesContainer;
    public Tilemap tileMap;

    [Header("Provided Assets")]
    public Tile[] grassTiles;
    public GameObject[] obstaclesPrefabs;
    public GameObject[] structurePrefabs;
    public GameObject[] destroyablePrefabs;

    public int[,] terrainMap;

    void Start()
    {
    }

    Tile GetRandomTile()
    {
        return grassTiles[Random.Range(0, grassTiles.Length)];
    }

    void RenderChunkAt(Vector3 position)
    {
        BoundsInt bounds = new BoundsInt(Vector3Int.FloorToInt(position), new Vector3Int(chunkSize, chunkSize, 1));
        if (generateGround)
        {
            TileBase[] tiles = new TileBase[chunkSize * chunkSize];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = GetRandomTile();
            }
            tileMap.SetTilesBlock(bounds, tiles);
        }
        if (generateObstacles)
        {
            PopulateChunkWithObstables(bounds);
        }
        if (generateStructures)
        {
            TryToPlaceStructure(bounds);
        }
        if (generateDestroyables)
        {
            TryToPlaceDestroyables(bounds);
        }

    }

    void PopulateChunkWithObstables(BoundsInt bounds)
    {
        int obstacleCount = Random.Range(minObstablesPerChunk, maxObstablesPerChunk + 1);
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 location = RandomPointInBounds(bounds);
            int index = Random.Range(0, obstaclesPrefabs.Length);
            GameObject obstacle = Instantiate(obstaclesPrefabs[index], location, new Quaternion(), obstaclesContainer.transform);
        }
    }

    void TryToPlaceStructure(BoundsInt bounds)
    {
        float chance = Random.value;
        if(chance <= structureChanceAsPercent / 100.0f)
        {
            Vector3 location = RandomPointInBounds(bounds);
            int index = Random.Range(0, structurePrefabs.Length);
            GameObject obstacle = Instantiate(structurePrefabs[index], location, new Quaternion(), obstaclesContainer.transform);
        }
    }

    void TryToPlaceDestroyables(BoundsInt bounds)
    {
        int count = Random.Range(minDestroyablesPerChunk, maxDestroyablesPerChunk + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 location = RandomPointInBounds(bounds);
            int index = Random.Range(0, destroyablePrefabs.Length);
            GameObject obstacle = Instantiate(destroyablePrefabs[index], location, new Quaternion(), obstaclesContainer.transform);
        }
    }

    Vector3 RandomPointInBounds(BoundsInt bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
            );
    }

    // Update is called once per frame
    void Update()
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        Vector3 offsetOrigin = new Vector3(playerLocation.Position.x - (renderBounds/2), playerLocation.Position.y - (renderBounds/2), 0);
        BoundsInt bounds = new BoundsInt(Vector3Int.FloorToInt(offsetOrigin), new Vector3Int(renderBounds, renderBounds, 0));
        List<Vector3> corners = new List<Vector3>();
        // bl, tl, tr, br
        corners.Add(new Vector3(bounds.min.x, bounds.min.y, 0));
        corners.Add(new Vector3(bounds.min.x, bounds.max.y, 0));
        corners.Add(new Vector3(bounds.max.x, bounds.max.y, 0));
        corners.Add(new Vector3(bounds.max.x, bounds.min.y, 0));

        bool debug = true;
        if (debug)
        {
            Debug.DrawLine(corners[0], corners[1], Color.magenta);
            Debug.DrawLine(corners[1], corners[2], Color.magenta);
            Debug.DrawLine(corners[2], corners[3], Color.magenta);
            Debug.DrawLine(corners[3], corners[0], Color.magenta);
        }

        foreach(Vector3 corner in corners)
        {
            if (!tileMap.HasTile(Vector3Int.FloorToInt(corner)))
            {
                Vector3Int chunkCoordinate = GetChunkCoordinate(corner);
                RenderChunkAt(chunkCoordinate);
            }
        }
    }

    Vector3Int GetChunkCoordinate(Vector3 coords)
    {
        return Vector3Int.FloorToInt(coords / chunkSize) * chunkSize;
    }
}
