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

    List<MapMarker> minimapIcons = new List<MapMarker>(); 
    TunnelPOI tunnel;

    float size;
    float mapRadius;

    void Awake()
    {
        size = gameObject.GetComponent<RectTransform>().rect.width;
        mapRadius = size / mapScale / 2f;

        // Clear minimap icons when world is closed to prevent references to objects in removed scene
        // I feel like there has to be a better way to organize this because this seems delicate
        GameManager.BeforeWorldClosed += () =>
        {
            minimapIcons.Clear();
        };
    }

    public void AddTransformToMinimap(Transform worldTransform, Sprite mapIcon)
    {
        RectTransform newMapIconTransform = Instantiate(mapIconPrefab, transform);
        newMapIconTransform.GetComponent<Image>().sprite = mapIcon;

        MapMarker newIcon = new MapMarker
        {
            WorldTransform = worldTransform,
            MinimapIconTransform = newMapIconTransform
        };
        minimapIcons.Add(newIcon);
    }

    void Update()
    {
        mapRadius = size / mapScale / 2f;
        
        foreach (var icon in minimapIcons)
        {
            Vector3 newIconPosition = icon.GetMapPosition(Player.Instance.transform.position, mapRadius, mapScale);
            icon.MinimapIconTransform.anchoredPosition = newIconPosition;
        }

        if (Player.Instance != null)
        {   
            playerIcon.rotation = Player.Instance.transform.rotation;
        }
    }

    
}