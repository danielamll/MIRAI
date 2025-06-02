using UnityEngine;

public class SockFusionManager : MonoBehaviour
{
    public GameObject detector1;
    public GameObject detector2;
    public float detectionRadius = 0.3f;

    private Collider sock1 = null;
    private Collider sock2 = null;
    private bool hasFused = false;

    void Update()
    {
        // Detecta calcetines tocados por cada detector
        sock1 = GetTouchingObject(detector1);
        sock2 = GetTouchingObject(detector2);

        if (!hasFused)
        {
            if (sock1 != null && sock2 != null && sock1 != sock2)
            {
                if (sock1.tag == sock2.tag)
                {
                    if (AreTouching(sock1, sock2))
                    {
                        Debug.Log($"Fusion triggered between {sock1.name} and {sock2.name} with tag {sock1.tag}");

                        // Elegir aleatoriamente cuál ocultar
                        Collider toHide = Random.value < 0.5f ? sock1 : sock2;
                        Collider toShowPair = toHide == sock1 ? sock2 : sock1;

                        // Ocultar solo el MeshRenderer del calcetín elegido
                        MeshRenderer meshToHide = toHide.GetComponentInChildren<MeshRenderer>();
                        if (meshToHide != null)
                        {
                            meshToHide.enabled = false;
                            Debug.Log($"Hid MeshRenderer of {toHide.name}");
                        }
                        else
                        {
                            Debug.LogWarning($"MeshRenderer not found in {toHide.name}");
                        }

                        // Activar el pair del otro calcetín
                        Transform pair = toShowPair.transform.Find("pair");
                        if (pair != null)
                        {
                            MeshRenderer pairRenderer = pair.GetComponent<MeshRenderer>();
                            if (pairRenderer != null)
                            {
                                pairRenderer.enabled = true;
                                Debug.Log($"Activated pair in {toShowPair.name}");
                            }
                            else
                            {
                                Debug.LogWarning($"Pair MeshRenderer not found in {toShowPair.name}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Pair object not found in {toShowPair.name}");
                        }

                        hasFused = true;  // Marca que la fusión ocurrió para este par
                    }
                }
                else
                {
                    Debug.Log($"Socks have different tags: {sock1.tag} and {sock2.tag}");
                }
            }
        }
        else
        {
            // Si se sueltan los calcetines (o los detectores dejan de tocarlos), reinicia hasFused para permitir otro evento
            if (sock1 == null || sock2 == null || !AreTouching(sock1, sock2))
            {
                Debug.Log("Resetting hasFused state, ready for next fusion.");
                hasFused = false;
            }
        }
    }

    private Collider GetTouchingObject(GameObject detector)
    {
        Collider[] hits = Physics.OverlapSphere(detector.transform.position, detectionRadius);
        foreach (Collider hit in hits)
        {
            if (!string.IsNullOrEmpty(hit.tag) && hit.tag != "Untagged")
            {
                return hit;
            }
        }
        return null;
    }

    private bool AreTouching(Collider a, Collider b)
    {
        if (a == null || b == null) return false;
        float distance = Vector3.Distance(a.transform.position, b.transform.position);
        // Debug.Log($"Distance between {a.name} and {b.name}: {distance}");
        return distance < detectionRadius;
    }
}
