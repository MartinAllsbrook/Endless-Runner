using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    Vector2[] points;
    Vector2 chunkPosition;
    float chunkSize;
    WorldObject[] objects;

    bool initialized = false;
    public void Initialize(Vector2 _chunkPosition, float _chunkSize, int pointCount, float minDistance)
    {
        if (initialized) return;

        initialized = true;

        chunkPosition = _chunkPosition;
        chunkSize = _chunkSize;

        transform.position = new Vector3(chunkPosition.x, chunkPosition.y, 0f);
        
        // Generate points in local space (0 to chunkSize) since chunk transform is already positioned
        points = GenerateBlueNoisePoints(Vector2.zero, chunkSize, pointCount, minDistance);
        objects = new WorldObject[points.Length];
    
        PlaceObjects();
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

    void PlaceObjects()
    {
        for (int i = 0; i < points.Length; i++)
        {
            objects[i] = World.Instance.PlaceObjectAt(points[i], transform);
        }
    }

    void DestroyObjects()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].ReturnToPool();
            }
        }
    }

    // Simple blue noise (Poisson Disk Sampling) implementation
    private Vector2[] GenerateBlueNoisePoints(Vector2 origin, float size, int count, float minDist)
    {
        List<Vector2> points = new List<Vector2>();
        int maxAttempts = 30;
        Rect bounds = new Rect(origin, new Vector2(size, size));
        System.Random rand = new System.Random();

        // Start with a random point
        Vector2 firstPoint = new Vector2(
            (float)(origin.x + rand.NextDouble() * size),
            (float)(origin.y + rand.NextDouble() * size)
        );
        points.Add(firstPoint);

        int attempts = 0;
        while (points.Count < count && attempts < count * maxAttempts)
        {
            Vector2 candidate = new Vector2(
                (float)(origin.x + rand.NextDouble() * size),
                (float)(origin.y + rand.NextDouble() * size)
            );
            bool valid = true;
            foreach (var p in points)
            {
                if (Vector2.Distance(candidate, p) < minDist)
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
            {
                points.Add(candidate);
            }
            attempts++;
        }
        return points.ToArray();
    }
}