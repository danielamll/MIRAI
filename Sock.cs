using UnityEngine;


public class Sock : MonoBehaviour
{
    public Vector3 startPosition;
    public Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void ReturnToStart()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
