using System.Collections;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float rotationSpeed = 90f; // Degrees per second
    public float directionChangeInterval = 2f;
    public Animator spiderAnimator;

    private Vector3 moveDirection;
    private float timeSinceLastChange;

    void Start()
    {
        ChooseRandomDirection();

        if (spiderAnimator != null)
        {
            spiderAnimator.SetBool("IsWalking", true);
        }
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= directionChangeInterval)
        {
            ChooseRandomDirection();
            timeSinceLastChange = 0f;
        }
    }

    void ChooseRandomDirection()
    {
        // Pick a random rotation angle (0-360)
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
        moveDirection = transform.forward;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with ground or other spiders if needed
        if (collision.gameObject.CompareTag("Ground")) return;

        // Change direction immediately on bump
        ChooseRandomDirection();
        timeSinceLastChange = 0f;
    }
}

