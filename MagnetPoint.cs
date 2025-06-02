using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class MagnetPoint : MonoBehaviour
{
    public float attractionForce = 5f;
    public float snapDistance = 0.05f;
    public string magnetTag = "magnet";
    public string pairID = "default";

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(magnetTag) || other.gameObject == gameObject) return;

        Debug.Log($"[Magnet] Contact with: {other.name}");

        MagnetPoint otherConnector = other.GetComponent<MagnetPoint>();
        if (otherConnector == null || otherConnector.pairID != this.pairID) return;

        Transform myMagnet = transform;
        Transform myPiece = transform.parent;
        Transform otherMagnet = other.transform;
        Transform otherPiece = other.transform.parent;

        if (myPiece == null || otherPiece == null || myPiece == otherPiece) return;

        var grab1 = myPiece.GetComponent<Grabbable>();
        var grab2 = otherPiece.GetComponent<Grabbable>();
        bool bothBeingHeld = (grab1 != null && grab1.SelectingPointsCount > 0) &&
                             (grab2 != null && grab2.SelectingPointsCount > 0);

        if (!bothBeingHeld)
        {
            Debug.Log("[Magnet] One or both socks are not being held");
            return;
        }

        Debug.Log("[Magnet] Both socks are being held and pairID matches");

        float distance = Vector3.Distance(myMagnet.position, otherMagnet.position);
        Debug.Log($"[Magnet] Distance between magnets: {distance}");

        if (distance > snapDistance)
        {
            Vector3 direction = (myMagnet.position - otherMagnet.position).normalized;
            otherPiece.position += direction * attractionForce * Time.deltaTime;

            otherPiece.rotation = Quaternion.RotateTowards(
                otherPiece.rotation,
                myPiece.rotation,
                attractionForce * 50 * Time.deltaTime
            );
        }
        else
        {
            Debug.Log("[Magnet] Snapping socks together");

            Vector3 magnetLocalOffset = otherMagnet.localPosition;
            Quaternion magnetLocalRotation = otherMagnet.localRotation;

            otherPiece.position = myMagnet.position - (otherPiece.rotation * magnetLocalOffset);
            otherPiece.rotation = myMagnet.rotation * Quaternion.Inverse(magnetLocalRotation);

            Transform myGroup = FindGroupRoot(myPiece);
            Transform otherGroup = FindGroupRoot(otherPiece);
            Transform targetGroup = null;

            if (myGroup != null) targetGroup = myGroup;
            else if (otherGroup != null) targetGroup = otherGroup;
            else
            {
                GameObject newGroup = new GameObject("AssembledGroup_" + pairID);
                newGroup.transform.position = myPiece.position;
                myPiece.SetParent(newGroup.transform);
                targetGroup = newGroup.transform;
            }

            if (myPiece.parent != targetGroup) myPiece.SetParent(targetGroup);
            if (otherPiece.parent != targetGroup) otherPiece.SetParent(targetGroup);

            Rigidbody rb = otherPiece.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            DisableGrabbing(myPiece);
            DisableGrabbing(otherPiece);

            HighlightAsPaired(myPiece);
            HighlightAsPaired(otherPiece);
        }
    }

    private Transform FindGroupRoot(Transform piece)
    {
        Transform current = piece;
        while (current != null)
        {
            if (current.name.StartsWith("AssembledGroup_")) return current;
            current = current.parent;
        }
        return null;
    }

    private void DisableGrabbing(Transform piece)
    {
        var grabbable = piece.GetComponent<Grabbable>();
        if (grabbable != null) grabbable.enabled = false;

        var grabInteractable = piece.GetComponent<GrabInteractable>();
        if (grabInteractable != null) grabInteractable.enabled = false;

        var handGrab = piece.GetComponent<HandGrabInteractable>();
        if (handGrab != null) handGrab.enabled = false;
    }

    private void HighlightAsPaired(Transform piece)
    {
        Renderer rend = piece.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Color original = rend.material.color;
            rend.material.color = Color.Lerp(original, Color.green, 0.4f);
        }
    }
}
