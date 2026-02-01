using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum Estado
    {
        Esperando,   // Patrullando
        Alerta,
        Buscando,
        Siguiendo,
        Regresando
    }

    [Header("Estado")]
    public Estado estadoActual = Estado.Esperando;

    [Header("Movimiento")]
    public float velocidad = 2f;
    public float velocidadPersecucion = 3.5f;

    [Header("Patrullaje")]
    public Transform[] puntosPatrulla;
    public float distanciaCambioPunto = 0.2f;
    private int indicePatrulla;

    [Header("Visión")]
    public float distanciaVision = 5f;
    public Transform puntoVision;
    public LayerMask capaJugador;

    [Header("Alerta Visual")]
    public GameObject alertaVisual; // hijo que se activa/desactiva

    [Header("Tiempos")]
    public float tiempoAlerta = 1.5f;
    public float tiempoBusqueda = 2f;
    private float contadorAlerta;
    private float contadorBusqueda;

    private Transform jugador;
    private PlayerController playerController;
    private Rigidbody2D rb;
    private Animator animator;

    private float yFija;
    private Vector2 ultimaPosicionJugador;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jugador = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
        }

        yFija = transform.position.y;

        if (alertaVisual != null)
            alertaVisual.SetActive(false); // aseguramos apagado inicial
    }

    private void Update()
    {
        if (jugador == null) return;

        ActualizarAnimacion();

        switch (estadoActual)
        {
            case Estado.Esperando:
                EstadoEsperando();
                break;

            case Estado.Alerta:
                EstadoAlerta();
                break;

            case Estado.Buscando:
                EstadoBuscando();
                break;

            case Estado.Siguiendo:
                EstadoSiguiendo();
                break;

            case Estado.Regresando:
                EstadoRegresando();
                break;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.position = new Vector2(rb.position.x, yFija);
    }

    // ================= ESTADOS =================

    void EstadoEsperando()
    {
        Patrullar();

        if (PuedeVerJugador())
        {
            estadoActual = Estado.Alerta;
            contadorAlerta = tiempoAlerta;
            ActivarAlerta(true);
            GameManager.Instance?.SetEnemyAlert(true);
        }
    }

    void EstadoAlerta()
    {
        rb.linearVelocity = Vector2.zero;
        contadorAlerta -= Time.deltaTime;

        if (contadorAlerta <= 0f)
        {
            ultimaPosicionJugador = jugador.position;
            contadorBusqueda = tiempoBusqueda;
            estadoActual = Estado.Buscando;
            ActivarAlerta(false);
        }
    }

    void EstadoBuscando()
    {
        MoverHacia(ultimaPosicionJugador, velocidad);
        contadorBusqueda -= Time.deltaTime;

        if (PuedeVerJugador())
        {
            estadoActual = Estado.Siguiendo;
            ActivarAlerta(true);
            return;
        }

        if (contadorBusqueda <= 0f ||
            Vector2.Distance(transform.position, ultimaPosicionJugador) < 0.2f)
        {
            // Vamos al punto de patrulla más cercano pero no cambiamos índice todavía
            estadoActual = Estado.Regresando;
            ActivarAlerta(false);
        }
    }

    void EstadoSiguiendo()
    {
        MoverHacia(jugador.position, velocidadPersecucion);
        ultimaPosicionJugador = jugador.position;

        if (!PuedeVerJugador())
        {
            contadorBusqueda = tiempoBusqueda;
            estadoActual = Estado.Buscando;
            ActivarAlerta(false);
        }
    }

    void EstadoRegresando()
    {
        if (puntosPatrulla.Length == 0)
        {
            estadoActual = Estado.Esperando;
            return;
        }

        // Obtener el índice del punto de patrulla más cercano
        if (indicePatrulla >= puntosPatrulla.Length)
            indicePatrulla = 0;

        Transform objetivo = puntosPatrulla[indicePatrulla];
        MoverHacia(objetivo.position, velocidad);

        if (Vector2.Distance(transform.position, objetivo.position) < distanciaCambioPunto)
        {
            estadoActual = Estado.Esperando;
            // Ahora incrementamos el índice para seguir patrullando normalmente
            indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
            GameManager.Instance?.SetEnemyAlert(false);
        }
    }

    // ================= PATRULLA =================

    void Patrullar()
    {
        if (puntosPatrulla.Length == 0) return;

        Transform objetivo = puntosPatrulla[indicePatrulla];
        MoverHacia(objetivo.position, velocidad);

        if (Vector2.Distance(transform.position, objetivo.position) < distanciaCambioPunto)
        {
            indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
        }
    }

    // ================= UTILIDADES =================

    bool PuedeVerJugador()
    {
        if (playerController != null && playerController.estaOculto)
            return false;

        Vector2 direccion = (jugador.position - puntoVision.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            puntoVision.position,
            direccion,
            distanciaVision,
            capaJugador
        );

        Debug.DrawRay(puntoVision.position, direccion * distanciaVision, Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    void MoverHacia(Vector2 objetivo, float vel)
    {
        Vector2 direccion = (objetivo - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direccion.x * vel, 0f);

        // Flip solo para el enemigo
        if (direccion.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = direccion.x > 0 ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
            transform.localScale = escala;

            // Contrarrestar el flip en el hijo de alerta
            if (alertaVisual != null)
            {
                Vector3 childScale = alertaVisual.transform.localScale;
                childScale.x = Mathf.Abs(childScale.x); // mantiene siempre positivo
                alertaVisual.transform.localScale = childScale;
            }
        }
    }

    void ActualizarAnimacion()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void ActivarAlerta(bool estado)
    {
        if (alertaVisual != null)
        {
            alertaVisual.SetActive(estado);
            // No tocar la escala X, así el hijo no hace flip
            // Solo se activa o desactiva
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController pc = collision.collider.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Morir();
            }
        }
    }
}
