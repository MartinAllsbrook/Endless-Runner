using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Map : MonoBehaviour
{
    [SerializeField] RectTransform mapPanel;
    [SerializeField] Minimap minimap;
    [SerializeField] RectTransform mapIconPrefab;
    [SerializeField] RectTransform playerIcon;
    [SerializeField] float mapScale = 0.1f;

    MapMarker[] mapMarkers;
    public MapMarker[] MapMarkers => mapMarkers;

    float mapDisplaySize => mapPanel.rect.width;

    List<RectTransform> instantiatedIcons = new List<RectTransform>();

    void Awake()
    {
        
    }

    void OnEnable()
    {
        POIManager.OnPOIsSpawned += HandlePOIsSpawned;
        InputReader.OnToggleMap += ToggleMap;
        GameManager.OnTunnelEntered += CloseAndClear;
    }

    void OnDisable()
    {
        POIManager.OnPOIsSpawned -= HandlePOIsSpawned;
        InputReader.OnToggleMap -= ToggleMap;
        GameManager.OnTunnelEntered -= CloseAndClear;
    }

    void HandlePOIsSpawned(PointOfInterest[] pois)
    {
        List<MapMarker> newMarkers = new List<MapMarker>();

        foreach (var poi in pois)
        {
            RectTransform newMapIcon = CreateMapIcon(poi, mapPanel);
            RectTransform newMinimapIcon = CreateMapIcon(poi, minimap.transform);

            MapMarker newMarker = new MapMarker(poi.transform, newMinimapIcon, newMapIcon);
            newMarkers.Add(newMarker);
        }

        mapMarkers = newMarkers.ToArray();
    }

    RectTransform CreateMapIcon(PointOfInterest poi, Transform parent)
    {
        RectTransform newIcon = Instantiate(mapIconPrefab, parent);
        newIcon.GetComponent<Image>().sprite = poi.MapIcon;
        instantiatedIcons.Add(newIcon);
        return newIcon;
    }

    void Update()
    {
        UpdateMapPositions();
        UpdatePlayerIcon();
    }

    void UpdatePlayerIcon()
    {
        if (Player.Instance == null)
            return;

        playerIcon.rotation = Player.Instance.transform.rotation;
        playerIcon.anchoredPosition = Player.Instance.transform.position * mapScale;
    }

    void UpdateMapPositions()
    {
        if (mapMarkers == null)
            return;

        foreach (var icon in mapMarkers)
        {
            icon.UpdateMapPosition(mapScale);
        }
    }

    void CloseAndClear()
    {
        mapPanel.gameObject.SetActive(false);
        Clear();
    }

    void Clear()
    {
        mapPanel.gameObject.SetActive(false);

        if (mapMarkers != null)
        {
            foreach (var marker in mapMarkers)
            {
                Destroy(marker.PointOfInterest.gameObject);
                Destroy(marker.PointOfInterest.gameObject);
            }
        }

        foreach (var icon in instantiatedIcons)        
        {
            Destroy(icon.gameObject);
        }

        instantiatedIcons.Clear();
        mapMarkers = null;
    }

    void ToggleMap()
    {
        mapPanel.gameObject.SetActive(!mapPanel.gameObject.activeSelf);
    }
}