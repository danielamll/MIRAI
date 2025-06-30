using UnityEngine;

public class BasketManagerWithMarkers : MonoBehaviour
{
    public int correctCount = 0;
    public int errorCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Solo continuar si el objeto tiene un hijo llamado "pair" → es una calceta
        Transform pair = other.transform.Find("pair");
        if (pair == null) return;

        string basketTag = gameObject.tag;
        string sockTag = other.tag;

        // Aún puedes usar este script si necesitas ReturnToStart
        Sock sockScript = other.GetComponent<Sock>();

        if (sockTag == basketTag)
        {
            if (!pair.gameObject.activeSelf)
            {
                string log = $"{other.name} has no pair. Returning.";
                Debug.Log(log);
                SendMarker(log);
                sockScript?.ReturnToStart();
                return;
            }

            correctCount++;
            string log2 = $"{other.name} correctly placed in {basketTag} basket. Total: {correctCount}";
            Debug.Log(log2);
            SendMarker(log2);
            DisableAndDestroySock(other);
        }
        else
        {
            if (pair.gameObject.activeSelf)
            {
                errorCount++;
                string log3 = $"{other.name} has a pair but wrong color ({sockTag}) for {basketTag} basket. Error count: {errorCount}";
                Debug.Log(log3);
                SendMarker(log3);
                DisableAndDestroySock(other);
            }
            else
            {
                string log4 = $"{other.name} wrong color and no pair. Returning.";
                Debug.Log(log4);
                SendMarker(log4);
                sockScript?.ReturnToStart();
            }
        }
    }

    private void DisableAndDestroySock(Collider sock)
    {
        Collider sockCollider = sock.GetComponent<Collider>();
        if (sockCollider != null)
            sockCollider.enabled = false;

        MeshRenderer[] renderers = sock.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Destroy(sock.gameObject, 1.5f);
    }

    private void SendMarker(string message)
    {
        if (BasketStreamManager.Instance != null)
        {
            BasketStreamManager.Instance.SendMarker(message);
        }
    }
}