using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    public int chunkSize = 10;
    public int renderBounds = 50;
    public int minObstablesPerChunk = 10;
    public int maxObstablesPerChunk = 30;
    public GameObject obstaclesContainer;
    public int[,] terrainMap;
    public Tilemap tileMap;
    public Tile[] grasses;
    public GameObject[] obstacles;
    public GameObject[] structures;
    public int structureChanceAsPercent = 20;
    Rigidbody2D playerBody;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody2D>();
    }

    Tile GetRandomTile()
    {
        return grasses[Random.Range(0, grasses.Length)];
    }

    void RenderChunkAt(Vector3 position)
    {
        TileBase[] tiles = new TileBase[chunkSize * chunkSize];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = GetRandomTile();
        }
        BoundsInt bounds = new BoundsInt(Vector3Int.FloorToInt(position), new Vector3Int(chunkSize, chunkSize, 1));
        tileMap.SetTilesBlock(bounds, tiles);
        PopulateChunkWithObstables(bounds);
        TryToPlaceStructure(bounds);
    }

    void PopulateChunkWithObstables(BoundsInt bounds)
    {
        int obstacleCount = Random.Range(minObstablesPerChunk, maxObstablesPerChunk+1);
        for(int i = 0; i < obstacleCount; i++)
        {
            Vector3 location = RandomPointInBounds(bounds);
            int index = Random.Range(0, obstacles.Length );
            GameObject obstacle = Instantiate(obstacles[index], location, new Quaternion(), obstaclesContainer.transform);
        }
    }

    void TryToPlaceStructure(BoundsInt bounds)
    {
        int chance = Random.Range(structureChanceAsPercent, 101);
        if(chance == 100)
        {
            Vector3 location = RandomPointInBounds(bounds);
            Debug.Log("Spawning Monolith at " + location);
            int index = Random.Range(0, structures.Length);
            GameObject obstacle = Instantiate(structures[index], location, new Quaternion(), obstaclesContainer.transform);
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
        Vector3 offsetOrigin = new Vector3(playerBody.position.x - (renderBounds/2), playerBody.position.y - (renderBounds/2), 0);
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
            //Debug.Log("Checking corner at " + corner);
            if (!tileMap.HasTile(Vector3Int.FloorToInt(corner)))
            {
                Vector3Int chunkCoordinate = GetChunkCoordinate(corner);
                Debug.Log("rendering chunk at: " + chunkCoordinate);
                RenderChunkAt(chunkCoordinate);
                //break;
            }
        }
    }

    Vector3Int GetChunkCoordinate(Vector3 coords)
    {
        return Vector3Int.FloorToInt(coords / chunkSize) * chunkSize;
    }
}
