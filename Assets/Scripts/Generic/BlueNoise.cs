using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for generating blue noise using Poisson disk sampling.
/// Blue noise creates evenly distributed points with a minimum distance between them.
/// </summary>
public static class BlueNoise
{
    /// <summary>
    /// Generates blue noise points within a bounding box using Poisson disk sampling.
    /// </summary>
    /// <param name="min">The minimum corner of the bounding box.</param>
    /// <param name="max">The maximum corner of the bounding box.</param>
    /// <param name="minDistance">The minimum distance between any two points.</param>
    /// <param name="maxAttempts">Maximum number of attempts to place a point around each active point (default: 30).</param>
    /// <returns>An array of Vector2 points distributed with blue noise characteristics.</returns>
    public static Vector2[] Generate(Vector2 min, Vector2 max, float minDistance, int maxAttempts = 30)
    {
        float width = max.x - min.x;
        float height = max.y - min.y;

        // Cell size should be minDistance / sqrt(2) to ensure only one point per cell
        float cellSize = minDistance / Mathf.Sqrt(2);
        int cols = Mathf.CeilToInt(width / cellSize);
        int rows = Mathf.CeilToInt(height / cellSize);

        // Grid to track which cells contain points (for fast lookup)
        Vector2?[,] grid = new Vector2?[rows, cols];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> activeList = new List<Vector2>();

        // Start with a random initial point
        Vector2 initialPoint = new Vector2(
            min.x + UnityEngine.Random.value * width,
            min.y + UnityEngine.Random.value * height
        );

        points.Add(initialPoint);
        activeList.Add(initialPoint);

        int gridX = Mathf.FloorToInt((initialPoint.x - min.x) / cellSize);
        int gridY = Mathf.FloorToInt((initialPoint.y - min.y) / cellSize);
        grid[gridY, gridX] = initialPoint;

        // Process active list
        while (activeList.Count > 0)
        {
            // Pick a random active point
            int randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * activeList.Count);
            Vector2 point = activeList[randomIndex];
            bool found = false;

            // Try to generate a new point around it
            for (int i = 0; i < maxAttempts; i++)
            {
                // Generate random point in annulus between minDistance and 2*minDistance
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                float radius = minDistance + UnityEngine.Random.value * minDistance;

                Vector2 candidate = new Vector2(
                    point.x + Mathf.Cos(angle) * radius,
                    point.y + Mathf.Sin(angle) * radius
                );

                // Check if candidate is within bounds
                if (candidate.x < min.x || candidate.x >= max.x ||
                    candidate.y < min.y || candidate.y >= max.y)
                {
                    continue;
                }

                // Check if candidate is far enough from existing points
                int candidateGridX = Mathf.FloorToInt((candidate.x - min.x) / cellSize);
                int candidateGridY = Mathf.FloorToInt((candidate.y - min.y) / cellSize);

                if (IsValidPoint(candidate, grid, minDistance, candidateGridX, candidateGridY))
                {
                    points.Add(candidate);
                    activeList.Add(candidate);
                    grid[candidateGridY, candidateGridX] = candidate;
                    found = true;
                    break;
                }
            }

            // If no valid point was found, remove from active list
            if (!found)
            {
                activeList.RemoveAt(randomIndex);
            }
        }

        return points.ToArray();
    }

    /// <summary>
    /// Checks if a candidate point is valid (far enough from all existing points).
    /// </summary>
    /// <param name="candidate">The candidate point to check.</param>
    /// <param name="grid">The grid containing existing points.</param>
    /// <param name="minDistance">The minimum distance between points.</param>
    /// <param name="gridX">The x grid coordinate of the candidate.</param>
    /// <param name="gridY">The y grid coordinate of the candidate.</param>
    /// <returns>True if the point is valid, false otherwise.</returns>
    private static bool IsValidPoint(Vector2 candidate, Vector2?[,] grid, float minDistance, int gridX, int gridY)
    {
        // Check neighboring cells
        int searchRadius = 2; // Check cells within 2 grid cells away

        for (int y = Mathf.Max(0, gridY - searchRadius); y <= Mathf.Min(grid.GetLength(0) - 1, gridY + searchRadius); y++)
        {
            for (int x = Mathf.Max(0, gridX - searchRadius); x <= Mathf.Min(grid.GetLength(1) - 1, gridX + searchRadius); x++)
            {
                Vector2? neighbor = grid[y, x];

                if (neighbor.HasValue)
                {
                    float distance = Vector2.Distance(candidate, neighbor.Value);
                    if (distance < minDistance)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Generates blue noise points with automatic minimum distance based on density. 
    /// Uses ideal hexagonal packing to estimate minDistance.
    /// </summary>
    /// <param name="min">The minimum corner of the bounding box.</param>
    /// <param name="max">The maximum corner of the bounding box.</param>
    /// <param name="targetCount">Approximate number of points to generate.</param>
    /// <param name="maxAttempts">Maximum number of attempts to place a point around each active point (default: 30).</param>
    /// <returns>An array of Vector2 points distributed with blue noise characteristics.</returns>
    public static Vector2[] GenerateWithCount(Vector2 min, Vector2 max, int targetCount, int maxAttempts = 30)
    {
        float width = max.x - min.x;
        float height = max.y - min.y;
        float area = width * height;

        // Calculate minDistance based on target count
        // Area per point = total area / count
        // Assume hexagonal packing: minDistance ≈ sqrt(2 * area / (count * sqrt(3)))
        float minDistance = Mathf.Sqrt((2 * area) / (targetCount * Mathf.Sqrt(3)));

        return Generate(min, max, minDistance, maxAttempts);
    }

    /// <summary>
    /// Generates deterministic blue noise points within a bounding box using Poisson disk sampling.
    /// Uses a seed for reproducible results - same seed and area will always produce the same points.
    /// This is useful for generating consistent points across chunk boundaries in tiled worlds.
    /// </summary>
    /// <param name="min">The minimum corner of the bounding box.</param>
    /// <param name="max">The maximum corner of the bounding box.</param>
    /// <param name="minDistance">The minimum distance between any two points.</param>
    /// <param name="seed">Seed for deterministic random generation.</param>
    /// <param name="maxAttempts">Maximum number of attempts to place a point around each active point (default: 30).</param>
    /// <returns>An array of Vector2 points distributed with blue noise characteristics.</returns>
    public static List<Vector2> Generate(Vector2 min, Vector2 max, float minDistance, int seed, int maxAttempts = 30)
    {
        // Use grid-based spatial hashing for deterministic generation
        // Each cell in world space has its own deterministic seed based on coordinates
        float cellSize = minDistance * 0.7f; // Slightly smaller than minDistance for better distribution
        
        int minCellX = Mathf.FloorToInt(min.x / cellSize);
        int maxCellX = Mathf.FloorToInt(max.x / cellSize);
        int minCellY = Mathf.FloorToInt(min.y / cellSize);
        int maxCellY = Mathf.FloorToInt(max.y / cellSize);
        
        List<Vector2> points = new List<Vector2>();
        
        // Generate candidate points for each cell
        for (int cellY = minCellY; cellY <= maxCellY; cellY++)
        {
            for (int cellX = minCellX; cellX <= maxCellX; cellX++)
            {
                // Create deterministic seed for this cell based on world grid coordinates and global seed
                int cellSeed = HashCoordinates(cellX, cellY, seed);
                System.Random cellRandom = new System.Random(cellSeed);
                
                // Randomly decide if this cell gets a point (probability based on desired density)
                if (cellRandom.NextDouble() < 0.65) // Adjust probability to control density
                {
                    // Generate point at random position within cell
                    float pointX = cellX * cellSize + (float)cellRandom.NextDouble() * cellSize;
                    float pointY = cellY * cellSize + (float)cellRandom.NextDouble() * cellSize;
                    Vector2 candidate = new Vector2(pointX, pointY);
                    
                    // Only include if within bounds
                    if (candidate.x >= min.x && candidate.x < max.x &&
                        candidate.y >= min.y && candidate.y < max.y)
                    {
                        // Check distance to already accepted points
                        bool valid = true;
                        foreach (var p in points)
                        {
                            if (Vector2.Distance(candidate, p) < minDistance)
                            {
                                valid = false;
                                break;
                            }
                        }
                        
                        if (valid)
                        {
                            points.Add(candidate);
                        }
                    }
                }
            }
        }
        
        return points;
    }
    
    /// <summary>
    /// Creates a deterministic hash from cell coordinates and seed.
    /// </summary>
    private static int HashCoordinates(int x, int y, int seed)
    {
        // Use large primes for good distribution
        int hash = seed;
        hash = hash * 73856093 ^ x;
        hash = hash * 19349663 ^ y;
        return hash;
    }

    /// <summary>
    /// Generates deterministic blue noise points with automatic minimum distance based on density.
    /// Uses a seed for reproducible results - same seed and area will always produce the same points.
    /// </summary>
    /// <param name="min">The minimum corner of the bounding box.</param>
    /// <param name="max">The maximum corner of the bounding box.</param>
    /// <param name="targetCount">Approximate number of points to generate.</param>
    /// <param name="seed">Seed for deterministic random generation.</param>
    /// <param name="maxAttempts">Maximum number of attempts to place a point around each active point (default: 30).</param>
    /// <returns>An array of Vector2 points distributed with blue noise characteristics.</returns>
    public static List<Vector2> GenerateWithCount(Vector2 min, Vector2 max, int targetCount, int seed, int maxAttempts = 30)
    {
        float width = max.x - min.x;
        float height = max.y - min.y;
        float area = width * height;

        // Calculate minDistance based on target count
        // Area per point = total area / count
        // Assume hexagonal packing: minDistance ≈ sqrt(2 * area / (count * sqrt(3)))
        float minDistance = Mathf.Sqrt((2 * area) / (targetCount * Mathf.Sqrt(3)));

        return Generate(min, max, minDistance, seed, maxAttempts);
    }
}
