using UnityEngine;

public class MagnetConnector : MonoBehaviour
{
    public float attractionForce = 5f;
    public float snapDistance = 0.05f;
    public string magnetTag = "magnet";

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(magnetTag) || other.gameObject == this.gameObject) return;

        Transform myMagnet = transform;
        Transform myPiece = transform.parent;
        Transform otherMagnet = other.transform;
        Transform otherPiece = other.transform.parent;

        if (myPiece == null || otherPiece == null || myPiece == otherPiece) return;

        float distance = Vector3.Distance(myMagnet.position, otherMagnet.position);
        if (distance > snapDistance)
        {
            // Atrae la pieza hacia el imán receptor
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
            // CALCULA la diferencia de posición y rotación entre el imán y su padre
            Vector3 magnetLocalOffset = otherMagnet.localPosition;
            Quaternion magnetLocalRotation = otherMagnet.localRotation;

            // AJUSTA la posición y rotación de la pieza para alinear los imanes
            otherPiece.position = myMagnet.position - (magnetLocalOffset = otherPiece.rotation * magnetLocalOffset);
            otherPiece.rotation = myMagnet.rotation * Quaternion.Inverse(magnetLocalRotation);

            // PEGA la pieza
            otherPiece.SetParent(myPiece);
            Rigidbody rb = otherPiece.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;
        }
    }
}
