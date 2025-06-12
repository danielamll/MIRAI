using UnityEngine;
using LSL;

public class SockPairMarkers : MonoBehaviour
{
    public GameObject detector1;
    public GameObject detector2;
    public float detectionRadius = 0.1f;
    public string streamName = "Markers";

    private Collider sock1 = null;
    private Collider sock2 = null;
    private bool hasFused = false;

    private StreamOutlet outlet;

    void Start()
    {
        StreamInfo streamInfo = new StreamInfo(streamName, "Markers", 1, 0, channel_format_t.cf_string, System.Guid.NewGuid().ToString());
        outlet = new StreamOutlet(streamInfo);
    }

    void Update()
    {
        sock1 = GetTouchingObject(detector1);
        sock2 = GetTouchingObject(detector2);

        if (!hasFused)
        {
            if (sock1 != null && sock2 != null && sock1 != sock2)
            {
                if (sock1.tag == sock2.tag && AreTouching(sock1, sock2))
                {
                    Debug.Log($"Fusion triggered between {sock1.name} and {sock2.name} with tag {sock1.tag}");

                    Collider toHide = Random.value < 0.5f ? sock1 : sock2;
                    Collider toShowPair = toHide == sock1 ? sock2 : sock1;

                    MeshRenderer meshToHide = toHide.GetComponentInChildren<MeshRenderer>();
                    if (meshToHide != null)
                        meshToHide.enabled = false;

                    Transform pair = toShowPair.transform.Find("pair");
                    if (pair != null)
                    {
                        MeshRenderer pairRenderer = pair.GetComponent<MeshRenderer>();
                        if (pairRenderer != null)
                            pairRenderer.enabled = true;
                    }

                    // Enviar marker por LSL
                    string[] sample = new string[] { $"fused:{sock1.tag}" };
                    outlet.push_sample(sample);

                    hasFused = true;
                }
            }
        }
        else if (sock1 == null || sock2 == null || !AreTouching(sock1, sock2))
        {
            hasFused = false;
        }
    }

    private Collider GetTouchingObject(GameObject detector)
    {
        Collider[] hits = Physics.OverlapSphere(detector.transform.position, detectionRadius);
        foreach (Collider hit in hits)
        {
            if (!string.IsNullOrEmpty(hit.tag) && hit.tag != "Untagged")
                return hit;
        }
        return null;
    }

    private bool AreTouching(Collider a, Collider b)
    {
        if (a == null || b == null) return false;
        float distance = Vector3.Distance(a.transform.position, b.transform.position);
        return distance < detectionRadius;
    }
}
