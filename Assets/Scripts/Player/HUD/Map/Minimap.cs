using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct MapMarker
{
    public Transform WorldTransform;
    public RectTransform MinimapIconTransform;

    public Vector3 GetMapPosition(Vector3 playerPosition, float mapRadius, float mapScale)
    {
        Vector3 offset = WorldTransform.position - playerPosition;
        Vector3 clampedOffset = Vector3.ClampMagnitude(offset, mapRadius);
        return clampedOffset * mapScale;
    }
}

class Minimap : MonoBehaviour
{
    [SerializeField] RectTransform playerIcon;
    [SerializeField] float mapScale = 0.1f;
    [SerializeField] RectTransform mapIconPrefab;

    MapMarker[] minimapIcons; 
    TunnelPOI tunnel;

    float size;
    float mapRadius;

    void Awake()
    {
        size = gameObject.GetComponent<RectTransform>().rect.width;
        mapRadius = size / mapScale / 2f;
    }

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
        List<MapMarker> newIcons = new List<MapMarker>();

        foreach (var poi in pois)
        {
            RectTransform newMapIconTransform = Instantiate(mapIconPrefab, transform);
            newMapIconTransform.GetComponent<Image>().sprite = poi.MapIcon;

            MapMarker newIcon = new MapMarker
            {
                WorldTransform = poi.transform,
                MinimapIconTransform = newMapIconTransform
            };
            newIcons.Add(newIcon);
        }

        minimapIcons = newIcons.ToArray();
    }

    void Update()
    {
        mapRadius = size / mapScale / 2f;
        
        if (minimapIcons != null)
        {    
            foreach (var icon in minimapIcons)
            {
                Vector3 newIconPosition = icon.GetMapPosition(Player.Instance.transform.position, mapRadius, mapScale);
                icon.MinimapIconTransform.anchoredPosition = newIconPosition;
            }
        }

        if (Player.Instance != null)
        {   
            playerIcon.rotation = Player.Instance.transform.rotation;
        }
    }

    
}