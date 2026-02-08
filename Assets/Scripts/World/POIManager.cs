using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

class POIManager : MonoBehaviour
{
    [SerializeField] TunnelPOI tunnelPrefab;
    [SerializeField] PointOfInterest[] poiPrefabs;
    [SerializeField] float spawnAnnulusInnerRadius = 50f;
    [SerializeField] float spawnAnnulusOuterRadius = 1000f;
    [SerializeField] float tunnelSpawnAnnulusInnerRadius = 450f;
    [SerializeField] float tunnelSpawnAnnulusOuterRadius = 650f;

    List<PointOfInterest> spawnedPOIs = new List<PointOfInterest>(); 

    public void SpawnPOIs()
    {
        // Start by spawning the tunnel
        SpawnTunnel();

        // Then spawn other POIs
        foreach (var poi in poiPrefabs)
        {
            for (int i = 0; i < poi.CountInWorld; i++)
            {
                SpawnPOI(poi);
            }
        }
    }

    void SpawnTunnel()
    {
        Vector3 spawnPosition = GetRandomPositionInAnnulus(tunnelSpawnAnnulusInnerRadius, tunnelSpawnAnnulusOuterRadius);
        TunnelPOI tunnel = Instantiate(tunnelPrefab, spawnPosition, Quaternion.identity);
        AddPOI(tunnel);
    }

    void SpawnPOI(PointOfInterest poiPrefab)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 spawnPosition = GetRandomPositionInAnnulus(spawnAnnulusInnerRadius, spawnAnnulusOuterRadius);

            if (IsPositionValid(spawnPosition, poiPrefab.Radius))
            {
                PointOfInterest newPOI = Instantiate(poiPrefab, spawnPosition, Quaternion.identity);
                AddPOI(newPOI);
                return;
            }
        }
    }

    bool IsPositionValid(Vector3 position, float radius)
    {
        foreach (var existingPOI in spawnedPOIs)
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

    void AddPOI(PointOfInterest poi)
    {
        spawnedPOIs.Add(poi);
        Minimap minimap = FindFirstObjectByType<Minimap>();
        if (minimap != null)
        {
            minimap.AddTransformToMinimap(poi.transform, poi.MinimapIcon);
        }
    }

    Vector3 GetRandomPositionInAnnulus(float innerRadius, float outerRadius)
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        float randomDistance = UnityEngine.Random.Range(innerRadius, outerRadius);
        Vector3 spawnPosition = Player.Instance.transform.position + new Vector3(randomDirection.x, randomDirection.y) * randomDistance;
        return spawnPosition;
    }
}