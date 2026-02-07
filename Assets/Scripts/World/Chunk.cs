using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    [SerializeField] Transform backgroundTransform;

    Dictionary<ScatterTag, Vector2[]> scatterPoints = new Dictionary<ScatterTag, Vector2[]>();
    Vector2 chunkPosition;
    float chunkSize;
    WorldObject[] scatteredObjects;

    bool initialized = false;
    public void Initialize(Vector2 _chunkPosition, float _chunkSize)
    {
        // Prevent double initialization
        if (initialized) return;
        initialized = true;

        // Store chunk position and size
        chunkPosition = _chunkPosition;
        chunkSize = _chunkSize;

        // Position and scale background
        transform.position = new Vector3(chunkPosition.x, chunkPosition.y, 0f);
        backgroundTransform.localScale = new Vector3(chunkSize, chunkSize, 1f);
        backgroundTransform.localPosition = new Vector3(chunkSize / 2f, chunkSize / 2f, 1f);    

        // Generate scatter points for this chunk
        GeneratePoints(ScatterManager.Instance.scatterSettings);
        PlaceObjects();

        SpawnEnemies();

    }

    void OnEnable()
    {
        if (!initialized) return;

        PlaceObjects();
    }

    public void Cleanup()
    {
        DestroyObjects();
    }

    void GeneratePoints(ScatterSettings[] scatterSettings)
    {
        int numPoints = 0;

        foreach (var setting in scatterSettings)
        {
            // Generate points for this scatter type
            Vector2[] points = BlueNoise.GenerateWithCount(chunkPosition, chunkPosition + Vector2.one * chunkSize, setting.targetDensity);
            scatterPoints[setting.tag] = points;
     
            numPoints += points.Length;
        }

        scatteredObjects = new WorldObject[numPoints];
    }

    void SpawnEnemies()
    {
        Vector2[] points = BlueNoise.GenerateWithCount(chunkPosition, chunkPosition + Vector2.one * chunkSize, 5);
        foreach (var point in points)
        {
            EnemyManager.Instance.SpawnEnemyAt(point);
        }
    }

    void PlaceObjects()
    {
        int index = 0;
        foreach (var kvp in scatterPoints)
        {
            ScatterTag tag = kvp.Key;
            Vector2[] points = kvp.Value;

            foreach (var point in points)
            {
                scatteredObjects[index] = ScatterManager.Instance.SpawnScatter(tag, point);
                index++;
            }
        }
    }

    void DestroyObjects()
    {
        for (int i = 0; i < scatteredObjects.Length; i++)
        {
            if (scatteredObjects[i] != null)
            {
                scatteredObjects[i].ReturnToPool();
            }
        }
    }
}