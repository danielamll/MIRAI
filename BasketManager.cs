using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public int correctCount = 0;
    public int errorCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(other.tag) && other.tag != "Untagged")
        {
            string basketTag = gameObject.tag; // Basket's tag (color)
            string sockTag = other.tag; // Sock's tag

            Sock sockScript = other.GetComponent<Sock>();
            Transform pair = other.transform.Find("pair");

            if (sockTag == basketTag)
            {
                if (pair == null || !pair.gameObject.activeSelf)
                {
                    Debug.Log($"❌ {other.name} has no pair. Returning.");
                    sockScript?.ReturnToStart();
                    return;
                }

                correctCount++;
                Debug.Log($"✅ {other.name} correctly placed in {basketTag} basket. Total: {correctCount}");

                DisableAndDestroySock(other);
            }
            else
            {
                if (pair != null && pair.gameObject.activeSelf)
                {
                    errorCount++;
                    Debug.Log($"❌ {other.name} has a pair but wrong color ({sockTag}) for {basketTag} basket. Error count: {errorCount}");

                    DisableAndDestroySock(other);
                }
                else
                {
                    Debug.Log($"❌ {other.name} wrong color and no pair. Returning.");
                    sockScript?.ReturnToStart();
                }
            }
        }
    }

    private void DisableAndDestroySock(Collider sock)
    {
        // Disable interaction
        Collider sockCollider = sock.GetComponent<Collider>();
        if (sockCollider != null)
            sockCollider.enabled = false;

        // Hide visuals
        MeshRenderer[] renderers = sock.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Destroy after delay
        Destroy(sock.gameObject, 1.5f);
    }
}
