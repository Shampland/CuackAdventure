using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Suavizado")]
    public float smoothSpeed = 0.125f;

    [Header("Offset")]
    public float offsetX = 0f;
    public float fixedY = 0f;

    [Header("Límites del mapa")]
    public float minX;
    public float maxX;

    private void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada SOLO en X
        float desiredX = target.position.x + offsetX;

        // Limitar dentro del mapa
        float clampedX = Mathf.Clamp(desiredX, minX, maxX);

        // Suavizado
        float smoothX = Mathf.Lerp(
            transform.position.x,
            clampedX,
            smoothSpeed
        );

        // Aplicar posición final
        transform.position = new Vector3(
            smoothX,
            fixedY,
            transform.position.z
        );
    }
}
