using System.Collections;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    public float moveSpeed = 0.5f;       // Velocidad de movimiento
    public float rotationSpeed = 20f;    // Velocidad de giro (grados por segundo)
    public float directionChangeInterval = 0.5f; // Cada cuánto cambia dirección (segundos)
    public Animator spiderAnimator;      // Asignas aquí tu Animator

    private Vector3 moveDirection;
    private float timeSinceLastChange;

    void Start()
    {
        // Inicializa con un movimiento hacia adelante
        moveDirection = transform.forward;
        timeSinceLastChange = 0f;

        if (spiderAnimator != null)
        {
            spiderAnimator.SetBool("IsWalking", true); // Asegúrate de tener este parámetro en tu Animator
        }
    }

    void Update()
    {
        // Mueve la araña hacia la dirección actual
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Lógica para cambiar la dirección cada ciertos segundos
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= directionChangeInterval)
        {
            ChangeDirection();
            timeSinceLastChange = 0f;
        }
    }

    void ChangeDirection()
    {
        // Decide aleatoriamente si va a girar o moverse hacia un lado
        int randomChoice = Random.Range(0, 3);

        if (randomChoice == 0)
        {
            // Girar a la derecha
            StartCoroutine(RotateSpider(rotationSpeed));
        }
        else if (randomChoice == 1)
        {
            // Girar a la izquierda
            StartCoroutine(RotateSpider(-rotationSpeed));
        }
        else
        {
            // Sigue moviéndose recto, pero opcionalmente podrías hacer algo más aquí
            moveDirection = transform.forward;
        }
    }

    IEnumerator RotateSpider(float rotationAngle)
    {
        float rotated = 0f;
        while (rotated < Mathf.Abs(rotationAngle))
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, Mathf.Sign(rotationAngle) * step, 0);
            rotated += step;
            yield return null;
        }
        // Actualiza la nueva dirección después de girar
        moveDirection = transform.forward;
    }
}
