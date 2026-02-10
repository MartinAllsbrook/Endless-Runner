using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

class POIManager : MonoBehaviour
{
    public static POIManager Instance { get; private set; }
    public static event Action<PointOfInterest[]> OnPOIsSpawned;
    public PointOfInterest[] POIs => pointsOfInterest;

    [Header("Prefabs")]
    [SerializeField] TunnelPOI tunnelPrefab;
    [SerializeField] PointOfInterest[] poiPrefabs;
    [SerializeField] Road roadPrefab;

    [Header("Settings")]
    [SerializeField] float spawnAnnulusInnerRadius = 50f;
    [SerializeField] float spawnAnnulusOuterRadius = 1000f;
    [SerializeField] float tunnelSpawnAnnulusInnerRadius = 450f;
    [SerializeField] float tunnelSpawnAnnulusOuterRadius = 650f;

    PointOfInterest[] pointsOfInterest = new PointOfInterest[0];

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    public void SpawnPOIs()
    {
        List<PointOfInterest> poiList = new List<PointOfInterest>(); 

        // Start by spawning the tunnel
        TunnelPOI tunnelPOI = SpawnTunnel();
        poiList.Add(tunnelPOI);

        // Then spawn other POIs
        foreach (var poiPrefab in poiPrefabs)
        {
            for (int i = 0; i < poiPrefab.CountInWorld; i++)
            {
                PointOfInterest newPOI = SpawnPOI(poiPrefab, poiList);
                if (newPOI != null)
                    poiList.Add(newPOI);
            }
        }

        pointsOfInterest = poiList.ToArray();
        OnPOIsSpawned?.Invoke(pointsOfInterest);

        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        foreach (var poi in pointsOfInterest)
        {
            if (poi is TunnelPOI tunnel)
                continue; // Skip tunnel for closest POI calculation

            float distance = Vector3.Distance(tunnelPOI.transform.position, poi.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = Array.IndexOf(pointsOfInterest, poi);
            }
        }

        Road road = Instantiate(roadPrefab);
        road.GenerateRoad(tunnelPOI.RoadConnectionPoints[0].position, tunnelPOI.RoadConnectionPoints[0].up, pointsOfInterest[closestIndex].RoadConnectionPoints[0].position, -pointsOfInterest[closestIndex].RoadConnectionPoints[0].up);
    }

    TunnelPOI SpawnTunnel()
    {
        Vector3 spawnPosition = GetRandomPositionInAnnulus(tunnelSpawnAnnulusInnerRadius, tunnelSpawnAnnulusOuterRadius);
        TunnelPOI tunnel = Instantiate(tunnelPrefab, spawnPosition, Quaternion.identity);
        return tunnel;
    }

    PointOfInterest SpawnPOI(PointOfInterest poiPrefab, List<PointOfInterest> poiList)
    {
        for (int attempt = 0; attempt < 30; attempt++)
        {
            Vector3 spawnPosition = GetRandomPositionInAnnulus(spawnAnnulusInnerRadius, spawnAnnulusOuterRadius);

            if (IsPositionValid(spawnPosition, poiPrefab.Radius, poiList))
            {
                PointOfInterest newPOI = Instantiate(poiPrefab, spawnPosition, Quaternion.identity);
                return newPOI;
            }
        }
        Debug.LogWarning($"Failed to spawn POI of type {poiPrefab.name} after multiple attempts.");
        return null;
    }

    bool IsPositionValid(Vector3 position, float radius, List<PointOfInterest> poiList)
    {
        foreach (var existingPOI in poiList)
        {
            float sumOfRadii = radius + existingPOI.Radius;
            float distance = Vector3.Distance(position, existingPOI.transform.position);
            
            if (distance < sumOfRadii) // Minimum distance between POIs
            {
                return false;
            }
        }
        return true;
    }

    Vector3 GetRandomPositionInAnnulus(float innerRadius, float outerRadius)
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        float randomDistance = UnityEngine.Random.Range(innerRadius, outerRadius);
        Vector3 spawnPosition = Player.Instance.transform.position + new Vector3(randomDirection.x, randomDirection.y) * randomDistance;
        return spawnPosition;
    }
}