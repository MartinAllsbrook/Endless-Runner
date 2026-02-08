using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Minimap : MonoBehaviour
{
    [SerializeField] float mapScale = 0.1f;
    [SerializeField] RectTransform playerIcon;

    MapMarker[] mapMarkers;
    float size;
    float mapRadius;

    void Awake()
    {
        size = gameObject.GetComponent<RectTransform>().rect.width;
        mapRadius = size / mapScale / 2f;
    }

    void Update()
    {
        UpdatePlayerIcon();
        UpdateMapPositions();
    }

    void UpdateMapPositions()
    {
        if (mapMarkers == null)
            return;

        foreach (var marker in mapMarkers)
        {
            marker.UpdateMinimapPosition(Player.Instance.transform.position, mapScale, mapRadius);
        }
    }

    void UpdatePlayerIcon()
    {
        if (Player.Instance == null)
            return;

        playerIcon.rotation = Player.Instance.transform.rotation;
    }

    public void SetMapMarkers(MapMarker[] markers)
    {
        mapMarkers = markers;
    }
}