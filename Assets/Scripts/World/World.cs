using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public static World Instance;

    [SerializeField] WorldObject worldObjectPrefab;
    [SerializeField] float chunkSize = 20f;
    [SerializeField] Chunk chunkPrefab;
    [SerializeField] int poolSize = 20;

    ObjectPool<WorldObject> worldObjectPool;
    Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();
    Dictionary<Vector2Int, Chunk> allChunks = new Dictionary<Vector2Int, Chunk>();

    float lastSpawnY = float.MinValue;
    float lastEnemySpawnY = float.MinValue;
    float spawnDistance = 5f;
    float enemySpawnDistance = 8f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize pool using ObjectPool<T> API
        worldObjectPool = new ObjectPool<WorldObject>(worldObjectPrefab, poolSize, this.transform);
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
        chunk.Initialize(position, chunkSize, 50, 1.5f);
        
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

