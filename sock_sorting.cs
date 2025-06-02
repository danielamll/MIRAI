using UnityEngine;
using Oculus.Interaction;

public class SockMagnetConnector : MonoBehaviour
{
    public float snapDistance = 0.05f;
    public float colorMatchTolerance = 0.01f;
    public LayerMask sockLayer;

    private Grabbable grabbable;
    private string sockColor;
    private bool isGrabbed => grabbable != null && grabbable.SelectingPointsCount > 0;

    void Start()
    {
        grabbable = GetComponentInParent<Grabbable>();
        sockColor = gameObject.tag; // usa el tag para definir el color del calcetín ("WhiteSock", "BlackSock", etc.)
    }

    void Update()
    {
        if (!isGrabbed) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, snapDistance, sockLayer);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            SockMagnetConnector otherSock = hit.GetComponent<SockMagnetConnector>();
            if (otherSock == null || !otherSock.isGrabbed) continue;

            // Verificar que ambos tengan el mismo color
            if (sockColor != otherSock.sockColor) continue;

            // Verifica que aún no estén pegados
            if (transform.parent == otherSock.transform.parent) continue;

            // Pega los calcetines
            PairSocks(otherSock);
            break;
        }
    }

    private void PairSocks(SockMagnetConnector otherSock)
    {
        // Pega físicamente
        otherSock.transform.SetParent(transform);
        Rigidbody rb1 = GetComponentInParent<Rigidbody>();
        Rigidbody rb2 = otherSock.GetComponentInParent<Rigidbody>();
        if (rb1) rb1.isKinematic = true;
        if (rb2) rb2.isKinematic = true;

        // Agrega efecto visual
        HighlightAsPaired();
        otherSock.HighlightAsPaired();

        Debug.Log("Socks paired: " + sockColor);
    }

    private void HighlightAsPaired()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Color original = rend.material.color;
            rend.material.color = Color.Lerp(original, Color.yellow, 0.5f); // efecto visual sutil
        }
    }
}
