using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
abstract class PointOfInterest : MonoBehaviour
{
    [SerializeField] int countInWorld = 3;
    [SerializeField] float radius = 10f;
    [SerializeField] Sprite mapIcon;
    [SerializeField] Transform[] roadConnectionPoints;

    public int CountInWorld => countInWorld;
    public float Radius => radius;
    public Sprite MapIcon => mapIcon;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }

    protected abstract void OnPlayerEnter();
}