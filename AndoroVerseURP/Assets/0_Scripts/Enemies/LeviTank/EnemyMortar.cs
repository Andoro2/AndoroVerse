using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMortar : MonoBehaviour
{
    public Transform player;         // Referencia al objeto del jugador
    public float maxHeight = 10f;    // Altura máxima de la parábola
    public float maxLaunchForce = 20f; // Fuerza máxima de lanzamiento
    public float maxLaunchAngle = 45f; // Ángulo máximo de lanzamiento en grados

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = player.position;
        targetPosition.y = initialPosition.y + maxHeight;

        // Calcular la dirección hacia el jugador
        Vector3 directionToPlayer = (targetPosition - initialPosition).normalized;

        // Calcular un factor para ajustar la velocidad en función del ángulo de lanzamiento
        float angleFactor = Mathf.Clamp01(maxLaunchAngle / 90f);

        // Calcular la velocidad inicial en el eje y para alcanzar la altura máxima
        float verticalSpeed = Mathf.Sqrt(maxLaunchForce * maxLaunchForce * Mathf.Sin(Mathf.Deg2Rad * maxLaunchAngle * 2) / Physics.gravity.magnitude);

        // Calcular la velocidad inicial en los ejes x y z
        float horizontalSpeed = maxLaunchForce * Mathf.Cos(Mathf.Deg2Rad * maxLaunchAngle);

        // Aplicar el factor de ajuste del ángulo a la velocidad
        verticalSpeed *= angleFactor;
        horizontalSpeed *= angleFactor;

        // Calcular la velocidad inicial total
        Vector3 initialVelocity = directionToPlayer * horizontalSpeed;
        initialVelocity.y = verticalSpeed;

        // Aplicar la velocidad inicial al proyectil
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;
    }
}
