using UnityEngine;

class Map : MonoBehaviour
{

    void OnEnable()
    {
        POIManager.OnPOIsSpawned += HandlePOIsSpawned;
    }

    void OnDisable()
    {
        POIManager.OnPOIsSpawned -= HandlePOIsSpawned;
    }

    void HandlePOIsSpawned(PointOfInterest[] pois)
    {
        
    }
}