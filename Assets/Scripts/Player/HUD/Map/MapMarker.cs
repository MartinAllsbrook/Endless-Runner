using Unity.VisualScripting;
using UnityEngine;

class MapMarker
{
    private Transform pointOfInterest;
    public Transform PointOfInterest => pointOfInterest;
    
    private RectTransform mapIconTransform;
    private RectTransform minimapIconTransform;

    public MapMarker(Transform _pointOfInterest, RectTransform _minimapIconTransform, RectTransform _mapIconTransform)
    {
        pointOfInterest = _pointOfInterest;
        minimapIconTransform = _minimapIconTransform;
        mapIconTransform = _mapIconTransform;
    }

    public void UpdateMinimapPosition(Vector3 playerPosition, float mapScale, float mapRadius)
    {
        Vector3 offset = pointOfInterest.position - playerPosition;
        Vector3 clampedOffset = Vector3.ClampMagnitude(offset, mapRadius);
        Vector3 position = clampedOffset * mapScale;
        minimapIconTransform.anchoredPosition = position;
    }

    public void UpdateMapPosition(float mapScale)
    {
        Vector3 position = pointOfInterest.position  * mapScale;
        mapIconTransform.anchoredPosition = position;
    }
}