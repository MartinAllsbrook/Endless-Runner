using UnityEngine;

class Minimap : MonoBehaviour
{
    [SerializeField] Transform playerIcon;
    [SerializeField] float mapScale = 0.1f;
    [SerializeField] RectTransform tunnelIndicator;

    TunnelPOI tunnel;

    float size;
    float mapRadius;

    void Awake()
    {
        size = gameObject.GetComponent<RectTransform>().rect.width;
    }

    void OnEnable()
    {
        Debug.Log("Minimap: Subscribing to OnWorldLoaded");
        GameManager.OnWorldLoaded += HandleWorldLoaded;
    }

    void OnDisable()
    {
        Debug.Log("Minimap: Unsubscribing from OnWorldLoaded");
        GameManager.OnWorldLoaded -= HandleWorldLoaded;
    }

    void HandleWorldLoaded()
    {
        Debug.Log("Minimap: World Loaded - Finding Tunnel");
        tunnel = FindFirstObjectByType<TunnelPOI>();
    }

    void Update()
    {
        mapRadius = size / mapScale / 2f;
        if (tunnel != null)
        {
            Vector3 tunnelPosition = tunnel.transform.position;
            Vector3 playerPosition = Player.Instance.transform.position;

            Vector3 offset = playerPosition - tunnelPosition;
            if (offset.magnitude > mapRadius)
            {
                offset = offset.normalized * mapRadius;
            }

            offset *= mapScale;
            offset *= -1f;

            tunnelIndicator.anchoredPosition = offset;
        }
        if (Player.Instance != null)
        {   
            playerIcon.rotation = Player.Instance.transform.rotation;
        }
    }

    
}