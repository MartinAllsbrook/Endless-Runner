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

    TunnelPOI exitTunnel;
    TunnelPOI goalTunnel;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    public void Generate()
    {
        SpawnPOIs();
        BuildRoads();
    }

    void SpawnPOIs()
    {
        List<PointOfInterest> poiList = new List<PointOfInterest>(); 

        // Start by spawning the tunnels
        exitTunnel = SpawnExitTunnel();
        poiList.Add(exitTunnel);

        goalTunnel = SpawnGoalTunnel();
        poiList.Add(goalTunnel);

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
    }

    void BuildRoads()
    {
        int numberOfRoadsToBuild = 0;
        foreach (var poi in pointsOfInterest)
        {
            numberOfRoadsToBuild += poi.ConnectionPoints.Length;
        }
        numberOfRoadsToBuild /= 2; // Each road connects 2 points, so divide by 2 to avoid double counting

        for (int i = 0; i < numberOfRoadsToBuild; i++)
        {
            Transform[] connectionPoints = FindClosestConnectionPoints();
            if (connectionPoints != null)
            {
                Road newRoad = Instantiate(roadPrefab);
                newRoad.GenerateRoad(connectionPoints[0].position, connectionPoints[0].up, connectionPoints[1].position, connectionPoints[1].up);
            }
        }
    }

    Transform[] FindClosestConnectionPoints()
    {
        float shortestDistance = float.MaxValue;
        PointOfInterest closestPOI_1 = null;
        PointOfInterest closestPOI_2 = null;
        int closestPointIndex_1 = -1;
        int closestPointIndex_2 = -1;

        for (int poiIndex_1 = 0; poiIndex_1 < pointsOfInterest.Length; poiIndex_1++)
        {
            PointOfInterest poi_1 = pointsOfInterest[poiIndex_1];
            
            if (poi_1.AllPointsConnected)
                continue;

            for (int poiIndex_2 = poiIndex_1 + 1; poiIndex_2 < pointsOfInterest.Length; poiIndex_2++)
            {
                PointOfInterest poi_2 = pointsOfInterest[poiIndex_2];
                
                if (poi_2.AllPointsConnected)
                    continue;

                if (poi_1.ConnectedPOIs.Contains(poi_2) || poi_2.ConnectedPOIs.Contains(poi_1))
                    continue;

                // Here we have two POIs that both have at least one unconnected connection point.

                for (int pointIndex_1 = 0; pointIndex_1 < poi_1.ConnectionPoints.Length; pointIndex_1++)
                {

                    ConnectionPoint connectionPoint_1 = poi_1.ConnectionPoints[pointIndex_1];

                    if (connectionPoint_1.Connected)
                        continue;

                    for (int pointIndex_2 = 0; pointIndex_2 < poi_2.ConnectionPoints.Length; pointIndex_2++)
                    {
                        ConnectionPoint connectionPoint_2 = poi_2.ConnectionPoints[pointIndex_2];

                        if (connectionPoint_2.Connected)
                            continue;

                        float distance = Vector3.Distance(connectionPoint_1.Point.position, connectionPoint_2.Point.position);

                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            closestPOI_1 = poi_1;
                            closestPOI_2 = poi_2;
                            closestPointIndex_1 = pointIndex_1;
                            closestPointIndex_2 = pointIndex_2;
                        }
                    }
                }
            }
        }

        if (closestPOI_1 != null && closestPOI_2 != null)
        {
            closestPOI_1.ConnectedPOIs.Add(closestPOI_2);
            closestPOI_2.ConnectedPOIs.Add(closestPOI_1);

            closestPOI_1.ConnectionPoints[closestPointIndex_1].Connected = true;
            closestPOI_2.ConnectionPoints[closestPointIndex_2].Connected = true;

            return new Transform[] { closestPOI_1.ConnectionPoints[closestPointIndex_1].Point, closestPOI_2.ConnectionPoints[closestPointIndex_2].Point };
        }
        else
        {
            Debug.LogWarning("No valid connection points found between POIs.");
            return null;
        }
    }

    TunnelPOI SpawnGoalTunnel()
    {
        Vector3 spawnPosition = GetRandomPositionInAnnulus(tunnelSpawnAnnulusInnerRadius, tunnelSpawnAnnulusOuterRadius);
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
        TunnelPOI tunnel = Instantiate(tunnelPrefab, spawnPosition, randomRotation);
        return tunnel;
    }

    TunnelPOI SpawnExitTunnel()
    {
        TunnelPOI tunnel = Instantiate(tunnelPrefab, Vector3.zero, Quaternion.identity);
        tunnel.SetAsExitTunnel(true);
        return tunnel;
    }

    PointOfInterest SpawnPOI(PointOfInterest poiPrefab, List<PointOfInterest> poiList)
    {
        for (int attempt = 0; attempt < 30; attempt++)
        {
            Vector3 spawnPosition = GetRandomPositionInAnnulus(spawnAnnulusInnerRadius, spawnAnnulusOuterRadius);

            if (IsPositionValid(spawnPosition, poiPrefab.Radius, poiList))
            {
                Quaternion randomRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                PointOfInterest newPOI = Instantiate(poiPrefab, spawnPosition, randomRotation);
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