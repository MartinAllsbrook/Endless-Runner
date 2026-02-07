using System;
using System.Collections.Generic;
using UnityEngine;

public enum ScatterTag
{
    Rock = 100,
    Cactus = 200,
    Brush = 300,
}

[Serializable]
public struct ScatterSettings
{
    public ScatterTag tag;
    public WorldObject prefab;
    public int targetDensity;
}

class ScatterManager : MonoBehaviour
{
    public static ScatterManager Instance;

    public ScatterSettings[] scatterSettings;

    Dictionary<ScatterTag, ScatterSettings> settingsDict = new Dictionary<ScatterTag, ScatterSettings>();
    Dictionary<ScatterTag, ObjectPool<WorldObject>> scatterPools = new Dictionary<ScatterTag, ObjectPool<WorldObject>>();

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize settings dictionary and pools
        foreach (var setting in scatterSettings)
        {
            settingsDict[setting.tag] = setting;
            scatterPools[setting.tag] = new ObjectPool<WorldObject>(setting.prefab, setting.targetDensity, this.transform);
        }
    }

    public WorldObject SpawnScatter(ScatterTag tag, Vector2 position)
    {
        if (scatterPools.ContainsKey(tag))
        {
            WorldObject obj = scatterPools[tag].Get();
            obj.transform.position = position;
            return obj;
        }
        else
        {
            Debug.LogWarning($"No scatter pool found for tag {tag}");
            return null;
        }
    }

    public void ReturnScatter(ScatterTag tag, WorldObject obj)
    {
        if (scatterPools.ContainsKey(tag))
        {
            scatterPools[tag].Return(obj);
        }
        else
        {
            Debug.LogWarning($"No scatter pool found for tag {tag}");
            Destroy(obj.gameObject);
        }
    }

    public ScatterSettings GetSettings(ScatterTag tag)
    {
        if (settingsDict.ContainsKey(tag))
        {
            return settingsDict[tag];
        }
        else
        {
            Debug.LogWarning($"No scatter settings found for tag {tag}");
            return default;
        }
    }
}