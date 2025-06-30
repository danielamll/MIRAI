using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using LSL;

public class OpenScene : MonoBehaviour
{
    [Header("LSL Settings")]
    public string streamName = "Spawner";    // Stream exacto generado por Python
    public int triggerToWatch = 6;           // Valor esperado: 0
    public string sceneToLoad = "TargetScene"; // Nombre de la escena a cargar

    private StreamInlet inlet;
    private StreamInfo[] results;

    void Start()
    {
        StartCoroutine(WaitForStream());
    }

    IEnumerator WaitForStream()
    {
        yield return new WaitForSeconds(1f);  // Esperar a que el stream se active

        results = LSL.LSL.resolve_stream("name", streamName);  // Buscar por nombre

        if (results.Length > 0)
        {
            inlet = new StreamInlet(results[0]);
            Debug.Log("[LSL] Conectado al stream: " + results[0].name());
        }
        else
        {
            Debug.LogWarning("[LSL] No se encontró el stream con name: " + streamName);
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            int[] sample = new int[1];  // Usamos int[] porque el stream es int32
            double timestamp = inlet.pull_sample(sample, 0.0f);

            if (timestamp > 0.0)
            {
                Debug.Log("[LSL] Trigger recibido (int): " + sample[0]);
                if (sample[0] == triggerToWatch)
                {
                    Debug.Log("[LSL] Cargando escena: " + sceneToLoad);
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }
}
