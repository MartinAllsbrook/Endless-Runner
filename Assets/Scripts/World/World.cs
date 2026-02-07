using UnityEngine;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour
{
    public static World Instance;
    public static event Action OnWorldLoaded = delegate { };

    [Header("Chunk")]
    [SerializeField] Chunk chunkPrefab;
    [SerializeField] float chunkSize = 20f;
    
    [Header("Tunnel")]
    [SerializeField] Transform tunnelPrefab;
    [SerializeField] float tunnelDistance = 500f;

    [Header("Seed")]
    [SerializeField] int seed = 0;

    ObjectPool<WorldObject> worldObjectPool;
    Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();
    Dictionary<Vector2Int, Chunk> allChunks = new Dictionary<Vector2Int, Chunk>();

    float lastSpawnY = float.MinValue;
    float lastEnemySpawnY = float.MinValue;
    float spawnDistance = 5f;
    float enemySpawnDistance = 8f;

    void Awake()
    {
        if (seed == 0)
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Place tunnel
        PlaceTunnel();

        OnWorldLoaded.Invoke();
    }

    void Update()
    {
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(Player.Instance.transform.position);

        // Calculate 3x3 grid around player
        HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                requiredChunks.Add(playerChunkCoord + new Vector2Int(x, y));
            }
        }

        // Activate/create required chunks
        foreach (var coord in requiredChunks)
        {
            if (!activeChunks.ContainsKey(coord))
            {
                Chunk chunk = GetOrCreateChunk(coord);
                chunk.gameObject.SetActive(true);
                activeChunks[coord] = chunk;
            }
        }

        // Deactivate chunks outside 3x3 grid
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!requiredChunks.Contains(kvp.Key))
            {
                kvp.Value.Cleanup();
                kvp.Value.gameObject.SetActive(false);
                chunksToRemove.Add(kvp.Key);
            }
        }

        foreach (var coord in chunksToRemove)
        {
            activeChunks.Remove(coord);
        }
    }

    void PlaceTunnel()
    {
        System.Random random = new System.Random(seed);

        float angle = (float)(random.NextDouble() * Mathf.PI * 2f);
        float randomZRotation = (float)(random.NextDouble() * 360f);

        Vector2 position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * tunnelDistance;
        Quaternion rotation = Quaternion.Euler(0f, 0f, randomZRotation);

        Instantiate(tunnelPrefab, new Vector3(position.x, position.y, 0f), rotation, transform);
    }

    Vector2Int GetChunkCoordFromPosition(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    Chunk GetOrCreateChunk(Vector2Int coord)
    {
        // Check if chunk already exists at this coordinate
        if (allChunks.TryGetValue(coord, out Chunk existingChunk))
        {
            return existingChunk;
        }

        // Create new chunk if it doesn't exist
        Chunk chunk = Instantiate(chunkPrefab, transform);
        Vector2 position = new Vector2(coord.x * chunkSize, coord.y * chunkSize);        
        chunk.Initialize(position, chunkSize); // TODO: Initialize with proper parameters
        
        // Store in allChunks dictionary
        allChunks[coord] = chunk;

        return chunk;
    }

    public WorldObject PlaceObjectAt(Vector2 position, Transform parent)
    {
        WorldObject obj = worldObjectPool.Get(position, parent);
        return obj;
    }
}

