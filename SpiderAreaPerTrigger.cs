using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

[System.Serializable]
public class SpiderAreaTriggerSettings
{
    public GameObject prefab;
    public int spawnCount;
    public float minDistance = 1.5f;
    public float maxDistance = 3.0f;
    public Transform spawnArea;
    public GameObject barrier;

    [Header("Animación")]
    public bool enableAnimation = true; // Se usa el Animator del prefab instanciado
}

public class SpiderAreaPerTrigger : MonoBehaviour
{
    [Header("LSL Settings")]
    public string streamName = "Spawner";

    [Header("Spiders per Trigger")]
    public SpiderAreaTriggerSettings[] spiderTriggers = new SpiderAreaTriggerSettings[6];

    private List<GameObject>[] spawnedSpidersPerTrigger = new List<GameObject>[6];
    private StreamInlet inlet;
    private StreamInfo[] results;
    private StreamOutlet spiderTriggerOutlet;

    void Start()
    {
        for (int i = 0; i < spawnedSpidersPerTrigger.Length; i++)
        {
            spawnedSpidersPerTrigger[i] = new List<GameObject>();
        }

        var streamInfo = new StreamInfo("SpiderTrigger", "Markers", 1, 0, channel_format_t.cf_string, System.Guid.NewGuid().ToString());
        spiderTriggerOutlet = new StreamOutlet(streamInfo);

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

                // Enviar marker de salida para que lo capture Python
                spiderTriggerOutlet.push_sample(new string[] { sample[0] });

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
        SpiderAreaTriggerSettings config = spiderTriggers[triggerLevel];
        if (config == null || config.prefab == null || config.spawnArea == null)
        {
            Debug.LogWarning($"Missing settings for trigger {triggerLevel}");
            return;
        }

        int desiredCount = config.spawnCount;
        List<GameObject> currentSpiders = spawnedSpidersPerTrigger[triggerLevel];

        for (int i = 0; i < spawnedSpidersPerTrigger.Length; i++)
        {
            if (i != triggerLevel)
            {
                foreach (var obj in spawnedSpidersPerTrigger[i])
                {
                    if (obj != null) Destroy(obj);
                }
                spawnedSpidersPerTrigger[i].Clear();

                if (spiderTriggers[i].barrier != null)
                {
                    spiderTriggers[i].barrier.SetActive(false);
                }
            }
        }

        if (config.barrier != null)
        {
            config.barrier.SetActive(true);
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
            SpawnSpiders(triggerLevel, config);
        }

        // Activar animaciones en los objetos actuales si es necesario
        foreach (var spider in currentSpiders)
        {
            if (spider != null && config.enableAnimation)
            {
                Animator animator = spider.GetComponent<Animator>();
                if (animator != null)
                    animator.enabled = true;
            }
        }
    }

    void SpawnSpiders(int triggerLevel, SpiderAreaTriggerSettings config)
    {
        BoxCollider box = config.spawnArea.GetComponentInChildren<BoxCollider>();
        if (box == null)
        {
            Debug.LogError("BoxCollider no encontrado en SpawnArea.");
            return;
        }

        for (int i = 0; i < config.spawnCount; i++)
        {
            Vector3 localPoint = new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.5f, 0.5f)
            );

            localPoint = Vector3.Scale(localPoint, box.size);
            Vector3 worldPoint = box.transform.TransformPoint(localPoint + box.center);

            GameObject spider = Instantiate(config.prefab, worldPoint, Quaternion.identity);
            spider.SetActive(true);

            if (config.enableAnimation)
            {
                Animator animator = spider.GetComponent<Animator>();
                if (animator != null)
                    animator.enabled = true;
            }

            spawnedSpidersPerTrigger[triggerLevel].Add(spider);

            Debug.Log($"[SPAWN] Instantiated spider for trigger {triggerLevel} at {worldPoint}");
        }
    }
}

public class AreaDistanceConstraint : MonoBehaviour
{
    public Vector3 center;
    public float minDistance = 1.5f;
    public float maxDistance = 3.0f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, center);

        if (distance < minDistance)
        {
            Vector3 dir = (transform.position - center).normalized;
            transform.position = center + dir * minDistance;
        }
        else if (distance > maxDistance)
        {
            Vector3 dir = (transform.position - center).normalized;
            transform.position = center + dir * maxDistance;
        }
    }
}