using UnityEngine;

class Inventory : MonoBehaviour
{
    int scrapCount = 0;

    public event System.Action<int> OnScrapChanged;

    public void AddScrap(int amount)
    {
        scrapCount += amount;
        OnScrapChanged?.Invoke(scrapCount);
    }
}