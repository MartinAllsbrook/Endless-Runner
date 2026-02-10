using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

struct ConnectionPoint
{
    public Transform Point;
    public bool Connected;
}

[RequireComponent(typeof(Collider2D))]
abstract class PointOfInterest : MonoBehaviour
{
    [SerializeField] int countInWorld = 3;
    [SerializeField] float radius = 10f;
    [SerializeField] Sprite mapIcon;
    [SerializeField] Transform[] roadConnectionPoints;

    ConnectionPoint[] connectionPoints;
    public int CountInWorld => countInWorld;
    public float Radius => radius;
    public Sprite MapIcon => mapIcon;

    List<PointOfInterest> connectedPOIs = new List<PointOfInterest>();
    public List<PointOfInterest> ConnectedPOIs => connectedPOIs;
    
    public bool AllPointsConnected
    {
        get
        {
            foreach (var cp in connectionPoints)
            {
                if (!cp.Connected)
                    return false;
            }
            return true;
        }
    }
    public ConnectionPoint[] ConnectionPoints => connectionPoints;

    void Awake()
    {
        connectionPoints = new ConnectionPoint[roadConnectionPoints.Length];
        for (int i = 0; i < roadConnectionPoints.Length; i++)
        {
            connectionPoints[i] = new ConnectionPoint { Point = roadConnectionPoints[i], Connected = false };
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }

    protected abstract void OnPlayerEnter();
}