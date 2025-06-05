using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

[System.Serializable]
public class SpiderTriggerSettings
{
    public GameObject prefab;
    public int spawnCount;
}

public class SpiderPerTrigger : MonoBehaviour
{
    [Header("LSL Settings")]
    public string streamName = "Markers";

    [Header("Spiders per Trigger")]
    public SpiderTriggerSettings[] spiderTriggers = new SpiderTriggerSettings[6];
    public Transform spawnArea;

    private List<GameObject>[] spawnedSpidersPerTrigger = new List<GameObject>[6];
    private StreamInlet inlet;
    private StreamInfo[] results;

    void Start()
    {
        for (int i = 0; i < spawnedSpidersPerTrigger.Length; i++)
        {
            spawnedSpidersPerTrigger[i] = new List<GameObject>();
        }
        StartCoroutine(WaitForStream());
    }

    IEnumerator WaitForStream()
    {
        yield return new WaitForSeconds(1f);
        results = LSL.LSL.resolve_stream("name", streamName);

        if (results.Length > 0)
        {
            inlet = new StreamInlet(results[0]);
            Debug.Log("[LSL] Stream connected to: " + streamName);
        }
        else
        {
            Debug.LogWarning("[LSL] No LSL stream found with name: " + streamName);
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            string[] sample = new string[1];
            double timestamp = inlet.pull_sample(sample, 0.0f);

            if (timestamp != 0.0)
            {
                Debug.Log("[LSL] Received trigger: " + sample[0]);
                if (int.TryParse(sample[0], out int triggerLevel))
                {
                    triggerLevel = Mathf.Clamp(triggerLevel, 0, 5);
                    UpdateSpiders(triggerLevel);
                }
            }
        }
    }

    void UpdateSpiders(int triggerLevel)
    {
        SpiderTriggerSettings config = spiderTriggers[triggerLevel];
        if (config == null || config.prefab == null)
        {
            Debug.LogWarning($"No prefab configured for trigger {triggerLevel}");
            return;
        }

        int desiredCount = config.spawnCount;
        List<GameObject> currentSpiders = spawnedSpidersPerTrigger[triggerLevel];

        // Elimina spiders de otros niveles
        for (int i = 0; i < spawnedSpidersPerTrigger.Length; i++)
        {
            if (i != triggerLevel)
            {
                foreach (var obj in spawnedSpidersPerTrigger[i])
                {
                    if (obj != null) Destroy(obj);
                }
                spawnedSpidersPerTrigger[i].Clear();
            }
        }

        if (currentSpiders.Count > desiredCount)
        {
            int toRemove = currentSpiders.Count - desiredCount;
            for (int i = 0; i < toRemove; i++)
            {
                if (currentSpiders.Count > 0)
                {
                    Destroy(currentSpiders[0]);
                    currentSpiders.RemoveAt(0);
                }
            }
        }
        else if (currentSpiders.Count < desiredCount)
        {
            int toSpawn = desiredCount - currentSpiders.Count;
            SpawnSpiders(triggerLevel, config.prefab, toSpawn);
        }
    }

    void SpawnSpiders(int triggerLevel, GameObject prefab, int count)
    {
        Vector3 center = spawnArea.position;
        Vector3 size = spawnArea.localScale;

        for (int i = 0; i < count; i++)
        {
            float x = UnityEngine.Random.Range(-size.x / 2f, size.x / 2f);
            float y = UnityEngine.Random.Range(-size.y / 2f, size.y / 2f);
            float z = UnityEngine.Random.Range(-size.z / 2f, size.z / 2f);

            Vector3 randomPos = center + new Vector3(x, y, z);
            GameObject spider = Instantiate(prefab, randomPos, Quaternion.identity);
            spawnedSpidersPerTrigger[triggerLevel].Add(spider);
        }
    }
}
