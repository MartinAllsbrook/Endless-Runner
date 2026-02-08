using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Map : MonoBehaviour
{
    [SerializeField] RectTransform mapPanel;
    [SerializeField] Minimap minimap;
    [SerializeField] RectTransform mapIconPrefab;
    [SerializeField] float mapScale = 0.1f;

    MapMarker[] mapMarkers;

    float mapDisplaySize => mapPanel.rect.width;

    void Awake()
    {
        
    }

    void OnEnable()
    {
        POIManager.OnPOIsSpawned += HandlePOIsSpawned;
        InputReader.OnToggleMap += ToggleMap;
    }

    void OnDisable()
    {
        POIManager.OnPOIsSpawned -= HandlePOIsSpawned;
        InputReader.OnToggleMap -= ToggleMap;
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
        minimap.SetMapMarkers(mapMarkers);
    }

    RectTransform CreateMapIcon(PointOfInterest poi, Transform parent)
    {
        RectTransform newIcon = Instantiate(mapIconPrefab, parent);
        newIcon.GetComponent<Image>().sprite = poi.MapIcon;
        return newIcon;
    }

    void Update()
    {
        UpdateMapPositions();
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

    void ToggleMap()
    {
        mapPanel.gameObject.SetActive(!mapPanel.gameObject.activeSelf);
    }
}