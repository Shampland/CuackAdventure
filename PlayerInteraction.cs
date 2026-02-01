using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    public Transform puntoRaycast;
    public float distancia = 1.2f;
    public LayerMask capaObjetos;

    private bool mirandoDerecha = true;

    void Update()
    {
        // Dirección basada en escala (más fiable)
        mirandoDerecha = transform.localScale.x > 0;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }

    void Interactuar()
    {
        Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(
            puntoRaycast.position,
            direccion,
            distancia,
            capaObjetos
        );

        Debug.DrawRay(puntoRaycast.position, direccion * distancia, Color.red, 0.2f);

        if (hit.collider == null)
        {
            Debug.Log("No hay objeto para interactuar");
            return;
        }

        Debug.Log("Detectado: " + hit.collider.name);

        ObjetoInteractuable obj = hit.collider.GetComponent<ObjetoInteractuable>();

        if (obj != null)
        {
            obj.Interactuar();
        }
        else
        {
            Debug.LogWarning("El objeto no tiene ObjetoInteractuable");
        }
    }
}
