
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class LSLSpiderSpawner : MonoBehaviour
{

    [Header("LSL Settings")]
    public string streamName = "Markers";
    
    public GameObject[] allPrefabs;
    public Transform spawnArea;
    public int[] spidersPerLevel = new int[6];
    public float triggerCooldown = 5f;

    private List<GameObject> spawnedSpiders = new List<GameObject>();
    private int currentLevel = 0;
    private float lastUpdateTime = 0f;

    private StreamInlet inlet;
    private StreamInfo[] results;

    void Start()
    {
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

    void UpdateSpiders(int newLevel)
    {
        if (Time.time - lastUpdateTime < triggerCooldown)
            return;

        int desiredCount = spidersPerLevel[newLevel];

        if (newLevel == 0)
        {
            foreach (var obj in spawnedSpiders)
            {
                if (obj != null) Destroy(obj);
            }
            spawnedSpiders.Clear();
        }
        else
        {
            int currentCount = spawnedSpiders.Count;

            if (currentCount > desiredCount)
            {
                int toRemove = currentCount - desiredCount;
                for (int i = 0; i < toRemove; i++)
                {
                    int index = UnityEngine.Random.Range(0, spawnedSpiders.Count);
                    if (spawnedSpiders[index] != null)
                    {
                        Destroy(spawnedSpiders[index]);
                        spawnedSpiders.RemoveAt(index);
                    }
                }
            }
            else if (currentCount < desiredCount)
            {
                int toSpawn = desiredCount - currentCount;
                SpawnSpiders(toSpawn);
            }
        }

        currentLevel = newLevel;
        lastUpdateTime = Time.time;
    }

    void SpawnSpiders(int count)
    {
        if (allPrefabs == null || allPrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned in allPrefabs array");
            return;
        }

        Vector3 center = spawnArea.position;
        Vector3 size = spawnArea.localScale;

        for (int i = 0; i < count; i++)
        {
            float x = UnityEngine.Random.Range(-size.x / 2f, size.x / 2f);
            float y = UnityEngine.Random.Range(-size.y / 2f, size.y / 2f);
            float z = UnityEngine.Random.Range(-size.z / 2f, size.z / 2f);

            Vector3 randomPos = center + new Vector3(x, y, z);
            GameObject chosen = allPrefabs[UnityEngine.Random.Range(0, allPrefabs.Length)];
            GameObject spider = Instantiate(chosen, randomPos, Quaternion.identity);
            spawnedSpiders.Add(spider);
        }
    }
}
