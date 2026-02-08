using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Minimap : MonoBehaviour
{
    [SerializeField] float mapScale = 0.1f;
    [SerializeField] RectTransform playerIcon;
    [SerializeField] Map map;

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
        if (map.MapMarkers == null)
            return;

        foreach (var marker in map.MapMarkers)
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
}